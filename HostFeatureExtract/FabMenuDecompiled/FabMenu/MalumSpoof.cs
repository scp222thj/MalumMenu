using AmongUs.Data;

namespace FabMenu;

public static class MalumSpoof
{
	public static uint parsedLevel;

	public static void spoofLevel()
	{
		if (!string.IsNullOrEmpty(FabMenu.spoofLevel.Value) && uint.TryParse(FabMenu.spoofLevel.Value, out parsedLevel) && parsedLevel != DataManager.Player.Stats.Level)
		{
			DataManager.Player.stats.level = parsedLevel - 1;
			((AbstractSaveData)DataManager.Player).Save();
		}
	}

	public static string spoofFriendCode()
	{
		string text = FabMenu.guestFriendCode.Value;
		if (string.IsNullOrWhiteSpace(text))
		{
			text = DestroyableSingleton<AccountManager>.Instance.GetRandomName();
		}
		return text;
	}

	public static void spoofPlatform(PlatformSpecificData platformSpecificData)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		if (Utils.stringToPlatformType(FabMenu.spoofPlatform.Value, out var platform))
		{
			platformSpecificData.Platform = platform.Value;
		}
	}
}
