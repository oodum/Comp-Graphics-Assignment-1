// Copied from https://drive.google.com/file/d/1r8ROjgsMh-mwKqGTZT7IWMCsJcs3GuU9/view

using UnityEngine;
using System;
using System.Runtime.InteropServices;
using EventBus;
using FMOD.Studio;
using FMODUnity;
using Sirenix.OdinInspector;

namespace MusicEngine {
	public class MusicPlayer : SerializedMonoBehaviour {
		FMOD.ChannelGroup masterChannelGroup;

		int masterSampleRate;
		double currentSamples;
		double currentTime;

		ulong dspClock;

		static double beatInterval;
		static double lastBeatInterval;

		double tempoTrackDSPStartTime;

		static string markerString = "";
		static bool justHitMarker;
		static int markerTime;


		double lastBeatTime = -2;
		double lastBeatDSPTime = -2;

		double lastBarTime = -2;
		double lastBarDSPTime = -2;

		float lastTimeSignatureUpper;
		float lastTimeSignatureLower;

		public static event Action<float> TempoChanged;

		public static event Action MarkerUpdated;


		[HideInInspector] public EventReference music;

		[HideInInspector] public PLAYBACK_STATE MusicPlayState = PLAYBACK_STATE.STOPPED;
		PLAYBACK_STATE lastMusicPlayState;

		GCHandle timelineHandle;

		EVENT_CALLBACK beatCallback;
		EventDescription descriptionCallback;

		[HideInInspector] public EventInstance MusicPlayEvent;

		[StructLayout(LayoutKind.Sequential), HideInInspector]
		public class TimelineInfo {
			public int CurrentBeat;
			public int CurrentBar;
			public int BeatTime;
			public int CurrentTime;
			public double CurrentPosition;
			public double CurrentRelativePosition;
			public float CurrentTempo;
			public float LastTempo;
			public int TimeSignatureUpper;
			public int TimeSignatureLower;
			public double SongLength;
			public FMOD.StringWrapper LastMarker;
		}

		public TimelineInfo TimelineInformation;

		EventBinding<BeatEvent> beatEventBinding;

		void Awake() {
			TimelineInformation = new();
		}

		void OnEnable() {
			EventBus<BeatEvent>.Register(beatEventBinding = new(OnBeat));
		}

		void OnDisable() {
			EventBus<BeatEvent>.Deregister(beatEventBinding);
		}

		void Start() {
			Preload();
		}
		void InitializeMusic(EventReference music) {
			this.music = music;
			MusicPlayEvent = RuntimeManager.CreateInstance(music);
		}

		void Preload() {
			MusicPlayEvent.getDescription(out var des);
			des.loadSampleData();
		}

		public void StartMusic(EventReference music) {
			StopMusic();
			Preload();
			InitializeMusic(music);
			MusicPlayEvent.start();
			AssignMusicCallbacks();
		}

		[Button("Start")]
		public void StartMusic() {
			if (music.IsNull) throw new("Music reference is not set");
			StartMusic(music);
		}

		double pauseDSPTime;
		int pausedTimelinePosition;
		[Button("Pause")]
		public void PauseMusic() {
			MusicPlayEvent.getTimelinePosition(out pausedTimelinePosition);
			masterChannelGroup.getDSPClock(out var dsp, out _);
			pauseDSPTime = dsp / (double)masterSampleRate;
			MusicPlayEvent.setPaused(true);
		}

		[Button("Resume")]
		public void ResumeMusic() {
			MusicPlayEvent.getPaused(out var paused);
			if (!paused) return;
			MusicPlayEvent.setPaused(false);
			masterChannelGroup.getDSPClock(out var dsp, out _);
			double resumeDSP = dsp / (double)masterSampleRate;
			double pauseDuration = resumeDSP - pauseDSPTime;
			tempoTrackDSPStartTime += pauseDuration;
			lastBeatDSPTime += pauseDuration;
			lastBarDSPTime += pauseDuration;
			MusicPlayEvent.setTimelinePosition(pausedTimelinePosition);
		}

		[Button("Stop")]
		public void StopMusic() {
			MusicPlayEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
			MusicPlayEvent.release();
		}

		void OnApplicationPause(bool pauseStatus) {
			if (pauseStatus) PauseMusic();
			else ResumeMusic();
		}

		void OnApplicationFocus(bool hasFocus) {
			if (hasFocus) ResumeMusic();
			else PauseMusic();
		}

