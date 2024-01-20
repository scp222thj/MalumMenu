using AmongUs.Data.Player;
using HarmonyLib;

namespace MalumMenu;

[HarmonyPatch(typeof(EOSManager), nameof(EOSManager.ProductUserId), MethodType.Getter)]
public static class Passive_SpoofedPUIDPostfix
{
    public static void Postfix(ref string __result)
    {
        if (MalumPlugin.spoofPuid.Value != ""){
            __result = MalumPlugin.spoofPuid.Value;
        }
    }
}

[HarmonyPatch(typeof(EOSManager), nameof(EOSManager.Update))]
public static class Passive_SpoofedFriendCodePostfix
{
    public static string defaultFC = null;
    public static void Postfix(EOSManager __instance)
    {
        if (CheatSettings.incognitoMode){
            if (defaultFC == null){
                defaultFC = __instance.FriendCode;
            }
            string username = DestroyableSingleton<AccountManager>.Instance.GetRandomName().ToLower();
            string discriminator = new System.Random().Next(1000, 10000).ToString();
            __instance.FriendCode = username + "#" + discriminator;

            return;
        }

        if (MalumPlugin.spoofFriendCode.Value != "" && MalumPlugin.spoofFriendCode.Value != __instance.FriendCode){
            __instance.FriendCode = MalumPlugin.spoofFriendCode.Value;
            
            return;
        }

        if (defaultFC != null){
            __instance.FriendCode = defaultFC;
            defaultFC = null;
        }
    }
}

[HarmonyPatch(typeof(PlayerCustomizationData), nameof(PlayerCustomizationData.Name), MethodType.Getter)]
public static class Spoofing_RandomNamePostfix
{
    public static byte? defaultColor = null;
    public static string defaultPet = null;
    public static string defaultVisor = null;
    public static string defaultSkin = null;
    public static string defaultHat = null;
    public static void Postfix(PlayerCustomizationData __instance, ref string __result)
    {        
        if (CheatSettings.incognitoMode){
            if (defaultColor == null){
                defaultColor = __instance.Color;
                defaultPet = __instance.Pet;
                defaultVisor = __instance.Visor;
                defaultSkin = __instance.Skin;
                defaultHat = __instance.Hat;
            }
            __result = DestroyableSingleton<AccountManager>.Instance.GetRandomName();
            __instance.Color = (byte)new System.Random().Next(18);
            __instance.Pet = Utils.referenceDataManager.Refdata.pets[new System.Random().Next(Utils.referenceDataManager.Refdata.pets.Count)].ProdId;
            __instance.Visor = Utils.referenceDataManager.Refdata.visors[new System.Random().Next(Utils.referenceDataManager.Refdata.visors.Count)].ProdId;
            __instance.Skin = Utils.referenceDataManager.Refdata.skins[new System.Random().Next(Utils.referenceDataManager.Refdata.skins.Count)].ProdId;
            __instance.Hat = Utils.referenceDataManager.Refdata.hats[new System.Random().Next(Utils.referenceDataManager.Refdata.hats.Count)].ProdId;
            return;
        }

        if (defaultColor != null){
            __instance.Color = (byte)defaultColor;
            __instance.Pet = defaultPet;
            __instance.Visor = defaultVisor;
            __instance.Skin = defaultSkin;
            __instance.Hat = defaultHat;

            defaultColor = null;
            defaultPet = defaultVisor = defaultSkin = defaultHat = null;
        }
    }
}