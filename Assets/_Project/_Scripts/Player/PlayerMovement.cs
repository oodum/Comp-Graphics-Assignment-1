using System;
using EventBus;
using Extensions;
using Input;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Combat {
	public class PlayerMovement : MonoBehaviour {
		[SerializeField, Required] PlayerInputProcessor inputProcessor;
		CombatManager combatManager;
		bool IsEnemyTurn => combatManager.CurrentCombatState == CombatManager.CombatState.Bad;
		public bool CanDash = true;

		[SerializeField] float dashDistance;
		[SerializeField] float dashSpeed;
		Vector2 position;

		void Start() {
			ServiceLocator.ServiceLocator.Global.Get(out combatManager);
			position = transform.position.Flatten();
		}

		void OnEnable() {
			inputProcessor.OnUpPressed += MoveUp;
			inputProcessor.OnDownPressed += MoveDown;
			inputProcessor.OnLeftPressed += MoveLeft;
			inputProcessor.OnRightPressed += MoveRight;
		}

		void OnDisable() {
			inputProcessor.OnUpPressed -= MoveUp;
			inputProcessor.OnDownPressed -= MoveDown;
			inputProcessor.OnLeftPressed -= MoveLeft;
			inputProcessor.OnRightPressed -= MoveRight;
		}

		void MoveUp() { Dash(Vector2.up); }

		void MoveDown() { Dash(Vector2.down); }

		void MoveLeft() { Dash(Vector2.left); }

		void MoveRight() { Dash(Vector2.right); }

		void Update() { transform.position = Vector3.MoveTowards(transform.position, position.UnFlatten(), dashSpeed).With(y: transform.position.y); }

		void Dash(Vector2 direction) {
			if (!IsEnemyTurn || !CanDash) return;
			position += direction * dashDistance;
			EventBus<DashEvent>.Raise(new(this));
		}
	}
}