		void AssignMusicCallbacks() {
			beatCallback = BeatEventCallback;

			timelineHandle = GCHandle.Alloc(TimelineInformation, GCHandleType.Pinned);
			MusicPlayEvent.setUserData(GCHandle.ToIntPtr(timelineHandle));
			MusicPlayEvent.setCallback(beatCallback,
				EVENT_CALLBACK_TYPE.TIMELINE_BEAT | EVENT_CALLBACK_TYPE.TIMELINE_MARKER);

			MusicPlayEvent.getDescription(out descriptionCallback);
			descriptionCallback.getLength(out int length);

			TimelineInformation.SongLength = length;

			RuntimeManager.CoreSystem.getMasterChannelGroup(out masterChannelGroup);

			RuntimeManager.CoreSystem.getSoftwareFormat(out masterSampleRate, out _, out _);
		}

		void SetTrackStartInfo() {
			UpdateDSPClock();

			tempoTrackDSPStartTime = currentTime;
			lastBeatTime = 0f;
			lastBeatDSPTime = currentTime;
		}

		void UpdateDSPClock() {
			masterChannelGroup.getDSPClock(out dspClock, out _);

			currentSamples = dspClock;
			currentTime = currentSamples / masterSampleRate;
		}

		void Update() {
			MusicPlayEvent.getPlaybackState(out MusicPlayState);
			if (lastMusicPlayState != PLAYBACK_STATE.PLAYING &&
				MusicPlayState == PLAYBACK_STATE.PLAYING) SetTrackStartInfo();
			lastMusicPlayState = MusicPlayState;
			if (MusicPlayState != PLAYBACK_STATE.PLAYING) return;

			UpdateCurrentPosition();

			UpdateDSPClock();
			CheckIfTempoMarkersChanged();

			if (beatInterval == 0f) return;

			CheckMarketHit();
			CheckNextBeat();
		}
		int lastTime;
		void UpdateCurrentPosition() {
			MusicPlayEvent.getTimelinePosition(out TimelineInformation.CurrentTime);

			double deltaTime = (TimelineInformation.CurrentTime - lastTime) / 1000d;
			lastTime = TimelineInformation.CurrentTime;

			if (TimelineInformation.TimeSignatureUpper == 0 ||
				TimelineInformation.TimeSignatureLower == 0) return;

			TimelineInformation.CurrentPosition +=
				deltaTime
				* TimelineInformation.CurrentTempo / 60d
				* TimelineInformation.TimeSignatureLower / 4d;

			TimelineInformation.CurrentRelativePosition +=
				deltaTime
				* TimelineInformation.CurrentTempo / 60d
				* TimelineInformation.TimeSignatureLower / 4d;
		}

		void CheckMarketHit() {
			if (!justHitMarker) return;
			justHitMarker = false;

			if (lastBeatDSPTime < currentTime - (beatInterval / 2f)) {
				RaiseBeatEvent(); // We trigger the beat event immediately if we're far enough past the last beat. This will help correct the timing when we hit a marker;
			}

			MusicPlayEvent.getTimelinePosition(out int currentTimelinePos);

			float offset = (currentTimelinePos - markerTime) / 1000f;

			tempoTrackDSPStartTime = currentTime - offset;
			lastBeatTime = 0f;
			lastBeatDSPTime = tempoTrackDSPStartTime;

			lastBarTime = 0f;
			lastBarDSPTime = tempoTrackDSPStartTime;

			MarkerUpdated?.Invoke();
		}

		float GetBeatInterval() => (float)beatInterval;

		void CheckNextBeat() {
			float fixedSongPosition = (float)(currentTime - tempoTrackDSPStartTime);
			float upBeatSongPosition = fixedSongPosition + GetBeatInterval();

			// DOWN BEAT
			if (fixedSongPosition >= lastBeatTime + beatInterval) {
				float correctionAmount =
					Mathf.Repeat(fixedSongPosition,
						(float)beatInterval); // This is the amount of time that we're off from the beat...

				lastBeatTime =
					(fixedSongPosition -
						correctionAmount); // ... we subtract that time from the current time to correct the timing off the next beat.
				lastBeatDSPTime =
					(currentTime -
						correctionAmount); // So if this beat is late by 0.1 seconds, the next beat will happen 0.1 seconds sooner.
			}

			// UP BEAT
			if (upBeatSongPosition >= lastBarTime + beatInterval) {
				float correctionAmount = Mathf.Repeat(upBeatSongPosition, (float)beatInterval);


				lastBarTime = (upBeatSongPosition - correctionAmount);
				lastBarDSPTime = ((currentTime + GetBeatInterval()) - correctionAmount);
			}
		}
		void RaiseBeatEvent() => RaiseBeatEvent(TimelineInformation);

