using UnityEditor;
using UnityEngine;

namespace GamePix.Editor
{
    public class BuildWindow
    {
        private static string buildDirectory = "html5";

        [MenuItem("GamePix/Build and Run", false, -1)]
        static void Build()
        {
            if (Emscripten.editorVersion != Emscripten.pluginVersion)
            {
                Debug.LogError("This Gamepix plugin version [emscripten: " + Emscripten.pluginVersion +
                    "] cannot be used for Unity " + ApplicationInfo.unityVersion +
                    " [emscripten: " + Emscripten.editorVersion +
                    "]. Check Gamepix plugin for Unity: " + Updater.defaultUrl);
                return;
            }
            Updater.TryUpdatePlugin(() => 
            {
                UnityConfigurator.SetWebGLSettings(true);
                Builder.BuildArchive(buildDirectory);
            });
        }

        [MenuItem("GamePix/Safe builds/Build and Run (Default resources)", false, 1)]
        static void DefaultResourceBuild()
        {
            if (Emscripten.editorVersion != Emscripten.pluginVersion)
            {
                Debug.LogError("This Gamepix plugin version [emscripten: " + Emscripten.pluginVersion +
                    "] cannot be used for Unity " + ApplicationInfo.unityVersion +
                    " [emscripten: " + Emscripten.editorVersion +
                    "]. Check Gamepix plugin for Unity: " + Updater.defaultUrl);
                return;
            }
            Updater.TryUpdatePlugin(() =>
            {
                UnityConfigurator.SetWebGLSettings(true, false);
                Builder.BuildArchive(buildDirectory, false);
            });
        }

        [MenuItem("GamePix/Check update", false, 2)]
        static void CheckUpdate()
        {
            Updater.TryUpdatePlugin(() =>
            {
                Debug.Log("The latest GamePix Unity3D plugin version is used");
            });
        }
    }
}