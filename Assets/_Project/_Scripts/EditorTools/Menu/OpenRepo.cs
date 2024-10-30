using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
namespace Editor {
    public static class OpenRepo {
        [MenuItem("Tools/Open Github Repo")]
        static void OpenGithubRepo() {
            const string url = "https://github.com/ooodum/Incantation.git";
            Application.OpenURL(url);
        }
    }
}
#endif
