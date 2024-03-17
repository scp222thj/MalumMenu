using UnityEngine;
using System.Collections.Generic;

namespace MalumMenu
{
    public class MenuUI : MonoBehaviour
    {
        public bool isVisible = false;
        private const float MinWindowWidth = 500;
        private const float MinWindowHeight = 400;
        private Rect windowRect = new Rect(10, 10, MinWindowWidth, MinWindowHeight);

        public int selectedTab = 0;
        public List<ITab> tabs;

        void Start()
        {
            tabs = new List<ITab>
            {
                new TextTab(),
                new PlayerListTab(),
            };
        }

        void Update()
        {
            if (Input.GetKeyDown(Utils.stringToKeycode(MalumMenu.menuKeybind.Value)))
            {
                isVisible = !isVisible;

                if (isVisible)
                {
                    Vector2 mousePosition = Input.mousePosition;
                    windowRect.position = new Vector2(mousePosition.x, Screen.height - mousePosition.y);
                }
            }

            if(Input.GetMouseButtonDown(0))
            {
                if(!windowRect.Contains(Input.mousePosition))
                {
                    TextInput.clearFocusedTextInput();
                }
            }

        }

        void OnGUI()
        {
            if (!isVisible) return;

            string configHtmlColor = MalumMenu.menuHtmlColor.Value;
            if (!configHtmlColor.StartsWith("#"))
            {
                configHtmlColor = "#" + configHtmlColor;
            }

            if (ColorUtility.TryParseHtmlString(configHtmlColor, out Color uiColor))
            {
                GUI.backgroundColor = uiColor;
            }

            windowRect = GUILayout.Window(0, windowRect, (GUI.WindowFunction)WindowFunction, "MalumMenu v" + MalumMenu.malumVersion);
        }

        void WindowFunction(int windowID)
        {
            GUILayout.BeginHorizontal();
            for (int i = 0; i < tabs.Count; i++)
            {
                if (tabs[i].isVisible())
                {
                    if (GUILayout.Button(tabs[i].title))
                    {
                        selectedTab = i;
                    }
                }
            }
            GUILayout.EndHorizontal();

            if (tabs[selectedTab].isVisible())
            {
                tabs[selectedTab].drawContent();
            }

            GUI.DragWindow(new Rect(0, 0, 10000, 20));

            // Apply minimum size constraints after window content has been drawn
            if (Event.current.type == EventType.Repaint)
            {
                windowRect.width = Mathf.Max(windowRect.width, MinWindowWidth);
                windowRect.height = Mathf.Max(windowRect.height, MinWindowHeight);
            }
        }
    }
}
