using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
namespace Editor {
    public static class SystemConsoleMenu {
        [MenuItem("Tools/Open Terminal")]
        static void OpenConsoleInCurrentDirectory() {
            var projectPath = Directory.GetCurrentDirectory();

            switch(Application.platform) {
                case RuntimePlatform.WindowsEditor:
                    OpenWindowsTerminal(projectPath);
                    break;
                case RuntimePlatform.OSXEditor:
                    OpenMacTerminal(projectPath);
                    break;
                case RuntimePlatform.LinuxEditor:
                    OpenLinuxTerminal(projectPath);
                    break;
                default:
                    UnityEngine.Debug.LogError("Unsupported platform");
                    break;
            }
        }
        static void OpenWindowsTerminal(string projectPath) {

            const string gitBashPath = @"C:\Program Files\Git\git-bash.exe";
            // Try to open Git Bash if it exists
            if (File.Exists(gitBashPath)) {
                Process.Start(new ProcessStartInfo {
                    FileName = gitBashPath,
                    Arguments = $"--cd=\"{projectPath}\"",
                    UseShellExecute = false,
                });
            } else {
                // Fall back to CMD otherwise
                Process.Start("cmd.exe", $"/K cd /d \"{projectPath}\"");
            }
        }
        static void OpenMacTerminal(string projectPath) {
            Process.Start("open", $"-a Terminal \"{projectPath}\"");
        }
        static void OpenLinuxTerminal(string projectPath) {
            UnityEngine.Debug.LogError("Doesn't work on linux :(");
        }
    }
}
#endif
