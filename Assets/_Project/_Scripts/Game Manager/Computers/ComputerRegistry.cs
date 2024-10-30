using System;
using IncantationSystem;
using IncantationSystem.Verification;
using Scoring;
using UnityEngine;
namespace Managers {
	public class ComputerRegistry : MonoBehaviour {
		TimingComputer timingComputer;
		void Awake() {
			ServiceLocator.ServiceLocator.Global.Register(timingComputer = new());
			ServiceLocator.ServiceLocator.Global.Register(new ScoreComputer(timingComputer));
			ServiceLocator.ServiceLocator.Global.Register(new DodgeComputer(timingComputer));
			ServiceLocator.ServiceLocator.Global.Register(new ConfidenceComputer(timingComputer));
			ServiceLocator.ServiceLocator.Global.Register(typeof(IIncantationVerifier), new IncantationVerifier());
		}
	}
}
