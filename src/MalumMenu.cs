using BepInEx;
using BepInEx.Unity.IL2CPP;
using UnityEngine.SceneManagement;
using System;
using UnityEngine;
using System.Collections.Generic;
using BepInEx.Configuration;
using HarmonyLib;

namespace MalumMenu;

[BepInAutoPlugin]
[BepInProcess("Among Us.exe")]
public partial class MalumMenu : BasePlugin
{
    public Harmony Harmony { get; } = new(Id);
    public static string malumVersion = "2.1.0";
    public static List<string> supportedAU = new List<string> { "2023.11.28" };
    private static MenuUI menuUI;
    public static ConfigEntry<string> menuKeybind;
    public static ConfigEntry<string> spoofFriendCode;
    public static ConfigEntry<string> spoofLevel;

    public override void Load()
    {

        // Load config settings
        menuKeybind = Config.Bind("MalumMenu",
                                "GUIKeybind",
                                "Delete",
                                "The keyboard key used to toggle the GUI on and off");

        spoofFriendCode = Config.Bind("MalumMenu.Spoofing",
                                "FriendCode",
                                "",
                                "Your spoofed friend code that will be used in online games. IMPORTANT: When using a spoofed friend code, players won't be able to send you friend requests");

        spoofLevel = Config.Bind("MalumMenu.Spoofing",
                                "Level",
                                "",
                                "Your spoofed level that will be used in online games. IMPORTANT: Custom levels can only be within 0 and 4294967295. Decimal numbers will not work");

        Harmony.PatchAll();
        menuUI = AddComponent<MenuUI>();

        SceneManager.add_sceneLoaded((Action<Scene, LoadSceneMode>)((scene, _) =>
        {
            if (scene.name == "MainMenu")
            {
                ModManager.Instance.ShowModStamp(); // Required by InnerSloth Modding Policy. Also helps debug players

                // Warn about unsupported AU versions
                if (!supportedAU.Contains(Application.version))
                {
                    Utils.showPopup("\nThis version of MalumMenu and this version of Among Us are incompatible\n\nInstall the right version to avoid problems");
                }
            }
        }));
    }
}

