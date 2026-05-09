using UnityEngine;

namespace MalumMenu;

public class TrailRecorderController : MonoBehaviour
{
    private void Update()
    {
        if (!Utils.isInGame) return;
        if (!CheatToggles.mapTrails) return;
        MinimapHandler.RecordTrails();
    }
}

