using HarmonyLib;
using UnityEngine;


namespace MalumMenu;

[HarmonyPatch(typeof(VersionShower), nameof(VersionShower.Start))]
	public static class VersionShowerPatch
	{
        //Postfix patch of VersionShower.Start to show MalumMenu version
		public static void Postfix(VersionShower __instance)
		{
            if (MalumPlugin.supportedAU.Contains(Application.version)){ // Checks if Among Us version is supported

                __instance.text.text =  $"MalumMenu v{MalumPlugin.malumVersion} (v{Application.version})"; // Supported
            
            }else{

                __instance.text.text =  $"MalumMenu v{MalumPlugin.malumVersion} (<color=red>v{Application.version}</color>)"; //Unsupported
            
            }
        }
    }

[HarmonyPatch(typeof(PingTracker), nameof(PingTracker.Update))]
	public static class PingTrackerPatch
	{
        //Postfix patch of VersionShower.Start to show game metrics
		public static void Postfix(PingTracker __instance)
		{
            __instance.text.alignment = TMPro.TextAlignmentOptions.TopRight;
            
            __instance.text.text = $"MalumMenu by scp222thj" + //ModInfo
                                    Utils.getColoredPingText(AmongUsClient.Instance.Ping); //Colored Ping

            //Position adjustments
            var offset_x = 1.2f;
            if (HudManager.InstanceExists && HudManager._instance.Chat.chatButton.active) offset_x += 0.8f;
            if (FriendsListManager.InstanceExists && FriendsListManager._instance.FriendsListButton.Button.active) offset_x += 0.8f;
            __instance.GetComponent<AspectPosition>().DistanceFromEdge = new Vector3(offset_x, 0f, 0f);
            
        }
    }

[HarmonyPatch(typeof(BackendEndpoints), nameof(BackendEndpoints.Announcements), MethodType.Getter)]
	public static class AnnouncementEndpointPatch
	{
        //Prefix patch of Getter method for BackendEndpoints.Announcements for custom announcements
		public static bool Prefix(StatsManager __instance, ref string __result)
        {
            __result = "https://scp222thj.dev/malumnews"; //MalumNews webserver
            return false;
        }
    }
