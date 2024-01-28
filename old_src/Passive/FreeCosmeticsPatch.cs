using HarmonyLib;

namespace MalumMenu;

[HarmonyPatch(typeof(HatManager), nameof(HatManager.Initialize))]
public static class FreeCosmetics_HatManager_Initialize_Postfix
{
    //Postfix patch of HatManager.Initialize to loop through all cosmetics and set them as free
    public static void Postfix(HatManager __instance){

        if (!CheatToggles.freeCosmetics){
            return;
        }

        foreach(var bundle in __instance.allBundles){ //Bundles
            bundle.Free = true;
        }

        foreach(var featuredBundle in __instance.allFeaturedBundles){ //Featured Bundles
            featuredBundle.Free = true;
        }

        foreach(var featuredCube in __instance.allFeaturedCubes){ //Featured Cosmicubes
            featuredCube.Free = true;
        }

        foreach(var featuredItem in __instance.allFeaturedItems){ //Featured Items
            featuredItem.Free = true;
        }

        foreach(var hat in __instance.allHats){ //Hats
            hat.Free = true;
        }

        foreach(var nameplate in __instance.allNamePlates){ //NamePlates
            nameplate.Free = true;
        }

        foreach(var pet in __instance.allPets){ //Pets
            pet.Free = true;
        }

        foreach(var skin in __instance.allSkins){ //Skins
            skin.Free = true;
        }

        foreach(var starBundle in __instance.allStarBundles){ //Star Bundles
            starBundle.price = 0; // StarBundles don't have a Free property, so price is changed instead
        }

        foreach(var visor in __instance.allVisors){ //Visors
            visor.Free = true;
        }
        
    }

}
