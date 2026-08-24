using UnityEngine;

namespace MalumMenu;

public class AnimationsTab : ITab
{
    public string name => "Animations";

    public void Draw()
    {
        GUILayout.BeginVertical(GUILayout.Width(MenuUI.windowWidth * 0.425f));

        DrawGeneral();

        GUILayout.Space(15);

        DrawSkipAnimations();

        GUILayout.Space(15);

        DrawClientSided();

        GUILayout.EndVertical();
    }

    private void DrawGeneral()
    {
        CheatToggles.animShields = GUILayout.Toggle(CheatToggles.animShields, " Shields");

        CheatToggles.animAsteroids = GUILayout.Toggle(CheatToggles.animAsteroids, " Asteroids");

        CheatToggles.animEmptyGarbage = GUILayout.Toggle(CheatToggles.animEmptyGarbage, " Empty Garbage");

        CheatToggles.animMedScan = GUILayout.Toggle(CheatToggles.animMedScan, " Medbay Scan");

        CheatToggles.animCamsInUse = GUILayout.Toggle(CheatToggles.animCamsInUse, " Cams In Use");

        // CheatToggles.animPet = GUILayout.Toggle(CheatToggles.animPet, " Pet");
    }

   private void DrawSkipAnimations()
 {
     GUILayout.Label("Skip Animations", GUIStylePreset.TabSubtitle);
     CheatToggles.SkipSeekerAnimation = GUILayout.Toggle(CheatToggles.SkipSeekerAnimation, " Skip Seeker Animations");

     CheatToggles.NoShhScreenAnimation = GUILayout.Toggle(CheatToggles.NoShhScreenAnimation, " Skip Shhh Screen");

     CheatToggles.SkipKillAnimation = GUILayout.Toggle(CheatToggles.SkipKillAnimation, " Skip Kill Animations");

 }

    private void DrawClientSided()
    {
        GUILayout.Label("Client-Sided", GUIStylePreset.TabSubtitle);

        CheatToggles.moonWalk = GUILayout.Toggle(CheatToggles.moonWalk, " Moonwalk");
    }
}
