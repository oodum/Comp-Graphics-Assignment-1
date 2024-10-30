using IncantationSystem.Creation;
using NUnit.Framework;
using UnityEngine;
namespace Tests.IncantationTests {
	public class NoteListCreationTests {
		IIncantationNoteListCreator creator;
		[SetUp]
		public void Setup() {
			creator = new IncantationNoteListCreator();
		}
		[Test]
		public void Create_NoNotes_ReturnsFalse() {
			var result = creator.Create("", out _, out var error);
			Debug.Log(error);
			Assert.IsFalse(result);
		}

		[Test]
		public void Create_CastOnly_ReturnsFalse() {
			var result = creator.Create("1,2,3", out _, out var error);
			Debug.Log(error);
			Assert.IsFalse(result);
		}

		[Test]
		public void Create_ThreePatterns_ReturnsFalse() {
			var result = creator.Create("1,2,3|4,5,6|7,8,9", out _, out var error);
			Debug.Log(error);
			Assert.IsFalse(result);
		}

		[Test]
		public void Create_BadString_ReturnsFalse() {
			var result = creator.Create("1,2,3|4ads,5,6,7", out _, out var error);
			Debug.Log(error);
			Assert.IsFalse(result);
		}

		[Test]
		public void Create_OneCastOneAttack_ReturnsTrue() {
			var result = creator.Create("1,2,3|4,5,6", out var incantation, out var error);
			Debug.Log(error);
			Assert.IsTrue(result);
		}
	}
}
