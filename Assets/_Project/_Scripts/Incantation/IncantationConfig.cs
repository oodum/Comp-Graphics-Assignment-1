using System.Collections.Generic;
using IncantationSystem.Creation;
using IncantationSystem.Verification;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace IncantationSystem {
	[CreateAssetMenu(fileName = "IncantationConfig", menuName = "Incantation/IncantationConfig")]
	public class IncantationConfig : SerializedScriptableObject {
		[SerializeField] string Name;
		[SerializeField] int Length = 4;

		[SerializeField, PropertyOrder(1)] List<Note> Notes;


		[FormerlySerializedAs("noteStrings")]
		[SerializeField, HorizontalGroup("Creation"), LabelText("Timing Values")]
		string noteInput;

		[Button, HorizontalGroup("Creation", width: 100)]
		void CreateNotes() {
			verifyColor = Color.white;
			var noteListCreator = new IncantationNoteListCreator();

			if (!noteListCreator.Create(noteInput, out Notes, out string error)) {
				Error(error);
				return;
            }

			VerifyButton();
		}


		Color verifyColor = Color.black;
		string verifyText = "Press the Create Notes button to view verification result.";
		float whiteScale = .5f;

		[InfoBox("@verifyText"), GUIColor("@verifyColor")]
		[Button, PropertyOrder(2)]
		void VerifyButton() {
			if (!Verify(out _, out string error)) {
				Error(error);
				return;
			}
			Success("Incantation is verified!");
		}


		bool Verify(out Incantation incantation, out string error) {
			incantation = new Incantation.Builder(Notes)
				.WithName(Name)
				.WithLength(Length)
				.Build(new IncantationVerifier(), out error);
			return incantation != null;
		}

		void Error(string error) {
			verifyColor = new(1, whiteScale, whiteScale);
			verifyText = error;
		}

		void Success(string message) {
			verifyColor = new(whiteScale, 1, whiteScale);
			verifyText = message;
		}

		public Incantation Build() {
			if (Verify(out var incantation, out var error)) return incantation;
			throw new(error);
		}
	}
}
