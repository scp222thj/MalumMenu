using UnityEngine;
using System;

namespace MalumMenu;

public class MovementTab : ITab
{
    public string name => "Movement";

    public void Draw()
    {
        GUILayout.BeginVertical(GUILayout.Width(MenuUI.windowWidth * 0.425f));

        DrawGeneral();

        GUILayout.Space(15);

        DrawTeleport();

        GUILayout.Space(15);

        DrawExtras();

        GUILayout.EndVertical();
    }

    private void DrawGeneral()
    {
        CheatToggles.noClip = GUILayout.Toggle(CheatToggles.noClip, " NoClip");

        CheatToggles.invertControls = GUILayout.Toggle(CheatToggles.invertControls, " Invert Controls");

        try
        {
            if (PlayerControl.LocalPlayer.Data.IsDead)
            {
                PlayerControl.LocalPlayer.MyPhysics.GhostSpeed = GUILayout.HorizontalSlider(PlayerControl.LocalPlayer.MyPhysics.GhostSpeed, 0f, 20f, GUILayout.Width(250f));
                Utils.SnapSpeedToDefault(0.05f, true);
                GUILayout.Label($"Current Speed: {PlayerControl.LocalPlayer?.MyPhysics.GhostSpeed} {(Utils.IsSpeedDefault(true) ? "(Default)" : "")}");
            }
            else
            {
                PlayerControl.LocalPlayer.MyPhysics.Speed = GUILayout.HorizontalSlider(PlayerControl.LocalPlayer.MyPhysics.Speed, 0f, 20f, GUILayout.Width(250f));
                Utils.SnapSpeedToDefault(0.05f);
                GUILayout.Label($"Current Speed: {PlayerControl.LocalPlayer?.MyPhysics.Speed} {(Utils.IsSpeedDefault() ? "(Default)" : "")}");
            }
        } catch (NullReferenceException) {}
    }

    private void DrawTeleport()
    {
        GUILayout.Label("Teleport", GUIStylePreset.TabSubtitle);

        CheatToggles.teleportCursor = GUILayout.Toggle(CheatToggles.teleportCursor, " to Cursor (RMB)");

        CheatToggles.teleportPlayer = GUILayout.Toggle(CheatToggles.teleportPlayer, " to Player");

        CheatToggles.blink = GUILayout.Toggle(CheatToggles.blink, " Blink to Cursor (MMB)");
    }

    private void DrawExtras()
    {
        GUILayout.Label("Extras", GUIStylePreset.TabSubtitle);

        CheatToggles.visionBoost      = GUILayout.Toggle(CheatToggles.visionBoost,      " Vision Boost");

        CheatToggles.showMeetingTimer = GUILayout.Toggle(CheatToggles.showMeetingTimer, " Meeting Timer");
    }
}
