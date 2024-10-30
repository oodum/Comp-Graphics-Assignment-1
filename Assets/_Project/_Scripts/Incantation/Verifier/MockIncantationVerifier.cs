using System.Linq;
namespace IncantationSystem.Verification {
	public class MockIncantationVerifier : IIncantationVerifier {
		public bool Verify(Incantation incantation, out string error) {
			error = null;
			return true;
		}
	}

	public class IncantationVerifier : IIncantationVerifier {
		static bool VerifyNotesExist(Incantation incantation, out string error) {
			if (incantation.Notes.Count == 0) {
				error = "Incantation must have at least one note";
				return false;
			}
			error = null;
			return true;
		}
		static bool VerifyNonNegativeLength(Incantation incantation, out string error) {
			if (incantation.Length < 0) {
				error = "Incantation length must be non-negative";
				return false;
			}
			error = null;
			return true;
		}
		static bool VerifyNonNegativeTiming(Incantation incantation, out string error) {
			if (incantation.Notes.Any(note => note.Timing < 0)) {
				error = "Note timings must be non-negative";
				return false;
			}
			error = null;
			return true;
		}
		static bool VerifyNotesAscending(Incantation incantation, out string error) {
			for (var i = 1; i < incantation.Notes.Count; i++) {
				if (!(incantation.Notes[i].Timing <= incantation.Notes[i - 1].Timing)) continue;
				error = "Notes must be in ascending order";
				return false;
			}
			error = null;
			return true;
		}
		static bool VerifyLength(Incantation incantation, out string error) {
			// In the Verify method, the notes are already verified to be in ascending order,
			// so we can just check the last note's timing instead of checking each note's timing.
			if (incantation.Notes.Last().Timing >= incantation.Length) {
				error = "All notes must be within the incantation's length";
				return false;
			}
			error = null;
			return true;
		}
		static bool VerifyFirstNoteIsNotAttack(Incantation incantation, out string error) {
			if (incantation.Notes[0].Type == NoteType.Attack) {
				error = "First note cannot be an attack note";
				return false;
			}
			error = null;
			return true;
		}
		static bool VerifyLastNoteIsNotCast(Incantation incantation, out string error) {
			if (incantation.Notes.Last().Type == NoteType.Cast) {
				error = "Last note cannot be a cast note";
				return false;
			}
			error = null;
			return true;
		}

		public bool Verify(Incantation incantation, out string error) {
			if (!VerifyNotesExist(incantation, out error)) return false;
			if (!VerifyNonNegativeLength(incantation, out error)) return false;
			if (!VerifyNonNegativeTiming(incantation, out error)) return false;
			if (!VerifyNotesAscending(incantation, out error)) return false;
			if (!VerifyLength(incantation, out error)) return false;
			if (!VerifyFirstNoteIsNotAttack(incantation, out error)) return false;
			if (!VerifyLastNoteIsNotCast(incantation, out error)) return false;
			return true;
		}
	}
}
