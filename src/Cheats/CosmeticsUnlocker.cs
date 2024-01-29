namespace MalumMenu;
public static class CosmeticsUnlocker
{
    public static void unlockCosmetics(HatManager hatManager){
        if (CheatToggles.freeCosmetics){
            foreach(var bundle in hatManager.allBundles){ //Bundles
            bundle.Free = true;
            }

            foreach(var featuredBundle in hatManager.allFeaturedBundles){ //Featured Bundles
                featuredBundle.Free = true;
            }

            foreach(var featuredCube in hatManager.allFeaturedCubes){ //Featured Cosmicubes
                featuredCube.Free = true;
            }

            foreach(var featuredItem in hatManager.allFeaturedItems){ //Featured Items
                featuredItem.Free = true;
            }

            foreach(var hat in hatManager.allHats){ //Hats
                hat.Free = true;
            }

            foreach(var nameplate in hatManager.allNamePlates){ //NamePlates
                nameplate.Free = true;
            }

            foreach(var pet in hatManager.allPets){ //Pets
                pet.Free = true;
            }

            foreach(var skin in hatManager.allSkins){ //Skins
                skin.Free = true;
            }

            foreach(var starBundle in hatManager.allStarBundles){ //Star Bundles
                starBundle.price = 0; // StarBundles don't have a Free property, so price is changed instead
            }

            foreach(var visor in hatManager.allVisors){ //Visors
                visor.Free = true;
            }
        }
    }
}