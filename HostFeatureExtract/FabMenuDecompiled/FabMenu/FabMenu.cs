using System;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace FabMenu;

[BepInProcess("Among Us.exe")]
[BepInPlugin("FabMenu", "FabMenu", "1.0.1+be8e9cb9a7d05b1e5a472410776bea1a51a93214")]
public class FabMenu : BasePlugin
{
	public static string malumVersion = "1.0.1";

	public static List<string> supportedAU = new List<string>(3) { "2025.9.9", "2025.10.14", "2025.11.18" };

	public static MenuUI menuUI;

	public static DoorsUI doorsUI;

	public static TasksUI tasksUI;

	public static ConfigEntry<string> menuKeybind;

	public static ConfigEntry<string> menuHtmlColor;

	public static ConfigEntry<bool> teleportMenuToMouse;

	public static ConfigEntry<bool> useHorizontalUI;

	public static ConfigEntry<bool> chatDarkMode;

	public static ConfigEntry<string> spoofLevel;

	public static ConfigEntry<string> spoofPlatform;

	public static ConfigEntry<bool> spoofDeviceId;

	public static ConfigEntry<string> guestFriendCode;

	public static ConfigEntry<bool> guestMode;

	public static ConfigEntry<bool> noTelemetry;

	public static ConfigEntry<bool> freeCosmetics;

	public static ConfigEntry<bool> avoidBans;

	public static ConfigEntry<bool> unlockFeatures;

	public const string Id = "FabMenu";

	public Harmony Harmony { get; } = new Harmony("FabMenu");

	public static string Name => "FabMenu";

	public static string Version => "1.0.1+be8e9cb9a7d05b1e5a472410776bea1a51a93214";

	public override void Load()
	{
		menuKeybind = ((BasePlugin)this).Config.Bind<string>("FabMenu.GUI", "Keybind", "Delete", "The keyboard key used to toggle the GUI on and off. List of supported keycodes: https://docs.unity3d.com/Packages/com.unity.tiny@0.16/api/Unity.Tiny.Input.KeyCode.html");
		menuHtmlColor = ((BasePlugin)this).Config.Bind<string>("FabMenu.GUI", "Color", "", "A custom color for your FabMenu GUI. Supports html color codes");
		teleportMenuToMouse = ((BasePlugin)this).Config.Bind<bool>("FabMenu.GUI", "TeleportMenuToMouse", true, "When enabled, the menu will always open at the current mouse position.");
		useHorizontalUI = ((BasePlugin)this).Config.Bind<bool>("FabMenu.GUI", "UseHorizontalUI", true, "When enabled, use the (new) horizontal tab-based UI instead of the vertical one.");
		chatDarkMode = ((BasePlugin)this).Config.Bind<bool>("FabMenu.Chat", "ChatDarkMode", true, "When enabled, in-game chat bubbles will use a dark theme by default.");
		guestMode = ((BasePlugin)this).Config.Bind<bool>("FabMenu.GuestMode", "GuestMode", false, "When enabled, a new guest account will generate every time you start the game, allowing you to bypass account bans and PUID detection");
		guestFriendCode = ((BasePlugin)this).Config.Bind<string>("FabMenu.GuestMode", "FriendName", "", "The username that will be used when setting a friend code for your guest account. IMPORTANT: Can only be used with GuestMode, needs to be ≤ 10 characters, and cannot include special characters/discriminator (#1234)");
		spoofLevel = ((BasePlugin)this).Config.Bind<string>("FabMenu.Spoofing", "Level", "", "A custom player level to display to others in online games to hide your actual platform. IMPORTANT: Custom levels can only be within 0 and 4294967295. Decimal numbers will not work");
		spoofPlatform = ((BasePlugin)this).Config.Bind<string>("FabMenu.Spoofing", "Platform", "", "A custom gaming platform to display to others in online lobbies to hide your actual platform. List of supported platforms: https://skeld.js.org/enums/_skeldjs_constant.Platform.html");
		spoofDeviceId = ((BasePlugin)this).Config.Bind<bool>("FabMenu.Privacy", "HideDeviceId", true, "When enabled it will hide your unique deviceId from Among Us, which could potentially help bypass hardware bans in the future");
		noTelemetry = ((BasePlugin)this).Config.Bind<bool>("FabMenu.Privacy", "NoTelemetry", true, "When enabled it will stop Among Us from collecting analytics of your games and sending them to Innersloth using Unity Analytics");
		CheatToggles.unlockFeatures = (CheatToggles.freeCosmetics = (CheatToggles.avoidBans = true));
		CheatToggles.chatDarkMode = chatDarkMode.Value;
		Harmony.PatchAll();
		menuUI = ((BasePlugin)this).AddComponent<MenuUI>();
		doorsUI = ((BasePlugin)this).AddComponent<DoorsUI>();
		tasksUI = ((BasePlugin)this).AddComponent<TasksUI>();
		((BasePlugin)this).AddComponent<CheatToggles.KeybindListener>().Plugin = this;
		if (noTelemetry.Value)
		{
			Analytics.enabled = false;
			Analytics.deviceStatsEnabled = false;
			PerformanceReporting.enabled = false;
		}
		SceneManager.sceneLoaded += UnityAction<Scene, LoadSceneMode>.op_Implicit((Action<Scene, LoadSceneMode>)delegate(Scene scene, LoadSceneMode _)
		{
			if (((Scene)(ref scene)).name == "MainMenu" && !supportedAU.Contains(Application.version))
			{
				((BasePlugin)this).Log.LogError((object)"This version of FabMenu and this version of Among Us are incompatible. Install the right version to avoid problems.");
			}
		});
	}
}
