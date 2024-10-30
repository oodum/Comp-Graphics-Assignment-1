using System.Collections.Generic;
using IncantationSystem;
using IncantationSystem.Creation;
using IncantationSystem.Verification;
using NUnit.Framework;
using UnityEngine;
namespace Tests.IncantationTests {
	public class IncantationVerificationTests {
		IncantationNoteListCreator creator;
		IIncantationVerifier verifier;
		[SetUp]
		public void Setup() {
			creator = new IncantationNoteListCreator();
			verifier = new IncantationVerifier();
		}

		[Test]
		public void Verify_EmptyIncantation_ReturnsFalse() {
			List<Note> notes = new();
			var incantation = Create(notes);
			Assert.IsNull(incantation);
		}

		[Test]
		public void Verify_NegativeLength_ReturnsFalse() {
			creator.Create("1,2,3|4,5,6", out var notes, out _);
			var incantation = new Incantation.Builder(notes)
				.WithLength(-1)
				.Build(verifier, out var error);
			Debug.Log(error);
			Assert.IsNull(incantation);
		}

		[Test]
		public void Verify_NegativeTiming_ReturnsFalse() {
			creator.Create("-1,0,1|2", out var notes, out _);
			var incantation = Create(notes);
			Assert.IsNull(incantation);
		}

		[Test]
		public void Verify_NotesNotAscending_ReturnsFalse() {
			creator.Create("1,0,3|4,5,6", out var notes, out _);
			var incantation = Create(notes);
			Assert.IsNull(incantation);
		}

		[Test]
		public void Verify_CastOnlyIncantation_ReturnsFalse() {
			List<Note> notes = new() {
				new(0, NoteType.Cast),
				new(1, NoteType.Cast),
				new(2, NoteType.Cast),
			};
			var incantation = Create(notes);
			Assert.IsNull(incantation);
		}

		[Test]
		public void Verify_AttackOnlyIncantation_ReturnsFalse() {
			List<Note> notes = new() {
				new(0, NoteType.Attack),
				new(1, NoteType.Attack),
				new(2, NoteType.Attack),
			};
			var incantation = Create(notes);
			Assert.IsNull(incantation);
		}

		[Test]
		public void Verify_TimingExceedsLength_ReturnsFalse() {
			creator.Create("1,2,3|4,5,6", out var notes, out _);
			var incantation = new Incantation.Builder(notes)
				.WithLength(2)
				.Build(verifier, out var error);
			Debug.Log(error);
			Assert.IsNull(incantation);
		}

		[Test]
		public void Verify_CorrectIncantation_ReturnsTrue() {
			creator.Create("0,1|2,2.5,3", out var notes, out _);
			var incantation = Create(notes);
			Assert.IsNotNull(incantation);
		}

		Incantation Create(List<Note> notes) {
			var incantation = new Incantation.Builder(notes).Build(verifier, out var error);
			Debug.Log(error);
			return incantation;
		}
	}
}
