using System.Collections.Generic;
namespace IncantationSystem {
	public class Incantation {
		public string Name { get; set; }
		public List<Note> Notes { get; private set; }
		public int Length { get; private set; }

		Incantation() { } // private constructor to enforce builder

		public class Builder {
			readonly Incantation incantation;
			public Builder(List<Note> notes) {
				incantation = new() {
					Notes = new(notes), // shallow copy
					Length = 4,
				};
			}

			public Builder WithName(string name) {
				incantation.Name = name;
				return this;
			}

			public Builder WithLength(int length) {
				incantation.Length = length;
				return this;
			}

			public Incantation Build(IIncantationVerifier verifier, out string error) {
				return verifier.Verify(incantation, out error) ? incantation : null;
			}
		}
	}

}
