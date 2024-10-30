using IncantationSystem;
using IncantationSystem.Creation;
using IncantationSystem.Verification;
using NUnit.Framework;
using Scoring;
namespace Tests.CastableIncantationTests {
	public class CastableIncantationTests {
		IIncantationNoteListCreator creator;
		IIncantationVerifier verifier;

		[SetUp]
		public void Setup() {
			creator = new IncantationNoteListCreator();
			verifier = new IncantationVerifier();
		}

		[Test]
		public void Verify_AllNotesPlayed_ReturnsTrue() {
			var incantation = GetMockCastableIncantation();
			incantation.Play(ScoreType.Perfect);
			incantation.Play(ScoreType.OK);
			incantation.Play(ScoreType.Great);
			incantation.Play(ScoreType.Perfect);
			incantation.Play(ScoreType.Perfect);
			incantation.Play(ScoreType.Perfect);
			Assert.IsTrue(incantation.IsComplete);
		}

		[Test]
		public void Verify_UnplayedNoteIsIncomplete_ReturnsTrue() {
			var incantation = GetMockCastableIncantation();
			incantation.Play(ScoreType.Perfect);
			incantation.Play(ScoreType.Perfect);
			incantation.Play(ScoreType.Perfect);
			incantation.Play(ScoreType.Perfect);
			Assert.IsFalse(incantation.IsComplete);
		}

		CastableIncantation GetMockCastableIncantation() {
			creator.Create("0,1|2,2.5,3", out var notes, out _);
			CastableIncantation incantation = new CastableIncantation(
				new Incantation.Builder(notes)
					.WithName("Test Incantation")
					.Build(verifier, out _)
			);
			return incantation;
		}
	}
}
