using UnityEditor;
using UnityEngine;

namespace GamePix.Editor
{
    public class UpdateWindow : EditorWindow
    {
        private static string downloadUrl = string.Empty;
        private static string windowTitle = "GamePix Update";
        private string warningLabel = "A new version of GamePix Unity3D plugin is available";
        private string removeWarningLabel = "*Please remove old version of GamePix Unity3D plugin before update";
        private GUIStyle bottomGroupStyle;
        private GUIStyle warningStyle;
        private GUIStyle removeWarningStyle;
        private GUIStyle downloadButtonStyle;
        private static Vector2 windowSize = new Vector2(300, 200);
        
        public static void Show(string url)
        {
            downloadUrl = url;
            var wnd = GetWindow(typeof(UpdateWindow), true, windowTitle, true);
            wnd.minSize = wnd.maxSize = windowSize;
        }
        
        void OnEnable ()
        {
            if (bottomGroupStyle == null)
            {
                bottomGroupStyle = new GUIStyle();
                bottomGroupStyle.alignment = TextAnchor.LowerLeft;
                
                warningStyle = new GUIStyle(EditorStyles.label);
                warningStyle.fontSize = 16;
                warningStyle.wordWrap = true;
                warningStyle.fontStyle = FontStyle.Bold;
                warningStyle.alignment = TextAnchor.MiddleCenter;
                
                removeWarningStyle = new GUIStyle(EditorStyles.label);
                removeWarningStyle.normal.textColor = Color.red;
                removeWarningStyle.fontSize = 11;
                removeWarningStyle.wordWrap = true;
                
                downloadButtonStyle = new GUIStyle(EditorStyles.miniButton);
                downloadButtonStyle.fontSize = 16;
                downloadButtonStyle.fixedHeight = 40;
            }
        }

        void OnGUI()
        {
            GUILayout.Space(25f);
            GUILayout.Label(warningLabel, warningStyle);
            GUILayout.Space(10f);
            GUILayout.Label(removeWarningLabel, removeWarningStyle);
            GUILayout.FlexibleSpace();
            using (var h =  new EditorGUILayout.VerticalScope(bottomGroupStyle))
            {
                if (GUILayout.Button("Download", downloadButtonStyle))
                {
                    Application.OpenURL(downloadUrl); 
                    GUIUtility.ExitGUI();
                }

                GUILayout.Space(10);
            }
        }
    }
}
