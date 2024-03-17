using UnityEngine;

public class TextField
{
    public string Text { get; set; } = "";
    private bool isFocused = false;
    private Rect textFieldRect;
    private float cursorBlinkTime = 0.5f;
    private float lastBlinkTime = 0.0f;
    private bool cursorVisible = true;

    public void draw()
    {
        GUILayout.Box("", GUILayout.Width(200), GUILayout.Height(20));
        
        if (Event.current.type == EventType.Repaint)
        {
            textFieldRect = GUILayoutUtility.GetLastRect();
        }

        handleTextInput();

        GUI.Label(new Rect(textFieldRect.x, textFieldRect.y, textFieldRect.width - 10, textFieldRect.height), Text);

        if (Event.current.type == EventType.MouseDown)
        {
            if (textFieldRect.Contains(Event.current.mousePosition))
            {
                isFocused = true;
                lastBlinkTime = Time.time;
                cursorVisible = true;
                Event.current.Use();
            }
            else
            {
                isFocused = false;
            }
        }

        if (isFocused)
        {
            manageCursor();
        }
    }

    private void handleTextInput()
    {
        if (isFocused && Event.current.type == EventType.KeyDown)
        {
            if (Event.current.keyCode == KeyCode.Backspace)
            {
                if (Text.Length > 0)
                {
                    Text = Text.Substring(0, Text.Length - 1);
                    Event.current.Use();
                }
            }
            else if (Event.current.character != '\0' && !char.IsControl(Event.current.character))
            {
                Text += Event.current.character;
                Event.current.Use();
            }
        }
    }

    private void manageCursor()
    {
        if (Time.time - lastBlinkTime > cursorBlinkTime)
        {
            cursorVisible = !cursorVisible;
            lastBlinkTime = Time.time;
        }

        if (cursorVisible)
        {
            Vector2 textSize = GUI.skin.label.CalcSize(new GUIContent(Text));
            GUI.Label(new Rect(textFieldRect.x + textSize.x + 2, textFieldRect.y + 2, 10, textFieldRect.height - 4), "|");
        }
    }
}