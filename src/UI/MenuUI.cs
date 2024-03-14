using ImGuiNET;
using UnityEngine;

namespace MalumMenu;
public class MenuUI
{
    public static bool _isMyUIOpen = true;
    public static void render()
    {
        if (_isMyUIOpen)
        {
            var dummy2 = true;
            if (ImGui.Begin("Test Window", ref dummy2, (int)ImGuiWindowFlags.None))
            {
                ImGui.Text("Hello, World!");


                if (ImGui.Button("Click me"))
                {
                    // Interacting with the unity api must be done from the unity main thread
                    // Can just use the dispatcher shipped with the library for that
                    UnityMainThreadDispatcher.Enqueue(() =>
                    {
                        Debug.Log("test");
                    });
                }
            }

            ImGui.End();
        }
    }
}