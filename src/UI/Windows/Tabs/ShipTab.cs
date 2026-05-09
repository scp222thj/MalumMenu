using UnityEngine;

namespace MalumMenu;

public class ShipTab : ITab
{
    public string name => "Ship";

    public void Draw()
    {
        GUILayout.BeginHorizontal();

        GUILayout.BeginVertical(GUILayout.Width(MenuUI.windowWidth * 0.425f));

        DrawGeneral();

        GUILayout.Space(15);

        DrawSabotage();

        GUILayout.EndVertical();

        GUILayout.BeginVertical();

        DrawVents();

        GUILayout.EndVertical();

        GUILayout.EndHorizontal();
    }

    private void DrawGeneral()
    {
        CheatToggles.unfixableLights = GUILayout.Toggle(CheatToggles.unfixableLights, " Unfixable Lights");

        // CheatToggles.reportBody = GUILayout.Toggle(CheatToggles.reportBody, " Report Body");

        CheatToggles.callMeeting = GUILayout.Toggle(CheatToggles.callMeeting, " Call Meeting");

        CheatToggles.closeMeeting = GUILayout.Toggle(CheatToggles.closeMeeting, " Close Meeting");

        CheatToggles.autoOpenDoorsOnUse = GUILayout.Toggle(CheatToggles.autoOpenDoorsOnUse, " Auto-Open Doors On Use");

        if (CheatToggles.autoOpenDoorsOnUse)
        {
            var seconds = MalumMenu.autoDoorOpenDelaySeconds.Value;
            GUILayout.Label($" Door Auto-Open Delay: {seconds:0.00}s");
            var newSeconds = GUILayout.HorizontalSlider(seconds, 0.1f, 4f);
            if (Mathf.Abs(newSeconds - seconds) > 0.0001f)
            {
                MalumMenu.autoDoorOpenDelaySeconds.Value = newSeconds;
                MalumMenu.Plugin.Config.Save();
            }
        }
    }

    private void DrawSabotage()
    {
        GUILayout.Label("Sabotage", GUIStylePreset.TabSubtitle);

        CheatToggles.reactorSab = GUILayout.Toggle(CheatToggles.reactorSab, " Reactor");

        CheatToggles.oxygenSab = GUILayout.Toggle(CheatToggles.oxygenSab, " Oxygen");

        CheatToggles.elecSab = GUILayout.Toggle(CheatToggles.elecSab, " Lights");

        CheatToggles.commsSab = GUILayout.Toggle(CheatToggles.commsSab, " Comms");

        CheatToggles.showDoorsMenu = GUILayout.Toggle(CheatToggles.showDoorsMenu, " Show Doors Menu");

        CheatToggles.mushSab = GUILayout.Toggle(CheatToggles.mushSab, " Mushroom Mixup");

        CheatToggles.mushSpore = GUILayout.Toggle(CheatToggles.mushSpore, " Trigger Spores");

        CheatToggles.sabotageMap = GUILayout.Toggle(CheatToggles.sabotageMap, " Open Sabotage Map");

        GUILayout.Space(8);

        CheatToggles.noSabotageCooldown = GUILayout.Toggle(CheatToggles.noSabotageCooldown, " No Sabotage Cooldown");
        var sabReduction = MalumMenu.sabotageCooldownReductionPercent.Value;
        GUILayout.Label($" Sabotage Cooldown Reduction: {sabReduction:0}%");
        var newSabReduction = GUILayout.HorizontalSlider(sabReduction, 0f, 100f);
        if (Mathf.Abs(newSabReduction - sabReduction) > 0.001f)
        {
            MalumMenu.sabotageCooldownReductionPercent.Value = newSabReduction;
            MalumMenu.Plugin.Config.Save();
        }

        CheatToggles.noDoorCooldown = GUILayout.Toggle(CheatToggles.noDoorCooldown, " No Door Cooldown");
        var doorReduction = MalumMenu.doorCooldownReductionPercent.Value;
        GUILayout.Label($" Door Cooldown Reduction: {doorReduction:0}%");
        var newDoorReduction = GUILayout.HorizontalSlider(doorReduction, 0f, 100f);
        if (Mathf.Abs(newDoorReduction - doorReduction) > 0.001f)
        {
            MalumMenu.doorCooldownReductionPercent.Value = newDoorReduction;
            MalumMenu.Plugin.Config.Save();
        }
    }

    private void DrawVents()
    {
        GUILayout.Label("Vents", GUIStylePreset.TabSubtitle);

        CheatToggles.unlockVents = GUILayout.Toggle(CheatToggles.unlockVents, " Unlock Vents");

        CheatToggles.kickVents = GUILayout.Toggle(CheatToggles.kickVents, " Kick All From Vents");

        CheatToggles.walkInVents = GUILayout.Toggle(CheatToggles.walkInVents, " Walk In Vents");
    }
}
