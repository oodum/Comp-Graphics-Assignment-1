using System;
using EventBus;
using FMODUnity;
using Utility;
using UnityEngine;
namespace MusicEngine {
	public class Metronome : Singleton<Metronome> {
		[SerializeField] EventReference metronomeTickSound;
		[SerializeField] bool showDebugInfo;
		EventBinding<BeatEvent> beatEvent;

		int currentBeat;
		bool isDownBeat;

		void OnEnable() {
			beatEvent = new(OnBeat);
			EventBus<BeatEvent>.Register(beatEvent);
		}

		void OnDisable() {
			EventBus<BeatEvent>.Deregister(beatEvent);
		}

		void OnBeat(BeatEvent beatEvent) {
			RuntimeManager.PlayOneShotAttached(metronomeTickSound, gameObject);
			currentBeat = beatEvent.Beat;
			isDownBeat = beatEvent.IsDownBeat;
		}

		void OnGUI() {
			if (!showDebugInfo) return;
			GUILayout.Label($"Full Progress: {MusicManager.Instance.FullProgress:F3}");
			GUILayout.Label($"Relative Progress: {MusicManager.Instance.RelativeProgress:F3}");
			GUILayout.Label($"Beat: {currentBeat}");
			GUILayout.Label($"Is down beat: {isDownBeat}");
			GUILayout.Label($"Current BPM: {MusicManager.Instance.BPM}");
		}
	}
}
