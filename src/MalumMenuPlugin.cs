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
public partial class MalumPlugin : BasePlugin
{
    public Harmony Harmony { get; } = new(Id);
    public static string malumVersion = "1.3.0";
    public static List<string> supportedAU = new List<string> { "2023.11.28" };
    private static MenuUI menuUI;
    public static ConfigEntry<string> menuKeybind;
    public static ConfigEntry<string> spoofFriendCode;
    public static ConfigEntry<string> spoofPuid;

    public override void Load()
    {

        menuKeybind = Config.Bind("MalumMenu",
                                "GUIKeybind",
                                "Delete",
                                "The keyboard key used to toggle the GUI on and off");

        spoofFriendCode = Config.Bind("MalumMenu.SpoofedUserData",
                                "SpoofedFriendCode",
                                "",
                                "Your spoofed friend code that will be used in online games. IMPORTANT: When using a spoofed friend code, players won't be able to send you friend requests");

        spoofPuid = Config.Bind("MalumMenu.SpoofedUserData",
                                "SpoofedPuid",
                                "",
                                "Your spoofed PUID that will be used in online games. IMPORTANT: Only valid, active PUIDs will let you connect to lobbies");

        
        Harmony.PatchAll();
        menuUI = AddComponent<MenuUI>();

        SceneManager.add_sceneLoaded((Action<Scene, LoadSceneMode>) ((scene, _) =>
        {
            if (scene.name == "MainMenu")
            {
                ModManager.Instance.ShowModStamp();

                //Warn about unsupported AU versions
                if (!supportedAU.Contains(Application.version)){
                    Utils.showPopup("\nThis version of MalumMenu and this version of Among Us are incompatible\n\nInstall the right version to avoid problems");
                }
            }
        }));
    }
}

