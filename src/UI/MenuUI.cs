using UnityEngine;

namespace MalumMenu;
public class MenuUI : MonoBehaviour
{
    void OnGUI() {
        if (GUILayout.Button("Press Me"))
            Debug.Log("Hello!");
    }

}
