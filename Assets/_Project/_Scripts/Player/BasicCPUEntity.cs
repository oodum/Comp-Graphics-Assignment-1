using System;
using System.Threading.Tasks;
using Combat;
using EventBus;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameEntity {
	public class BasicCPUEntity : Entity {
		static readonly int FRESNEL = Shader.PropertyToID("_Fresnel");
		static readonly int TRANSPARENCY = Shader.PropertyToID("_Transparency");
		CombatManager combatManager;
		[SerializeField] Material hologramShader;

		void Start() {
			ServiceLocator.ServiceLocator.For(this).Get(out combatManager);
			Register();
		}

		protected override void Register() { combatManager.RegisterAsBadEntity(this); }

		protected override void Deregister() { combatManager.DeregisterEntity(this); }

		public override void Kill() {
			Deregister();
			ApplyMaterialToDescendants(transform);
			_ = KillSequence();
		}

		void ApplyMaterialToDescendants(Transform transform) {
			foreach (Transform child in transform) {
				ApplyMaterialToDescendants(child);
				if (child.TryGetComponent(out Renderer foundRenderer)) {
					/*  for (int i = 0; i < foundRenderer.materials.Length; i++) { // this didn't work
						foundRenderer.materials[i] = hologramShader;
					} */
					foundRenderer.material = hologramShader;
				}
			}
		}

		async Task KillSequence() {
			await AnimateHologram();
			EventBus<WinEvent>.Raise(new());
			Destroy(gameObject);
		}

		async Task AnimateHologram() {
			float duration = 1f; // Duration in seconds
			float targetValue = 10f;
			float startValue = 0f;
			float elapsedTime = 0f;

			while (elapsedTime < duration) {
				elapsedTime += Time.deltaTime;
				float currentValue = Mathf.Lerp(startValue, targetValue, elapsedTime / duration);
				hologramShader.SetFloat(FRESNEL, currentValue);
				if (elapsedTime/ duration > 0.8f) {
					float transparency = Mathf.Lerp(1, 0, Mathf.InverseLerp(0.8f * duration, duration, elapsedTime));
					hologramShader.SetFloat(TRANSPARENCY, transparency);
				}
				await Task.Yield(); // Wait for the next frame
			}

			// Ensure the final value is set
			hologramShader.SetFloat(FRESNEL, targetValue);
		}
	}

	public struct WinEvent : IEvent { }
}