using UnityEngine;

namespace MalumMenu;

public interface ITab
{
    string name { get; }
    Texture2D icon { get; }
    void Draw();
}
