using UnityEngine;
using UnityEngine.Serialization;
namespace GameEntity{
	public class HealthComponent : MonoBehaviour {
		 public int Health;
		public void Heal(int amount) {
			Health += amount;
		}

		public void Damage(int amount) {
			Health -= amount;
		}
	}
}
