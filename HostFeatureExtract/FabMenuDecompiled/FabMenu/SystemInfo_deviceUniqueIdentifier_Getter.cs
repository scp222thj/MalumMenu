using System;
using System.Security.Cryptography;
using HarmonyLib;

namespace FabMenu;

[HarmonyPatch(/*Could not decode attribute arguments.*/)]
public static class SystemInfo_deviceUniqueIdentifier_Getter
{
	public static void Postfix(ref string __result)
	{
		if (FabMenu.spoofDeviceId.Value)
		{
			byte[] array = new byte[16];
			using (RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create())
			{
				randomNumberGenerator.GetBytes(array);
			}
			__result = BitConverter.ToString(array).Replace("-", "").ToLower();
		}
	}
}
