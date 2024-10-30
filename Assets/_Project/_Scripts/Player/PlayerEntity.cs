using System;
using Combat;
using IncantationSystem.Castables;
using UnityEngine;
namespace GameEntity {
	[RequireComponent(typeof(PlayerCaster), typeof(PlayerMovement))]
	public class PlayerEntity : Entity {
		CombatManager combatManager;

		void Awake() {
			ServiceLocator.ServiceLocator.For(this).Register(this);
		}

		void Start() {
			ServiceLocator.ServiceLocator.For(this).Get(out combatManager);
			Register();
		}

		protected override void Register() {
			combatManager.RegisterAsGoodEntity(this);
		}
		protected override void Deregister() {
			combatManager.DeregisterEntity(this);
		}
	}
}