		static void RaiseBeatEvent(TimelineInfo info) {
			EventBus<BeatEvent>.Raise(new(info.CurrentBeat));
		}

		void OnBeat(BeatEvent beatEvent) {
			if (beatEvent.IsDownBeat) OnBar();
		}

		void OnBar() {
			TimelineInformation.CurrentRelativePosition = 0;
		}

		void CheckIfTempoMarkersChanged() {
			if (!Mathf.Approximately(TimelineInformation.CurrentTempo,
				TimelineInformation.LastTempo)) SetTrackTempo();
			if (!Mathf.Approximately(TimelineInformation.TimeSignatureUpper, lastTimeSignatureUpper)
				|| !Mathf.Approximately(TimelineInformation.TimeSignatureLower,
					lastTimeSignatureLower)) SetTrackTempo();
		}

		void SetTrackTempo() {
			MusicPlayEvent.getTimelinePosition(out int currentTimelinePos);

			float offset = (currentTimelinePos - TimelineInformation.BeatTime) / 1000f;

			tempoTrackDSPStartTime = currentTime - offset;

			lastBeatTime = 0f;
			lastBeatDSPTime = tempoTrackDSPStartTime;

			lastBarTime = 0f;
			lastBarDSPTime = tempoTrackDSPStartTime;

			lastTimeSignatureUpper = TimelineInformation.TimeSignatureUpper;
			lastTimeSignatureLower = TimelineInformation.TimeSignatureLower;

			lastBeatInterval = beatInterval;

			TimelineInformation.LastTempo = TimelineInformation.CurrentTempo;

			beatInterval = 60f / TimelineInformation.CurrentTempo;

			TempoChanged?.Invoke((float)beatInterval);
		}

		[AOT.MonoPInvokeCallback(typeof(EVENT_CALLBACK))]
		static FMOD.RESULT BeatEventCallback(EVENT_CALLBACK_TYPE type, IntPtr instancePtr,
			IntPtr parameterPtr) {
			EventInstance instance = new EventInstance(instancePtr);

			// Retrieve the user data
			FMOD.RESULT result = instance.getUserData(out IntPtr timelineInfoPtr);
			if (result != FMOD.RESULT.OK) {
				Debug.LogError("Timeline Callback error: " + result);
			} else if (timelineInfoPtr != IntPtr.Zero) {
				// Get the object to store beat and marker details
				GCHandle timelineHandle = GCHandle.FromIntPtr(timelineInfoPtr);
				TimelineInfo timelineInfo = (TimelineInfo)timelineHandle.Target;

				switch(type) {
					case EVENT_CALLBACK_TYPE.TIMELINE_BEAT: {
						// There's more info about the callback in the "parameter" variable.
						var parameter =
							(TIMELINE_BEAT_PROPERTIES)Marshal.PtrToStructure(parameterPtr,
								typeof(TIMELINE_BEAT_PROPERTIES));
						timelineInfo.CurrentBar = parameter.bar;
						timelineInfo.CurrentBeat = parameter.beat;
						timelineInfo.BeatTime = parameter.position;
						timelineInfo.CurrentTempo = parameter.tempo;
						timelineInfo.TimeSignatureUpper = parameter.timesignatureupper;
						timelineInfo.TimeSignatureLower = parameter.timesignaturelower;
						RaiseBeatEvent(timelineInfo);
					}
						break;
					case EVENT_CALLBACK_TYPE.TIMELINE_MARKER: {
						var parameter =
							(TIMELINE_MARKER_PROPERTIES)Marshal.PtrToStructure(parameterPtr,
								typeof(TIMELINE_MARKER_PROPERTIES));
						timelineInfo.LastMarker = parameter.name;
						markerString = parameter.name;
						markerTime = parameter.position;
						justHitMarker = true;
					}
						break;
				}
			}

			return FMOD.RESULT.OK;
		}
	}
	public struct BeatEvent : IEvent {
		public bool IsDownBeat => Beat == 1;
		public int Beat; // 0 is first beat

		public BeatEvent(int beat) => Beat = beat;
	}
}
