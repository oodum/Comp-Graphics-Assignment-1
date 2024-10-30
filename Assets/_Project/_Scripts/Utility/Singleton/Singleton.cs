using System;
using UnityEngine;
namespace Utility {
    public class Singleton<T> : MonoBehaviour where T : Component {
        static T instance;
        public static bool Exists => instance != null;

        public static T Instance {
            get {
                if (instance == null) {
	                // try to find the instance in the scene
                    instance = FindAnyObjectByType<T>();
                    if (instance == null) {
	                    // if still not found, create a new game object and add the component
                        var go = new GameObject(typeof(T).Name + " (Singleton)");
                        instance = go.AddComponent<T>();
                    }
                }
                return instance;
            }
        }

        /// <summary>
        /// If you need to use Awake, override it and call base.Awake()
        /// </summary>
        protected virtual void Awake() {
            if (!Application.isPlaying) return;
            instance = this as T;
        }

        /// <summary>
        /// If you need to use OnDestroy, override it and call base.OnDestroy()
        /// </summary>
        protected virtual void OnDestroy() {
            if (instance == this) {
                instance = null;
            }
        }
    }
}
