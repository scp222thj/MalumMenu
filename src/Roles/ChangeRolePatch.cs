using HarmonyLib;
using System.Linq;
using Il2CppSystem.Collections.Generic;
using System;

namespace MalumMenu;

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
public static class ChangeRole_PlayerPhysics_LateUpdate_Postfix
{
    // Postfix patch of PlayerPhysics.LateUpdate to open player pick menu to pick a role
    public static bool isActive;
    public static AmongUs.GameOptions.RoleTypes? oldRole = null;
    public static void Postfix(PlayerPhysics __instance)
    {
        if (CheatToggles.changeRole)
        {
            if (!isActive)
            {
                // Close any player pick menus already open & their cheats
                if (Utils_PlayerPickMenu.playerpickMenu != null)
                {
                    Utils_PlayerPickMenu.playerpickMenu.Close();
                    CheatToggles.DisablePPMCheats("changeRole");
                }

                // Creates a list of roles for the pick menu
                List<GameData.PlayerInfo> roleList = new List<GameData.PlayerInfo>();

                // This check is done to prevent players from getting kicked by mistake
                // when switching to shapeshifter role & trying to shapeshift
                if (oldRole == AmongUs.GameOptions.RoleTypes.Shapeshifter)
                {
                    // Shapeshifter custom choice
                    GameData.PlayerInfo shapeshifterChoice = new GameData.PlayerInfo(255)
                    {
                        PlayerName = "Shapeshifter"
                    };
                    shapeshifterChoice.Outfits[PlayerOutfitType.Default].ColorId = 0;
                    shapeshifterChoice.Outfits[PlayerOutfitType.Default].SkinId = "skin_screamghostface";
                    shapeshifterChoice.Outfits[PlayerOutfitType.Default].VisorId = "visor_eliksni";
                    shapeshifterChoice.Role = RoleManager.Instance.AllRoles.First(r => r.Role == AmongUs.GameOptions.RoleTypes.Shapeshifter);
                    shapeshifterChoice.PlayerName = Utils.getRoleName(shapeshifterChoice);
                    roleList.Add(shapeshifterChoice);

                }

                // Impostor choice for the pick menu
                GameData.PlayerInfo impostorChoice = new GameData.PlayerInfo(255)
                {
                    PlayerName = "Impostor"
                };
                impostorChoice.Outfits[PlayerOutfitType.Default].ColorId = 0;
                impostorChoice.Role = RoleManager.Instance.AllRoles.First(r => r.Role == AmongUs.GameOptions.RoleTypes.Impostor);
                impostorChoice.PlayerName = Utils.getRoleName(impostorChoice);
                roleList.Add(impostorChoice);

                // Engineer choice for the pick menu
                GameData.PlayerInfo engineerChoice = new GameData.PlayerInfo(255)
                {
                    PlayerName = "Engineer"
                };
                engineerChoice.Outfits[PlayerOutfitType.Default].ColorId = 10;
                engineerChoice.Outfits[PlayerOutfitType.Default].SkinId = "skin_Mech";
                engineerChoice.Outfits[PlayerOutfitType.Default].VisorId = "visor_D2CGoggles";
                engineerChoice.Role = RoleManager.Instance.AllRoles.First(r => r.Role == AmongUs.GameOptions.RoleTypes.Engineer);
                engineerChoice.PlayerName = Utils.getRoleName(engineerChoice);
                roleList.Add(engineerChoice);

                // Scientist choice for the pick menu
                GameData.PlayerInfo scientistChoice = new GameData.PlayerInfo(255)
                {
                    PlayerName = "Scientist"
                };
                scientistChoice.Outfits[PlayerOutfitType.Default].ColorId = 10;
                scientistChoice.Outfits[PlayerOutfitType.Default].SkinId = "skin_Science";
                scientistChoice.Outfits[PlayerOutfitType.Default].VisorId = "visor_pk01_PaperMaskVisor";
                scientistChoice.Role = RoleManager.Instance.AllRoles.First(r => r.Role == AmongUs.GameOptions.RoleTypes.Scientist);
                scientistChoice.PlayerName = Utils.getRoleName(scientistChoice);
                roleList.Add(scientistChoice);

                // Crewmate choice for the pick menu
                GameData.PlayerInfo crewmateChoice = new GameData.PlayerInfo(255)
                {
                    PlayerName = "Crewmate"
                };
                crewmateChoice.Outfits[PlayerOutfitType.Default].ColorId = 10;
                crewmateChoice.Role = RoleManager.Instance.AllRoles.First(r => r.Role == AmongUs.GameOptions.RoleTypes.Crewmate);
                crewmateChoice.PlayerName = Utils.getRoleName(crewmateChoice);
                roleList.Add(crewmateChoice);

                // New player pick menu made for changing your roles with the custom list
                Utils_PlayerPickMenu.openPlayerPickMenu(roleList, (Action)(() =>
                {
                    // Log the old, original role before it gets changed by changeRole cheat
                    if (!Utils.isLobby && oldRole == null)
                    {
                        oldRole = PlayerControl.LocalPlayer.Data.RoleType;
                    }

                    if (PlayerControl.LocalPlayer.Data.IsDead)
                    { // Prevent accidential client-side revives
                        if (Utils_PlayerPickMenu.targetPlayerData.Role.TeamType == RoleTeamTypes.Impostor)
                        {
                            RoleManager.Instance.SetRole(PlayerControl.LocalPlayer, AmongUs.GameOptions.RoleTypes.ImpostorGhost);
                        }
                        else
                        {
                            RoleManager.Instance.SetRole(PlayerControl.LocalPlayer, AmongUs.GameOptions.RoleTypes.CrewmateGhost);
                        }
                    }
                    else
                    {
                        RoleManager.Instance.SetRole(PlayerControl.LocalPlayer, Utils_PlayerPickMenu.targetPlayerData.Role.Role);
                    }
                }));

                isActive = true;
            }

            // Deactivate cheat if menu is closed
            if (Utils_PlayerPickMenu.playerpickMenu == null)
            {
                CheatToggles.changeRole = false;
            }
        }
        else
        {
            if (isActive)
            {
                isActive = false;
            }
        }
    }
}