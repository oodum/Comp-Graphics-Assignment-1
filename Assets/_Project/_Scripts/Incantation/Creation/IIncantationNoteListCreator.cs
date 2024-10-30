using System.Collections.Generic;
using System.Linq;
namespace IncantationSystem.Creation {
	public interface IIncantationNoteListCreator {
		public bool Create(string input, out List<Note> notes, out string error);
	}

	public class IncantationNoteListCreator : IIncantationNoteListCreator {
		// This incantation creator only assumes one cast pattern and one attack pattern
		// separated by a pipe character '|'
		// The cast pattern and attack pattern are comma-separated floats
		public bool Create(string input, out List<Note> notes, out string error) {
			notes = null;
			error = null;

			if (!Parse(input, out List<float> castPattern, out List<float> attackPattern, out error)) return false;
			notes = new();
			CreateCastPattern(notes, castPattern);
			CreateAttackPattern(notes, attackPattern);
			return true;
		}

		static bool Parse(string input, out List<float> castPattern, out List<float> attackPattern,
			out string error) {
			attackPattern = null;
			castPattern = null;
			error = null;

			string[] patterns = input.Split('|');
			if (patterns.Length != 2) {
				error =
					"Input must contain exactly one cast pattern and one attack pattern separated by a pipe character '|'";
				return false;
			}

			if (!ParsePattern(patterns[0], out castPattern, out error)) return false;
			if (!ParsePattern(patterns[1], out attackPattern, out error)) return false;

			return true;
		}

		static bool ParsePattern(string pattern, out List<float> timings, out string error) {
			timings = null;
			error = null;

			string[] split = pattern.Split(',');
			timings = new();
			foreach (string number in split) {
				if (float.TryParse(number, out var timing)) {
					timings.Add(timing);
					continue;
				}
				error = $"Failed to parse timing value: {number}";
				return false;
			}

			return true;
		}

		static void CreateCastPattern(List<Note> notes, List<float> castPattern) {
			notes.AddRange(castPattern.Select(timing => new Note(timing, NoteType.Cast)));
		}
		static void CreateAttackPattern(List<Note> notes, List<float> attackPattern) {
			notes.AddRange(attackPattern.Select(timing => new Note(timing, NoteType.Attack)));
		}
	}
}
