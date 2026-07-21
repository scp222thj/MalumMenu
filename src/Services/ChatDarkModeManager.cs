using System;
using UnityEngine;
using Il2CppInterop.Runtime.Injection;

namespace MalumMenu;

public class ChatDarkModeManager : MonoBehaviour
{
    private ChatController _chatController;
    private bool _toggleVisible;
    private Rect _toggleRect = new(0, 0, 80, 24);
    private bool _initialized;

    public ChatDarkModeManager(IntPtr ptr) : base(ptr) { }

    public void Init(ChatController controller)
    {
        _chatController = controller;
        _initialized = false;
    }

    public void Update()
    {
        if (MalumMenu.isPanicked) return;
        if (_chatController == null) return;

        bool chatVisible = _chatController.IsOpenOrOpening;

        if (chatVisible && !_initialized)
        {
            ChatDarkModeService.SetOwner(_chatController);
            _initialized = true;
        }

        if (!chatVisible)
        {
            _toggleVisible = false;
            _initialized = false;
            return;
        }

        _toggleVisible = true;

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.y = Screen.height - mousePos.y;

            if (_toggleRect.Contains(mousePos))
            {
                ChatDarkModeService.OnTogglePressed();
            }
        }
    }

    public void OnGUI()
    {
        if (!_toggleVisible || !CheatToggles.enableChat) return;
        if (_chatController == null || !_chatController.IsOpenOrOpening) return;

        float x = Screen.width - 100f;
        float y = Screen.height - 280f;
        _toggleRect = new Rect(x, y, 80, 24);

        var prevColor = GUI.backgroundColor;
        GUI.backgroundColor = ChatDarkModeService.Enabled
            ? new Color(0.2f, 0.6f, 0.2f, 0.85f)
            : new Color(0.3f, 0.3f, 0.3f, 0.85f);

        if (GUI.Button(_toggleRect, "Dark Mode"))
        {
            ChatDarkModeService.OnTogglePressed();
        }

        GUI.backgroundColor = prevColor;
    }

    public static ChatDarkModeManager Create(ChatController controller)
    {
        ClassInjector.RegisterTypeInIl2Cpp<ChatDarkModeManager>();
        var go = new GameObject("MalumMenu_ChatDarkModeManager");
        go.hideFlags = HideFlags.HideAndDontSave;
        var manager = go.AddComponent<ChatDarkModeManager>();
        manager.Init(controller);
        return manager;
    }
}
