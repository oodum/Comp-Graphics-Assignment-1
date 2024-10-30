using System;
using UnityEngine;
namespace Scoring {
	public class TimingComputer {
		public const float PERFECT_THRESHOLD = 0.06f;
		public const float GREAT_THRESHOLD = 0.1f;
		public const float OK_THRESHOLD = 0.25f;
		public const float MISS_THRESHOLD = 0.325f;

		public ScoreType Compute(float timing) {
			timing = Math.Abs(timing);
			return timing switch {
				<= PERFECT_THRESHOLD => ScoreType.Perfect,
				<= GREAT_THRESHOLD and > PERFECT_THRESHOLD => ScoreType.Great,
				<= OK_THRESHOLD and > GREAT_THRESHOLD => ScoreType.OK,
				<= MISS_THRESHOLD and > OK_THRESHOLD => ScoreType.Miss,
				_ => ScoreType.None
			};

		}
	}

}