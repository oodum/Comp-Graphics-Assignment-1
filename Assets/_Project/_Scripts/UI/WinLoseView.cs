using System;
using EventBus;
using Extensions;
using GameEntity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project._Scripts.UI {
	public class WinLoseView : MonoBehaviour {
		[SerializeField] Color winColor, loseColor;
		[SerializeField] TextMeshProUGUI text;
		CanvasGroup canvasGroup;
		Image screen;

		EventBinding<WinEvent> winEvent;
		EventBinding<LoseEvent> loseEvent;


		void Awake() {
			canvasGroup = gameObject.GetOrAdd<CanvasGroup>();
			screen = gameObject.GetOrAdd<Image>();
		}
		void Start() { canvasGroup.alpha = 0; }

		void OnEnable() {
			winEvent = new(Win);
			loseEvent = new(Lose);
			EventBus<WinEvent>.Register(winEvent);
			EventBus<LoseEvent>.Register(loseEvent);
		}

		void OnDisable() {
			EventBus<WinEvent>.Deregister(winEvent);
			EventBus<LoseEvent>.Deregister(loseEvent);
		}

		void Win() {
			canvasGroup.alpha = 1;
			screen.color = winColor;
			text.text = "You Win!";
		}

		void Lose() {
			canvasGroup.alpha = 1;
			screen.color = loseColor;
			text.text = "You Lose!";
		}
	}
}