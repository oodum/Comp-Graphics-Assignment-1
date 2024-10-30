namespace Scoring {
	public class DodgeComputer {
		readonly TimingComputer timingComputer;
		public DodgeComputer(TimingComputer timingComputer) => this.timingComputer = timingComputer;
		public DodgeResult CalculateDodge(float timing) {
			ScoreType scoreType = timingComputer.Compute(timing);

			switch(scoreType) {
				case ScoreType.Perfect:
					return new(0f, 0.02f, true);
				case ScoreType.Great:
					return new(0f, 0f, true);
				case ScoreType.OK:
					return new(0.25f, 0f, true);
				case ScoreType.Miss:
					return new(1f, 0f, false);
			}
			// If scoretype is none?? not really sure what to do here
			return new(1f, 0f, false);
		}
	}

	public struct DodgeResult {
		public float DamageMultiplier;
		public float ExtraCadenza;
		public bool Moving;

		public DodgeResult(float damageMultiplier, float extraCadenza, bool moving) {
			DamageMultiplier = damageMultiplier;
			ExtraCadenza = extraCadenza;
			Moving = moving;
		}
	}
}