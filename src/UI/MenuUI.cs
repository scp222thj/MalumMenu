using ImGuiNET;

namespace MalumMenu;
public class MenuUI
{
    private static bool _isMyUIOpen = true;
    public static void MyUI()
    {
        if (DearImGuiInjection.DearImGuiInjection.IsCursorVisible)
        {
            var dummy = true;
            ImGui.ShowDemoWindow(ref dummy);

            if (ImGui.BeginMainMenuBar())
            {
                if (ImGui.BeginMenu("MainBar", true))
                {
                    if (ImGui.MenuItem("MyTestPlugin"))
                    {
                        _isMyUIOpen ^= true;
                    }

                    ImGui.EndMenu();
                }

                ImGui.EndMainMenuBar();
            }

            if (ImGui.BeginMainMenuBar())
            {
                if (ImGui.BeginMenu("MainBar", true))
                {
                    if (ImGui.MenuItem("MyTestPlugin2"))
                    {
                        _isMyUIOpen ^= true;
                    }

                    ImGui.EndMenu();
                }

                ImGui.EndMainMenuBar();
            }
        }

        if (_isMyUIOpen)
        {
            var dummy2 = true;
            if (ImGui.Begin("test", ref dummy2, (int)ImGuiWindowFlags.None))
            {
                ImGui.Text("hello there");


                if (ImGui.Button("Click me"))
                {
                    // Interacting with the unity api must be done from the unity main thread
                    // Can just use the dispatcher shipped with the library for that
                    UnityMainThreadDispatcher.Enqueue(() =>
                    {
                        UnityEngine.Debug.Log("test");
                    });
                }
            }

            ImGui.End();
        }
    }

}
