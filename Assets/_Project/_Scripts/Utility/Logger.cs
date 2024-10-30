using UnityEngine;
namespace Utility {
    [CreateAssetMenu(fileName = "Logger", menuName = "Logger")]
    public class Logger : ScriptableObject {
        [SerializeField] string Name;
        [SerializeField] Color color = Color.white;
        public void Log(int value) {
            Debug.Log($"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{Name}: Value changed to {value}</color>");
        }
    }
}
