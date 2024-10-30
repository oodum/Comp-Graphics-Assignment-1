using System;
using Combat;
using UnityEngine;
using UnityEngine.UI;

public class TurnView : MonoBehaviour {
	Image image;
	[SerializeField] Sprite attackSprite, dodgeSprite;
	CombatManager combatManager;

	void Awake() {
		image = GetComponent<Image>();
	}

	void Start() {
		ServiceLocator.ServiceLocator.Global.Get(out combatManager);
	}
	
	void Update() {
		image.sprite = combatManager.CurrentCombatState == CombatManager.CombatState.Good ? attackSprite : dodgeSprite;
	}
}
