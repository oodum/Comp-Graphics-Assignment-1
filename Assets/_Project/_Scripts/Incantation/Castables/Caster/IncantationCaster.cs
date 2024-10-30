using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EventBus;
using Extensions;
using GameEntity;
using MusicEngine;
using Scoring;
using Sirenix.OdinInspector;
using UnityEngine;

namespace IncantationSystem.Castables {
	[RequireComponent(typeof(IncantationQuiver))]
	public class IncantationCaster : SerializedMonoBehaviour {
		IncantationQuiver quiver;
		ConfidenceComputer confidenceComputer;
		Entity entity;
		List<CastStrategy> castStrategies;
		[SerializeField] FMODUnity.EventReference castSound;
		[SerializeField] bool showDebugInfo;

		EventBinding<BeatEvent> beatEvent;

		[HideInInspector] public bool MarkNoReset;

		void Awake() {
			quiver = gameObject.GetOrAdd<IncantationQuiver>();
			entity = gameObject.GetOrAdd<Entity>();
		}

		void OnEnable() {
			beatEvent = new(OnBeat);
			EventBus<BeatEvent>.Register(beatEvent);
		}

		void OnDisable() { EventBus<BeatEvent>.Deregister(beatEvent); }

		void Start() {
			ServiceLocator.ServiceLocator.For(this).Get(out confidenceComputer);
			castStrategies = new() {
				new CastingStrategy(entity, confidenceComputer),
				new AttackingStrategy(entity, confidenceComputer),
				new CompletedStrategy(confidenceComputer),
				new FailedStrategy(confidenceComputer),
			};
			// The ambiguous strategy needs a reference to the casting and attacking strategies
			// More information is inside the ambiguous strategy class
			var castStrategy = castStrategies.Find(strategy => strategy is CastingStrategy) as CastingStrategy;
			var attackingStrategy = castStrategies.Find(strategy => strategy is AttackingStrategy) as AttackingStrategy;
			castStrategies.Add(new AmbiguousStrategy(confidenceComputer, castStrategy, attackingStrategy));
		}


		public ICastCommand Cast(float realTiming) {
			// Find the first strategy that matches the correct state
			CastStrategy strategy = castStrategies.Find(strategy => strategy.Predicate(quiver));
			FMODUnity.RuntimeManager.PlayOneShotAttached(castSound, gameObject);
			return strategy?.Cast(quiver, realTiming);
		}

		void OnBeat(BeatEvent @event) {
			if (@event.IsDownBeat) {
				Reset();
			}
		}

		void Reset() {
			if (MarkNoReset) {
				MarkNoReset = false;
				return;
			}

			foreach (var incantation in quiver.Incantations) {
				incantation.Reset();
			}

			EventBus<IncantationResetEvent>.Raise(new(entity));
		}

		void OnGUI() {
			if (!showDebugInfo) return;
			StringBuilder sb = new();
			foreach (var incantation in quiver.Incantations) {
				sb.AppendLine(incantation.Name);
				sb.AppendLine($"  Confidence: {incantation.Confidence}");
				sb.AppendLine($"  CurrentIndex: {incantation.CurrentIndex}");
				sb.AppendLine($"  {GetIncantationState(incantation)}");
				sb.AppendLine();
			}

			GUI.Label(new(new(0, 150), new(400, 1000)), sb.ToString());
		}

		string GetIncantationState(CastableIncantation incantation) {
			string state = "";
			foreach (var note in incantation.Notes) {
				state += incantation.GetRating(note) == ScoreType.None ? $"{note.Timing} " : "X ";
			}

			return state;
		}
	}
}