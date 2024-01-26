using UnityEngine;
using Il2CppSystem.Collections.Generic;

namespace MalumMenu
{
    public class ConsoleUI : MonoBehaviour
    {
        public bool isVisible = false;
        private Vector2 scrollPosition = Vector2.zero;
        private List<string> logEntries = new List<string>();
        private Rect windowRect = new Rect(320, 10, 500, 300); // Adjust size and position as needed
        private GUIStyle logStyle;

        public void Log(string message)
        {
            if (logEntries.Count >= 100) // Limit the number of logs to keep memory usage in check
            {
                logEntries.RemoveAt(0); // Remove the oldest log entry
            }

            logEntries.Add(message);

            // Scroll to the bottom
            scrollPosition.y = float.MaxValue;
        }

        private void OnGUI()
        {
            
            if (!isVisible) return;

            if (logStyle == null){

                logStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 20
                };

            }

            Color configUIColor;
        
            if(ColorUtility.TryParseHtmlString(MalumMenu.menuHtmlColor.Value, out configUIColor)){
                GUI.backgroundColor = configUIColor;
            }

            windowRect = GUI.Window(1, windowRect, (GUI.WindowFunction)ConsoleWindow, "MalumConsole");
        }

        private void ConsoleWindow(int windowID)
        {
            GUILayout.BeginVertical();
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true);

            foreach (var log in logEntries)
            {
                GUILayout.Label(log, logStyle); // Use the custom GUIStyle with the specified font size
            }

            GUILayout.EndScrollView();
            GUILayout.EndVertical();

            GUI.DragWindow();
        }

    }
}
