using System;
using EventBus;
using MusicEngine;
using TMPro;
using UnityEngine;

public class MetronomeView : MonoBehaviour {
	[SerializeField] RectTransform smallHand;

	[SerializeField] float smallHandRotationSpeed;

	[SerializeField] RectTransform bigHand;
	[SerializeField] float leftAngle, rightAngle;
	MusicManager musicManager;

	[SerializeField] TextMeshProUGUI text;

	EventBinding<BeatEvent> beatEvent;

	void OnEnable() {
		beatEvent = new(UpdateText);
		EventBus<BeatEvent>.Register(beatEvent);
	}

	void OnDisable() { EventBus<BeatEvent>.Deregister(beatEvent); }
	void Start() { musicManager = MusicManager.Instance; }

	void Update() {
		UpdateSmallHandRotation();
		UpdateBigHandRotation();
	}

	void UpdateSmallHandRotation() { smallHand.Rotate(Vector3.forward, -smallHandRotationSpeed * Time.deltaTime); }

	void UpdateBigHandRotation() {
		if (double.IsNaN(musicManager.RelativeProgress)) return;
		float factor = Mathf.PingPong((float)musicManager.RelativeProgress, 1);
		Quaternion rotation = Quaternion.AngleAxis(Mathf.Lerp(leftAngle, rightAngle, factor), Vector3.forward).normalized;
		bigHand.localRotation = rotation;
	}

	void UpdateText(BeatEvent @event) { text.text = @event.Beat.ToString(); }
}