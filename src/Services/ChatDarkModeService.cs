using System.Reflection;
using UnityEngine;

namespace MalumMenu;

public static class ChatDarkModeService
{
    private static ChatController _ownerController;

    public static readonly Color DarkBackground = new(0.1f, 0.1f, 0.12f, 1f);
    public static readonly Color DarkBubbleOther = new(0.18f, 0.18f, 0.22f, 1f);
    public static readonly Color DarkBubbleSelf = new(0.12f, 0.18f, 0.28f, 1f);
    public static readonly Color DarkInputField = new(0.14f, 0.14f, 0.16f, 1f);
    public static readonly Color DarkSubmitButton = new(0.22f, 0.22f, 0.26f, 1f);
    public static readonly Color DarkTextColor = new(0.9f, 0.9f, 0.9f, 1f);
    public static readonly Color DarkPlaceholderColor = new(0.5f, 0.5f, 0.5f, 1f);

    private static readonly FieldInfo _freeChatTextAreaField = typeof(FreeChatInputField)
        .GetField("textArea", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

    private static readonly FieldInfo _textBoxPlaceholderField = typeof(TextBoxTMP)
        .GetField("placeholderText", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

    private static readonly FieldInfo _chatBubblePlayerInfoField = typeof(ChatBubble)
        .GetField("playerInfo", BindingFlags.NonPublic | BindingFlags.Instance);

    public static bool Enabled => CheatToggles.chatDarkMode;

    public static void OnTogglePressed()
    {
        CheatToggles.chatDarkMode = !CheatToggles.chatDarkMode;

        if (CheatToggles.chatDarkMode)
        {
            ApplyAll();
        }
        else
        {
            RestoreAll();
        }
    }

    public static void SetOwner(ChatController controller)
    {
        _ownerController = controller;
    }

    public static void ApplyAll()
    {
        if (!Enabled) return;

        var controller = _ownerController != null ? _ownerController : GetChatController();
        if (controller == null) return;

        ApplyToChatPanel(controller);
        ApplyToBubbles(controller);
        ApplyToInputField(controller);
        ApplyTextBoxIfChatOwned(controller);
    }

    public static void RestoreAll()
    {
        var controller = _ownerController != null ? _ownerController : GetChatController();
        if (controller == null) return;

        try
        {
            var chatScreen = controller.chatScreen;
            if (chatScreen != null)
            {
                foreach (var sr in chatScreen.GetComponentsInChildren<SpriteRenderer>(true))
                {
                    sr.color = Color.white;
                }
                foreach (var img in chatScreen.GetComponentsInChildren<UnityEngine.UI.Image>(true))
                {
                    img.color = Color.white;
                }
            }
        }
        catch { }
    }

    public static void ApplyTextBoxIfChatOwned(ChatController controller = null)
    {
        if (!Enabled) return;

        controller = controller ?? _ownerController ?? GetChatController();
        if (controller == null) return;

        try
        {
            var freeChatField = controller.freeChatField;
            if (freeChatField == null) return;

            var textBox = _freeChatTextAreaField?.GetValue(freeChatField) as TextBoxTMP;
            if (textBox == null) return;

            var inputField = textBox.GetComponent<TMPro.TMP_InputField>();
            if (inputField != null)
            {
                var textArea = inputField.textViewport;
                if (textArea != null)
                {
                    var bgImg = textArea.GetComponent<UnityEngine.UI.Image>();
                    if (bgImg != null)
                    {
                        bgImg.color = DarkInputField;
                    }
                    else
                    {
                        var bgSr = textArea.GetComponent<SpriteRenderer>();
                        if (bgSr != null) bgSr.color = DarkInputField;
                    }
                }

                inputField.textComponent.color = DarkTextColor;

                var placeholder = _textBoxPlaceholderField?.GetValue(textBox) as TMPro.TextMeshPro;
                if (placeholder != null)
                {
                    placeholder.color = DarkPlaceholderColor;
                }
            }
        }
        catch { }
    }

    public static void ApplyToPanel(Component panel, bool isBubble = false, bool isSelf = false)
    {
        if (!Enabled || panel == null) return;

        try
        {
            var sr = panel.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.color = isBubble ? (isSelf ? DarkBubbleSelf : DarkBubbleOther) : DarkBackground;
            }

            var img = panel.GetComponent<UnityEngine.UI.Image>();
            if (img != null)
            {
                img.color = isBubble ? (isSelf ? DarkBubbleSelf : DarkBubbleOther) : DarkBackground;
            }
        }
        catch { }
    }

    public static void ApplyToText(TMPro.TextMeshPro text, bool forceColor = false)
    {
        if (!Enabled || text == null) return;
        if (forceColor || text.color != Color.white)
        {
            text.color = DarkTextColor;
        }
    }

    private static void ApplyToChatPanel(ChatController controller)
    {
        try
        {
            var chatScreen = controller.chatScreen;
            if (chatScreen == null) return;

            var sr = chatScreen.GetComponentInChildren<SpriteRenderer>();
            if (sr != null) sr.color = DarkBackground;

            var img = chatScreen.GetComponentInChildren<UnityEngine.UI.Image>();
            if (img != null) img.color = DarkBackground;
        }
        catch { }
    }

    private static void ApplyToBubbles(ChatController controller)
    {
        try
        {
            var scroller = controller.scroller;
            if (scroller == null || scroller.Inner == null) return;

            for (int i = 0; i < scroller.Inner.childCount; i++)
            {
                var child = scroller.Inner.GetChild(i);
                if (child == null) continue;

                var chatBubble = child.GetComponent<ChatBubble>();
                if (chatBubble == null || !chatBubble.gameObject.activeSelf) continue;

                var playerInfo = _chatBubblePlayerInfoField?.GetValue(chatBubble) as NetworkedPlayerInfo;
                bool isSelf = playerInfo != null && PlayerControl.LocalPlayer != null
                    && playerInfo.PlayerId == PlayerControl.LocalPlayer.PlayerId;
                ApplyToPanel(chatBubble.Background, true, isSelf);
            }
        }
        catch { }
    }

    private static void ApplyToInputField(ChatController controller)
    {
        try
        {
            var freeChatField = controller.freeChatField;
            if (freeChatField == null) return;

            var submitButton = freeChatField.gameObject.GetComponentInChildren<ChatInputFieldButton>();
            if (submitButton != null)
            {
                var btnImg = submitButton.GetComponent<UnityEngine.UI.Image>();
                if (btnImg != null)
                {
                    btnImg.color = DarkSubmitButton;
                }
                else
                {
                    var btnSr = submitButton.GetComponent<SpriteRenderer>();
                    if (btnSr != null) btnSr.color = DarkSubmitButton;
                }
            }

            var textBox = _freeChatTextAreaField?.GetValue(freeChatField) as TextBoxTMP;
            if (textBox != null)
            {
                ApplyTextBoxIfChatOwned(controller);
            }
        }
        catch { }
    }

    private static ChatController GetChatController()
    {
        if (HudManager.InstanceExists && HudManager.Instance.Chat != null)
        {
            return HudManager.Instance.Chat;
        }
        return null;
    }
}
