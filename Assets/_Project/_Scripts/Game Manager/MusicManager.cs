using FMODUnity;
using Sirenix.OdinInspector;
using UnityEngine;
using Utility;
namespace MusicEngine {
	[RequireComponent(typeof(MusicPlayer))]
	public class MusicManager : Singleton<MusicManager> {
		MusicPlayer musicPlayer;
		[SerializeField, Required] EventReference music;
		public double FullProgress => musicPlayer.TimelineInformation.CurrentPosition;
		public int BarLength => musicPlayer.TimelineInformation.TimeSignatureUpper;
		public double RelativeProgress => musicPlayer.TimelineInformation.CurrentRelativePosition
			% musicPlayer.TimelineInformation.TimeSignatureUpper;
		public float BPM => musicPlayer.TimelineInformation.CurrentTempo;

		protected override void Awake() {
			musicPlayer = GetComponent<MusicPlayer>();
			base.Awake();
		}

		void Start() {
			if (music.IsNull) throw new("Music reference is not set");
			musicPlayer.StartMusic(music);
		}
		
		public float ConvertProgressToTime(float progress) {
			return progress * 60 / BPM;
		} 
	}
}
