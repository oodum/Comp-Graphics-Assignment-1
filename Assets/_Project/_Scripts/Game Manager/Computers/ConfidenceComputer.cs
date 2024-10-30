using UnityEngine;
using System.Collections.Generic;
namespace Scoring {
	public class ConfidenceComputer {
		readonly TimingComputer timingComputer;
		public ConfidenceComputer(TimingComputer timingComputer) =>
			this.timingComputer = timingComputer;

		readonly Dictionary<ScoreType, float> ConfidenceLookup = new() {
			{ ScoreType.Perfect, 1 },
			{ ScoreType.Great, 0.9f },
			{ ScoreType.OK, 0.6f },
			{ ScoreType.Miss, 0 },
			{ ScoreType.None, 0.8f }, // Slightly penalize none so its confidence doesn't trump a cast with a great or above rating
		};

		public float ComputeConfidence(float timing) {
			ScoreType scoreType = timingComputer.Compute(timing);
			return ConfidenceLookup[scoreType];
		}

		public ScoreType ComputeRating(float timing) => timingComputer.Compute(timing);
	}
}
