using UnityEngine;

namespace MalumMenu;

public class TrollTab : ITab
{
    public string name => "Troll";

    public void Draw()
    {
        GUILayout.BeginHorizontal();

        GUILayout.BeginVertical(GUILayout.Width(MenuUI.windowWidth * 0.425f));

        DrawFrame();

        GUILayout.Space(15);

        DrawTeleport();

        GUILayout.Space(15);

        DrawDisguise();

        GUILayout.EndVertical();

        GUILayout.BeginVertical();

        DrawFreeze();

        GUILayout.EndVertical();

        GUILayout.EndHorizontal();
    }

    private void DrawFrame()
    {
        GUILayout.Label("Frame Players", GUIStylePreset.TabSubtitle);

        CheatToggles.frameAsShapeshifter = GUILayout.Toggle(CheatToggles.frameAsShapeshifter, " Frame as Shapeshifter (Host Only)");
        GUILayout.Label("<size=10>Pick victim → pick disguise target\nOthers see victim shapeshift (looks like hacking)</size>");

        GUILayout.Space(5);

        CheatToggles.fakeVentOnPlayer = GUILayout.Toggle(CheatToggles.fakeVentOnPlayer, " Fake Vent on Player (Host Only)");
        GUILayout.Label("<size=10>Others see the player enter a vent</size>");
    }

    private void DrawTeleport()
    {
        GUILayout.Label("Force Teleport", GUIStylePreset.TabSubtitle);

        CheatToggles.teleportPlayerToPlayer = GUILayout.Toggle(CheatToggles.teleportPlayerToPlayer, " Teleport Player to Player (Host Only)");
        GUILayout.Label("<size=10>Pick player A → pick player B\nMoves A to B's position</size>");
    }

    private void DrawDisguise()
    {
        GUILayout.Label("Disguise", GUIStylePreset.TabSubtitle);

        CheatToggles.fakeShapeshift = GUILayout.Toggle(CheatToggles.fakeShapeshift, " Disguise as Another Player");
        GUILayout.Label("<size=10>You appear to be someone else on others' screens</size>");
    }

    private void DrawFreeze()
    {
        GUILayout.Label("Control", GUIStylePreset.TabSubtitle);

        CheatToggles.freezePlayer = GUILayout.Toggle(CheatToggles.freezePlayer, " Freeze Player (Host Only)");
        GUILayout.Label("<size=10>Continuously snap a player back to one spot</size>");
    }
}
