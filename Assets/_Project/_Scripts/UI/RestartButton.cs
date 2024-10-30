
using System;
using Extensions;
using MusicEngine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _Project._Scripts.UI {
	public class RestartButton : MonoBehaviour{
		void Update() {
			if (Keyboard.current.rKey.wasPressedThisFrame) {
				Restart();
			}
		}

		void Restart() {
			MusicManager.Instance.StopMusic();
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}
	}
}