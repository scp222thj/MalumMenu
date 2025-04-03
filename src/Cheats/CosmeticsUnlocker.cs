namespace MalumMenu;

public static class CosmeticsUnlocker
{
    public static void unlockCosmetics(HatManager hatManager)
    {
        if (!CheatToggles.freeCosmetics || hatManager == null) return;

        // Bundles
        foreach (var bundle in hatManager.allBundles)
            bundle.Free = true;

        // Featured Bundles
        foreach (var featuredBundle in hatManager.allFeaturedBundles)
            featuredBundle.Free = true;

        // Featured Cosmicubes
        foreach (var featuredCube in hatManager.allFeaturedCubes)
            featuredCube.Free = true;

        // Featured Items
        foreach (var featuredItem in hatManager.allFeaturedItems)
            featuredItem.Free = true;

        // Hats
        foreach (var hat in hatManager.allHats)
            hat.Free = true;

        // Nameplates
        foreach (var nameplate in hatManager.allNamePlates)
            nameplate.Free = true;

        // Pets
        foreach (var pet in hatManager.allPets)
            pet.Free = true;

        // Skins
        foreach (var skin in hatManager.allSkins)
            skin.Free = true;

        // Star Bundles (no Free flag, use price instead)
        foreach (var starBundle in hatManager.allStarBundles)
            starBundle.price = 0;

        // Visors
        foreach (var visor in hatManager.allVisors)
            visor.Free = true;
    }
}