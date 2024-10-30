using UnityEngine;
using System.Collections.Generic;
namespace Scoring {
	public class ScoreComputer {
		readonly TimingComputer timingComputer;
		public ScoreComputer(TimingComputer timingComputer) => this.timingComputer = timingComputer;
		public ScoreResult CalculateScore(float timing) {
			ScoreType scoreType = timingComputer.Compute(timing);

			Dictionary<ScoreType, int> scoreLookup = new() {
				{ ScoreType.Perfect, 300 },
				{ ScoreType.Great, 150 },
				{ ScoreType.OK, 50 },
				{ ScoreType.Miss, 0 },
				{ ScoreType.None, 0 }
			};

			int score = scoreLookup[scoreType];
			float extraCadenza = 0f;

			if (scoreType == ScoreType.Perfect) extraCadenza = 0.02f;

			return new(score, extraCadenza);
		}
	}

	public struct ScoreResult {
		public int Score;
		public float ExtraCadenza;

		public ScoreResult(int score, float extraCadenza) {
			Score = score;
			ExtraCadenza = extraCadenza;
		}
	}
}
