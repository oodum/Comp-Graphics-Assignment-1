using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class LUTChanger : MonoBehaviour {
	[SerializeField] Volume volume;
	[SerializeField] Texture defaultLUT, coolLUT, warmLUT;
	ColorLookup lookup;

	void Start() {
		volume.profile.TryGet(out lookup);
	}

	void Update() {
		if (Keyboard.current.bKey.wasPressedThisFrame) {
			lookup.texture.value = defaultLUT;
		}
		if (Keyboard.current.nKey.wasPressedThisFrame) {
			lookup.texture.value = coolLUT;
		}
		if (Keyboard.current.mKey.wasPressedThisFrame) {
			lookup.texture.value = warmLUT;
		}
	}
}
