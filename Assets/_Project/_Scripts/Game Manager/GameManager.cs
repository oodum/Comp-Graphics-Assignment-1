using System;
using Scoring;
using UnityEngine;
using Utility;

public class GameManager : Singleton<GameManager> {
	ScoreComputer scoreComputer;
	DodgeComputer dodgeComputer;

	void Start() {
		ServiceLocator.ServiceLocator.For(this)
			.Get(out scoreComputer)
			.Get(out dodgeComputer);
	}
	public ScoreResult GetScore(float timing) {
		ScoreResult scoreResult = scoreComputer.CalculateScore(timing);

		return scoreResult;
	}

	public DodgeResult GetDodge(float timing) {
		DodgeResult dodgeResult = dodgeComputer.CalculateDodge(timing);

		return dodgeResult;
	}

	public void Pause() {
		//noop
	}
}
