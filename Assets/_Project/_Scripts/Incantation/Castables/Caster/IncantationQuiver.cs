using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
namespace IncantationSystem.Castables {
	public class IncantationQuiver : SerializedMonoBehaviour {
		[OdinSerialize] List<IncantationConfig> configs;
		[HideInInspector] public List<CastableIncantation> Incantations;

		void Awake() {
			if (configs == null || configs.Count == 0) {
				Debug.LogError("IncantationQuiver has no incantations");
				return;
			}
			Incantations = new();
			foreach (IncantationConfig config in configs) {
				Incantations.Add(new(config.Build()));
			}
		}
	}
}
