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
    public static ConfigEntry<string> customFriendCode;
    public static ConfigEntry<string> customPuid;

    public override void Load()
    {

        menuKeybind = Config.Bind("MalumMenu",   // The section under which the option is shown
                                "GUIKeybind",  // The key of the configuration option in the configuration file
                                "Delete", // The default value
                                "The keyboard key used to toggle the GUI on and off"); // Description of the option to show in the config file

        customFriendCode = Config.Bind("MalumMenu.Cheats",
                                "CustomFriendCode",
                                "",
                                "The custom friend code that will be used in online games");

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

