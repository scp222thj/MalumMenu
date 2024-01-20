using HarmonyLib;
using UnityEngine;

namespace MalumMenu;

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
public static class Tracers_PlayerPhysics_LateUpdate_Postfix
{
    public static LineRenderer lineRenderer;
    public static Color color;

    //Postfix patch of PlayerPhysics.LateUpdate to render tracer lines for each player
    public static void Postfix(PlayerPhysics __instance){
        try{ //try-catch to avoid errors in lobbies
            if (!__instance.AmOwner){ //Don't draw a tracer for LocalPlayer

                //Tracers are just LineRenderers for each player
                lineRenderer = __instance.myPlayer.gameObject.GetComponent<LineRenderer>();

                if(!lineRenderer){
                    lineRenderer = __instance.myPlayer.gameObject.AddComponent<LineRenderer>();
                }

                lineRenderer.SetVertexCount(2);
                lineRenderer.SetWidth(0.02F, 0.02F);

                color = Color.clear; //Initally make tracer line invisible

                //Crewmate, alive
                if (CheatToggles.tracersCrew && !__instance.myPlayer.Data.Role.IsImpostor){
                    if (!__instance.myPlayer.Data.IsDead){
                        if (CheatToggles.colorBasedTracers){
                            color = __instance.myPlayer.Data.Color; //Color-Based Tracer
                        }else{
                            color = __instance.myPlayer.Data.Role.TeamColor; //Role-Based Tracer
                        }
                    }

                //Impostor, alive
                } else if (CheatToggles.tracersImps && __instance.myPlayer.Data.Role.IsImpostor){
                    if (!__instance.myPlayer.Data.IsDead){
                        if (CheatToggles.colorBasedTracers){
                            color = __instance.myPlayer.Data.Color; //Color-Based Tracer
                        }else{
                            color = __instance.myPlayer.Data.Role.TeamColor; //Role-Based Tracer
                        }
                    }
                }

                //Any Role, dead
                if (CheatToggles.tracersGhosts && __instance.myPlayer.Data.IsDead){
                    if (CheatToggles.colorBasedTracers){
                        color = __instance.myPlayer.Data.Color; //Color-Based Tracers
                    }else{
                        color = Palette.White;
                    }
                }

                //I just picked an already existing material from the game
                Material material = DestroyableSingleton<HatManager>.Instance.PlayerMaterial;

                lineRenderer.material = material;
                lineRenderer.SetColors(color, color);
                        
                //Tracers are connected from each player to LocalPlayer  
                lineRenderer.SetPosition(0, __instance.myPlayer.transform.position);
                lineRenderer.SetPosition(1, PlayerControl.LocalPlayer.transform.position);

            }
        }catch{}
    }
}    

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
public static class TracersDeadBodies_PlayerPhysics_LateUpdate_Postfix
{
    public static LineRenderer lineRenderer;
    public static Color color;
    
    //Postfix patch of PlayerPhysics.LateUpdate to render tracer lines for each dead body
    public static void Postfix(PlayerPhysics __instance){
        GameObject[] bodyObjects = GameObject.FindGameObjectsWithTag("DeadBody");
        foreach(GameObject bodyObject in bodyObjects) //Finds and loops through all dead bodies
        {
            DeadBody deadBody = bodyObject.GetComponent<DeadBody>();

            if (deadBody){
                if (!deadBody.Reported){ //Only unreported dead bodies have tracers

                    //Tracers are just LineRenderers for each dead body
                    lineRenderer = bodyObject.GetComponent<LineRenderer>();

                    if(!lineRenderer){
                        lineRenderer = bodyObject.AddComponent<LineRenderer>();
                    }

                    lineRenderer.SetVertexCount(2);
                    lineRenderer.SetWidth(0.02F, 0.02F);

                    color = Color.clear; //Initally make tracer line invisible

                    if (CheatToggles.tracersBodies){
                        if (CheatToggles.colorBasedTracers){

                            //Fetch the dead body's PlayerInfo
                            GameData.PlayerInfo playerById = GameData.Instance.GetPlayerById(deadBody.ParentId);

                            color = playerById.Color; //Color-Based Tracers

                        }else{

                            color = Color.yellow;

                        }
                    }    

                    //I just picked an already existing material from the game
                    Material material = DestroyableSingleton<HatManager>.Instance.PlayerMaterial;

                    lineRenderer.material = material;
                    lineRenderer.SetColors(color, color);
            
                    //Tracers are connected from each dead body to LocalPlayer  
                    lineRenderer.SetPosition(0, bodyObject.transform.position);
                    lineRenderer.SetPosition(1, PlayerControl.LocalPlayer.transform.position);
                
                }
            }
        }
    }
}    