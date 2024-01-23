using HarmonyLib;
using System.Linq;
using Il2CppSystem.Collections.Generic;
using System;

namespace MalumMenu;

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
public static class ChangeRole_PlayerPhysics_LateUpdate_Postfix
{
    //Postfix patch of PlayerPhysics.LateUpdate to open player pick menu to murder any player
    public static bool isActive;
    public static AmongUs.GameOptions.RoleTypes? oldRole = null;
    public static void Postfix(PlayerPhysics __instance){
        if (CheatToggles.changeRole){

            if (!isActive){

                //Close any player pick menus already open & their cheats
                if (Utils_PlayerPickMenu.playerpickMenu != null){
                    Utils_PlayerPickMenu.playerpickMenu.Close();
                    CheatToggles.DisablePPMCheats("changeRole");
                }

                List<GameData.PlayerInfo> playerDataList = new List<GameData.PlayerInfo>();

                if (oldRole == AmongUs.GameOptions.RoleTypes.Shapeshifter){

                    GameData.PlayerInfo shapeshifterChoice = new GameData.PlayerInfo(255)
                    {
                        PlayerName = "Shapeshifter"
                    };
                    shapeshifterChoice.Outfits[PlayerOutfitType.Default].ColorId = 0;
                    shapeshifterChoice.Outfits[PlayerOutfitType.Default].SkinId = "skin_screamghostface";
                    shapeshifterChoice.Outfits[PlayerOutfitType.Default].VisorId = "visor_eliksni";
                    shapeshifterChoice.Role = RoleManager.Instance.AllRoles.First(r => r.Role == AmongUs.GameOptions.RoleTypes.Shapeshifter);
                    shapeshifterChoice.PlayerName = Utils.getRoleName(shapeshifterChoice);
                    playerDataList.Add(shapeshifterChoice);

                }

                GameData.PlayerInfo impostorChoice = new GameData.PlayerInfo(255)
                {
                    PlayerName = "Impostor"
                };
                impostorChoice.Outfits[PlayerOutfitType.Default].ColorId = 0;
                impostorChoice.Role = RoleManager.Instance.AllRoles.First(r => r.Role == AmongUs.GameOptions.RoleTypes.Impostor);
                impostorChoice.PlayerName = Utils.getRoleName(impostorChoice);
                playerDataList.Add(impostorChoice);

                GameData.PlayerInfo engineerChoice = new GameData.PlayerInfo(255)
                {
                    PlayerName = "Engineer"
                };
                engineerChoice.Outfits[PlayerOutfitType.Default].ColorId = 10;
                engineerChoice.Outfits[PlayerOutfitType.Default].SkinId = "skin_Mech";
                engineerChoice.Outfits[PlayerOutfitType.Default].VisorId = "visor_D2CGoggles";
                engineerChoice.Role = RoleManager.Instance.AllRoles.First(r => r.Role == AmongUs.GameOptions.RoleTypes.Engineer);
                engineerChoice.PlayerName = Utils.getRoleName(engineerChoice);
                playerDataList.Add(engineerChoice);

                GameData.PlayerInfo scientistChoice = new GameData.PlayerInfo(255)
                {
                    PlayerName = "Scientist"
                };
                scientistChoice.Outfits[PlayerOutfitType.Default].ColorId = 10;
                scientistChoice.Outfits[PlayerOutfitType.Default].SkinId = "skin_Science";
                scientistChoice.Outfits[PlayerOutfitType.Default].VisorId = "visor_pk01_PaperMaskVisor";
                scientistChoice.Role = RoleManager.Instance.AllRoles.First(r => r.Role == AmongUs.GameOptions.RoleTypes.Scientist);
                scientistChoice.PlayerName = Utils.getRoleName(scientistChoice);
                playerDataList.Add(scientistChoice);

                GameData.PlayerInfo crewmateChoice = new GameData.PlayerInfo(255)
                {
                    PlayerName = "Crewmate"
                };
                crewmateChoice.Outfits[PlayerOutfitType.Default].ColorId = 10;
                crewmateChoice.Role = RoleManager.Instance.AllRoles.First(r => r.Role == AmongUs.GameOptions.RoleTypes.Crewmate);
                crewmateChoice.PlayerName = Utils.getRoleName(crewmateChoice);
                playerDataList.Add(crewmateChoice);

                //New player pick menu made for killing players
                Utils_PlayerPickMenu.openPlayerPickMenu(playerDataList, (Action) (() =>
                {

                    if (!Utils.isLobby && oldRole == null){
                        oldRole = PlayerControl.LocalPlayer.Data.RoleType;
                    }

                    if (PlayerControl.LocalPlayer.Data.IsDead){
                        if (Utils_PlayerPickMenu.targetPlayerData.Role.TeamType == RoleTeamTypes.Impostor){
                            RoleManager.Instance.SetRole(PlayerControl.LocalPlayer, AmongUs.GameOptions.RoleTypes.ImpostorGhost);
                        }else{
                            RoleManager.Instance.SetRole(PlayerControl.LocalPlayer, AmongUs.GameOptions.RoleTypes.CrewmateGhost);
                        }
                    }else{
                        RoleManager.Instance.SetRole(PlayerControl.LocalPlayer, Utils_PlayerPickMenu.targetPlayerData.Role.Role);
                    }

                    
                
                }));

                isActive = true;
            }

            //Deactivate cheat if menu is closed
            if (Utils_PlayerPickMenu.playerpickMenu == null){
                CheatToggles.changeRole = false;
            }

        }else{
            if (isActive){
                isActive = false;
            }
        }
    }
}