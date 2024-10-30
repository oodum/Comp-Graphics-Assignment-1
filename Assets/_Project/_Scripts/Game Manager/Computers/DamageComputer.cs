using System.Collections.Generic;
namespace Scoring {
	public static class DamageComputer {
		static readonly Dictionary<ScoreType, float> damageLookup = new() {
			{ ScoreType.Perfect, 1f },
			{ ScoreType.Great, 1f },
			{ ScoreType.OK, 0.5f },
			{ ScoreType.Miss, 0f },
			{ ScoreType.None, 0f },
		};
		public static float GetDamageMultiplier(ScoreType scoreType) {

			return damageLookup[scoreType];
		}
	}
}
