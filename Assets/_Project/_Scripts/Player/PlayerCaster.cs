using System;
using System.Linq;
using Combat;
using EventBus;
using Extensions;
using GameEntity;
using Input;
using MusicEngine;
using Scoring;
using Sirenix.OdinInspector;
using UnityEngine;

namespace IncantationSystem.Castables {
	[RequireComponent(typeof(IncantationCaster))]
	public class PlayerCaster : MonoBehaviour {
		IncantationCaster caster;
		IncantationQuiver quiver;
		PlayerEntity player;
		[SerializeField, Required] PlayerInputProcessor inputProcessor;

		MusicManager musicManager;
		CombatManager combatManager;
		EventBinding<IncantationResetEvent> incantationResetEvent;

		[SerializeField] bool showDebugInfo;

		void Awake() {
			caster = gameObject.GetOrAdd<IncantationCaster>();
			quiver = gameObject.GetOrAdd<IncantationQuiver>();
			player = gameObject.GetOrAdd<PlayerEntity>();
		}

		void OnEnable() {
			inputProcessor.OnCastPressed += Cast;
			EventBus<IncantationResetEvent>.Register(incantationResetEvent = new(OnIncantationReset));
		}

		void OnDisable() {
			inputProcessor.OnCastPressed -= Cast;
			EventBus<IncantationResetEvent>.Deregister(incantationResetEvent);
		}

		void Start() {
			inputProcessor.SetFight(); // TODO: Refactor
			musicManager = MusicManager.Instance;
			ServiceLocator.ServiceLocator.For(this).Get(out combatManager);
		}

		void Cast() {
			float timing = (float)musicManager.RelativeProgress;
			if (combatManager.CurrentCombatState != CombatManager.CombatState.Good
			    && !IsBuffered(ref timing)) return;
			var castCommand = caster.Cast(timing);
			casts += $"{timing:F2} ";
			castCommand.Raise();
		}

		bool IsBuffered(ref float timing) {
			if (quiver.Incantations.Any(i => i.IsComplete || i.IsFailed)) return false;
			float inverseTiming = Mathf.Abs(musicManager.BarLength - timing);
			if (Mathf.Approximately(inverseTiming, 4)) return true;
			if (musicManager.ConvertProgressToTime(inverseTiming) <= TimingComputer.MISS_THRESHOLD) {
				timing = -inverseTiming;
				caster.MarkNoReset = true;
				return true;
			}

			return false;
		}

		void OnIncantationReset(IncantationResetEvent @event) {
			if (@event.Entity.ID == player.ID)
				casts = String.Empty;
		}

		string casts = String.Empty;

		void OnGUI() {
			if (!showDebugInfo) return;
			GUI.Label(new(new(200, 130), new(400, 1000)), casts);
		}
	}
}