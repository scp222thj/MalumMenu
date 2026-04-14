using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSystem.Collections.Generic;

namespace FabMenu;

public static class CosmeticsUnlocker
{
	public static void unlockCosmetics(HatManager hatManager)
	{
		if (!CheatToggles.freeCosmetics)
		{
			return;
		}
		Enumerator<BundleData> enumerator = hatManager.allBundles.GetEnumerator();
		while (enumerator.MoveNext())
		{
			enumerator.Current.Free = true;
		}
		enumerator = hatManager.allFeaturedBundles.GetEnumerator();
		while (enumerator.MoveNext())
		{
			enumerator.Current.Free = true;
		}
		Enumerator<CosmicubeData> enumerator2 = hatManager.allFeaturedCubes.GetEnumerator();
		while (enumerator2.MoveNext())
		{
			enumerator2.Current.Free = true;
		}
		Enumerator<CosmeticData> enumerator3 = hatManager.allFeaturedItems.GetEnumerator();
		while (enumerator3.MoveNext())
		{
			enumerator3.Current.Free = true;
		}
		foreach (HatData item in (Il2CppArrayBase<HatData>)(object)hatManager.allHats)
		{
			((CosmeticData)item).Free = true;
		}
		foreach (NamePlateData item2 in (Il2CppArrayBase<NamePlateData>)(object)hatManager.allNamePlates)
		{
			((CosmeticData)item2).Free = true;
		}
		foreach (PetData item3 in (Il2CppArrayBase<PetData>)(object)hatManager.allPets)
		{
			((CosmeticData)item3).Free = true;
		}
		foreach (SkinData item4 in (Il2CppArrayBase<SkinData>)(object)hatManager.allSkins)
		{
			((CosmeticData)item4).Free = true;
		}
		Enumerator<StarBundle> enumerator8 = hatManager.allStarBundles.GetEnumerator();
		while (enumerator8.MoveNext())
		{
			enumerator8.Current.price = 0f;
		}
		foreach (VisorData item5 in (Il2CppArrayBase<VisorData>)(object)hatManager.allVisors)
		{
			((CosmeticData)item5).Free = true;
		}
	}
}
