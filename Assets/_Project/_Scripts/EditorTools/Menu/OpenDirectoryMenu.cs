using UnityEditor;
using UnityEngine;
using System.IO;
using System.Diagnostics;
#if UNITY_EDITOR
namespace Editor {
    public class OpenDirectoryMenu {
        [MenuItem("Tools/Open Directory")]
		static void OpenDirectory() {
            var projectPath = Directory.GetCurrentDirectory();
            switch(Application.platform) {
                case RuntimePlatform.WindowsEditor:
                    OpenWindowsExplorer(projectPath);
                    break;
                case RuntimePlatform.OSXEditor:
                    OpenMacFinder(projectPath);
                    break;
                case RuntimePlatform.LinuxEditor:
                    OpenLinuxFileManager(projectPath);
                    break;
                default:
                    UnityEngine.Debug.LogError("Unsupported platform");
                    break;
            }
        }
        static void OpenWindowsExplorer(string projectPath) {
            Process.Start("explorer.exe", projectPath);
        }
        static void OpenMacFinder(string projectPath) {
            Process.Start("open", projectPath);
        }
        static void OpenLinuxFileManager(string projectPath) {
            UnityEngine.Debug.LogError("Doesn't work on linux :(");
        }
    }
}
#endif
