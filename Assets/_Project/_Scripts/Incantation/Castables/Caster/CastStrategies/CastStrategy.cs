using System;
using System.Linq;
using GameEntity;
using MusicEngine;
using Scoring;
using Sirenix.Utilities;
using UnityEngine;

namespace IncantationSystem.Castables {
	public abstract class CastStrategy {
		readonly ConfidenceComputer confidenceComputer;

		protected CastStrategy(ConfidenceComputer confidenceComputer) { this.confidenceComputer = confidenceComputer; }

		public abstract Func<IncantationQuiver, bool> Predicate { get; }
		public abstract ICastCommand Cast(IncantationQuiver quiver, float timing);

		protected float ComputeTimingDifferenceOfUpcomingNote(CastableIncantation incantation,
			float realTiming) {
			if (incantation.IsComplete) return 0;
			var noteTiming = incantation.GetNextUnplayedNote().Timing;
			var result = MusicManager.Instance.ConvertProgressToTime(realTiming - noteTiming);
			return result;
		}

		protected CastableIncantation GetTopConfidenceIncantation(IncantationQuiver quiver) {
			CastableIncantation topIncantation = null;
			var maxConfidence = float.MinValue;
			foreach (var incantation in quiver.Incantations) {
				if (incantation.Confidence > maxConfidence) {
					maxConfidence = incantation.Confidence;
					topIncantation = incantation;
				}
			}

			return topIncantation;
		}

		protected bool IsAlive(IncantationQuiver quiver) { return quiver.Incantations.Any(incantation => !incantation.IsFailed); }

		protected void Play(CastableIncantation incantation, float difference) {
			var penalization = GetConfidencePenalization(difference, out var rating);
			incantation.Play(rating, penalization);
		}

		protected void PlayWithoutConfidence(CastableIncantation incantation, float difference) {
			var rating = confidenceComputer.ComputeRating(difference);
			incantation.Play(rating, 0);
		}

		protected float GetConfidencePenalization(float difference, out ScoreType rating) {
			const float NUDGE_FACTOR = 0.1f; // Determines by how much to separate two slightly different timings with the same rating;
			rating = confidenceComputer.ComputeRating(difference);
			return 1 - (confidenceComputer.ComputeConfidence(difference) + difference * NUDGE_FACTOR);
		}
	}

	public class CastingStrategy : CastStrategy {
		readonly Entity entity;

		public CastingStrategy(Entity entity, ConfidenceComputer confidenceComputer) : base(confidenceComputer) { this.entity = entity; }

		public override Func<IncantationQuiver, bool> Predicate => quiver =>
			IsAlive(quiver)
			&& quiver.Incantations.TrueForAll(incantation =>
				!incantation.IsComplete && incantation.GetNextUnplayedNote().Type == NoteType.Cast
				|| incantation.IsFailed && incantation.GetNextUnplayedNote().Type == NoteType.Attack);

		public override ICastCommand Cast(IncantationQuiver quiver, float timing) {
			foreach (var incantation in quiver.Incantations) {
				if (incantation.IsFailed) continue;
				var difference = ComputeTimingDifferenceOfUpcomingNote(incantation, timing);
				Play(incantation, difference);
			}

			var topIncantation = GetTopConfidenceIncantation(quiver);
			return new CastCommand(entity, topIncantation);
		}
	}

	// This takes care of the case where the most likely incantation is a cast, but an attack is also possible
	// The job of this strategy is to disambiguate between the two, collapsing the ambiguity to either a cast or an attack
	public class AmbiguousStrategy : CastStrategy {
		readonly CastingStrategy castingStrategy;
		readonly AttackingStrategy attackingStrategy;

		public AmbiguousStrategy(ConfidenceComputer confidenceComputer, CastingStrategy castingStrategy, AttackingStrategy attackingStrategy) :
			base(confidenceComputer) {
			this.castingStrategy = castingStrategy;
			this.attackingStrategy = attackingStrategy;
		}

		public override Func<IncantationQuiver, bool> Predicate => quiver => {
			var topIncantation = GetTopConfidenceIncantation(quiver);
			return topIncantation.GetNextUnplayedNote().Type == NoteType.Cast
			       && quiver.Incantations.Any(incantation => !incantation.IsFailed && incantation.GetNextUnplayedNote().Type == NoteType.Attack);
		};

		public override ICastCommand Cast(IncantationQuiver quiver, float timing) {
			var sorted = quiver.Incantations.OrderByDescending(incantation => incantation.Confidence).ToList();
			var castIncantation = sorted.First(); // We can assume this because the predicate asserted that the top incantation is a cast
			var attackIncantation = sorted.First(incantation => incantation.GetNextUnplayedNote().Type == NoteType.Attack);
			if (GetPotentialConfidence(castIncantation, timing) > GetPotentialConfidence(attackIncantation, timing)) {
				attackIncantation.Confidence = 0; // Set confidence to 0 to remove the ambiguity
				return castingStrategy.Cast(quiver, timing);
			} else {
				castIncantation.Confidence = 0;
				return attackingStrategy.Cast(quiver, timing);
			}
		}

		// Determines the confidence of an incantation if it were to be played
		float GetPotentialConfidence(CastableIncantation incantation, float difference) {
			var penalization = GetConfidencePenalization(difference, out _);
			return incantation.Confidence - penalization;
		}
	}

	public class AttackingStrategy : CastStrategy {
		CastableIncantation chosenIncantation;
		readonly Entity entity;

		public AttackingStrategy(Entity entity, ConfidenceComputer confidenceComputer) :
			base(confidenceComputer) {
			this.entity = entity;
		}

		public override Func<IncantationQuiver, bool> Predicate => quiver => {
			chosenIncantation = GetTopConfidenceIncantation(quiver);
			return !chosenIncantation.IsComplete && chosenIncantation.GetNextUnplayedNote().Type == NoteType.Attack;
		};

		public override ICastCommand Cast(IncantationQuiver quiver, float timing) {
			DisqualifyNonTopIncantations(quiver);
			var difference = ComputeTimingDifferenceOfUpcomingNote(chosenIncantation, timing);
			PlayWithoutConfidence(chosenIncantation, difference);
			chosenIncantation.GetLastPlayedNote(out var rating);
			var adjustedDamage = (int)(5 * DamageComputer.GetDamageMultiplier(rating));
			return new AttackCommand(entity, chosenIncantation, adjustedDamage);
		}

		void DisqualifyNonTopIncantations(IncantationQuiver quiver) {
			if (quiver.Incantations.Count(incantation => incantation.Confidence > 0) == 1) return;
			foreach (CastableIncantation incantation in quiver.Incantations.Where(incantation =>
				         incantation != chosenIncantation)) {
				incantation.Confidence = 0;
			}
		}
	}

	public class CompletedStrategy : CastStrategy {
		public CompletedStrategy(ConfidenceComputer confidenceComputer) :
			base(confidenceComputer) {
		}

		public override Func<IncantationQuiver, bool> Predicate => quiver =>
			quiver.Incantations.Any(incantation => incantation.IsComplete);

		public override ICastCommand Cast(IncantationQuiver quiver, float timing) { return new NullCastCommand(); }
	}

	public class FailedStrategy : CastStrategy {
		public FailedStrategy(ConfidenceComputer confidenceComputer) : base(confidenceComputer) { }

		public override Func<IncantationQuiver, bool> Predicate => quiver =>
			quiver.Incantations.All(incantation => incantation.Confidence <= 0);

		public override ICastCommand Cast(IncantationQuiver quiver, float timing) { return new NullCastCommand(); }
	}
}