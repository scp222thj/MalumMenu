﻿using BepInEx;
using BepInEx.Unity.IL2CPP;
using UnityEngine.SceneManagement;
using System;
using UnityEngine;
using UnityEngine.Analytics;
using System.Collections.Generic;
using BepInEx.Configuration;
using HarmonyLib;

namespace MalumMenu;

[BepInAutoPlugin]
[BepInProcess("Among Us.exe")]
public partial class MalumMenu : BasePlugin
{
    public Harmony Harmony { get; } = new(Id);
    public static string malumVersion = "2.2.0";
    public static List<string> supportedAU = new List<string> { "2023.11.28", "2024.3.5" };//宝宝们这个东西真的好用吗
    public static MenuUI menuUI;
    // public static ConsoleUI consoleUI;
    public static ConfigEntry<string> menuKeybind;
    public static ConfigEntry<string> menuHtmlColor;
    public static ConfigEntry<string> spoofLevel;
    public static ConfigEntry<string> spoofPlatform;
    public static ConfigEntry<bool> spoofDeviceId;
    public static ConfigEntry<string> guestFriendCode;
    public static ConfigEntry<bool> guestMode;
    public static ConfigEntry<bool> noTelemetry;

    public override void Load()
    {

        //Load config settings
        menuKeybind = Config.Bind("MalumMenu.GUI",
                                "Keybind",
                                "Delete",
                                "The keyboard key used to toggle the GUI on and off. List of supported keycodes: https://docs.unity3d.com/Packages/com.unity.tiny@0.16/api/Unity.Tiny.Input.KeyCode.html");

        menuHtmlColor = Config.Bind("MalumMenu.GUI",
                                "Color",
                                "",
                                "A custom color for your MalumMenu GUI. Supports html color codes");

        guestMode = Config.Bind("MalumMenu.GuestMode",
                                "GuestMode",
                                false,
                                "When enabled, a new guest account will generate every time you start the game, allowing you to bypass account bans and PUID detection");
        
        guestFriendCode = Config.Bind("MalumMenu.GuestMode",
                                "FriendName",
                                "",
                                "The username that will be used when setting a friend code for your guest account. IMPORTANT: Can only be used with GuestMode, needs to be ≤ 10 characters, and cannot include special characters/discriminator (#1234)");
        
        spoofLevel = Config.Bind("MalumMenu.Spoofing",
                                "Level",
                                "",
                                "A custom player level to display to others in online games to hide your actual platform. IMPORTANT: Custom levels can only be within 0 and 4294967295. Decimal numbers will not work");
        
        spoofPlatform = Config.Bind("MalumMenu.Spoofing",
                                "Platform",
                                "",
                                "A custom gaming platform to display to others in online lobbies to hide your actual platform. List of supported platforms: https://skeld.js.org/enums/constant.Platform.html");

        spoofDeviceId = Config.Bind("MalumMenu.Privacy",
                                "HideDeviceId",
                                true,
                                "When enabled it will hide your unique deviceId from Among Us, which could help bypass hardware bans in the future");

        noTelemetry = Config.Bind("MalumMenu.Privacy",
                                "NoTelemetry",
                                true,
                                "When enabled it will stop Among Us from collecting analytics of your games and sending them Innersloth using Unity Analytics");



        Harmony.PatchAll();
        
        menuUI = AddComponent<MenuUI>();
        // consoleUI = AddComponent<ConsoleUI>();

        if (noTelemetry.Value){

            Analytics.enabled = false;
            Analytics.deviceStatsEnabled = false;
            PerformanceReporting.enabled = false;

        }

        SceneManager.add_sceneLoaded((Action<Scene, LoadSceneMode>) ((scene, _) =>
        {
            if (scene.name == "MainMenu")
            {
                ModManager.Instance.ShowModStamp(); // byd树懒的byd要求
                
                //不是牢底你AU版本错了还弄呢（笑
                if (!supportedAU.Contains(Application.version)){
                    Utils.showPopup("\nAU版本不对啊 请你————滚！粗！砌！更！新！\n否则可能导致很多问题");
                }
                Utils.showPopup("\n用本MOD可能导致账号永久封禁！\n模组作者不承担任何后果！\n随时跑路！\n");
            }
        }));
    }
}

