// Decompiled extraction from ModMenuCrew.dll
// Contains the host-only feature implementations for instant start, remove map, and spawn lobby.
// Requires Among Us, BepInEx, Unity, and game-specific types to compile.

using System;
using AmongUs.GameOptions;
using Hazel;
using InnerNet;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ModMenuCrew.Features
{
    public static partial class GameCheats
    {
        private static float _lastInstantStartTime;

        internal static void InstantStartGame()
        {
            if (!IntegrityGuard.IsIntact || Time.time - _lastInstantStartTime < 3f || (Object)(object)AmongUsClient.Instance == (Object)null || !((InnerNetClient)AmongUsClient.Instance).AmHost || (int)((InnerNetClient)AmongUsClient.Instance).GameState == 2)
            {
                return;
            }
            try
            {
                _lastInstantStartTime = Time.time;
                AmongUsClient.Instance.KickNotJoinedPlayers();
                HudManager instance = DestroyableSingleton<HudManager>.Instance;
                if ((Object)(object)((instance != null) ? instance.GameMenu : null) != (Object)null && DestroyableSingleton<HudManager>.Instance.GameMenu.IsOpen)
                {
                    DestroyableSingleton<HudManager>.Instance.GameMenu.Close();
                }
                AmongUsClient.Instance.StartGame();
                GameStartManager instance2 = DestroyableSingleton<GameStartManager>.Instance;
                if ((Object)(object)instance2 != (Object)null)
                {
                    AmongUsClient.Instance.DisconnectHandlers.Remove(((Il2CppObjectBase)instance2).Cast<IDisconnectHandler>());
                    Object.Destroy((Object)(object)((Component)instance2).gameObject);
                }
            }
            catch (Exception value)
            {
                Debug.LogError(Object.op_Implicit($"[InstantStart] Error: {value}"));
            }
        }

        public static partial class MapCheats
        {
            internal static void DestroyMap()
            {
                if (!((InnerNetClient)AmongUsClient.Instance).AmHost)
                {
                    Debug.LogWarning(Object.op_Implicit("[MapCheats] Only the host can remove the map or lobby."));
                    return;
                }
                LobbyBehaviour instance = LobbyBehaviour.Instance;
                if ((Object)(object)instance != (Object)null)
                {
                    if (instance != null)
                    {
                        ((InnerNetObject)instance).Despawn();
                    }
                    LobbyBehaviour.Instance = null;
                    Debug.Log(Object.op_Implicit("[MapCheats] LobbyBehaviour despawned e singleton limpo."));
                }
                ShipStatus instance2 = ShipStatus.Instance;
                if ((Object)(object)instance2 != (Object)null)
                {
                    if (instance2 != null)
                    {
                        ((InnerNetObject)instance2).Despawn();
                    }
                    ShipStatus.Instance = null;
                    Debug.Log(Object.op_Implicit("[MapCheats] ShipStatus despawned e singleton limpo."));
                }
            }

            internal static void SpawnLobby()
            {
                if (!((InnerNetClient)AmongUsClient.Instance).AmHost)
                {
                    Debug.LogWarning(Object.op_Implicit("[MapCheats] Only the host can create the lobby."));
                }
                else if ((Object)(object)LobbyBehaviour.Instance == (Object)null)
                {
                    LobbyBehaviour lobbyPrefab = DestroyableSingleton<GameStartManager>.Instance.LobbyPrefab;
                    if ((Object)(object)lobbyPrefab != (Object)null)
                    {
                        LobbyBehaviour.Instance = Object.Instantiate<LobbyBehaviour>(lobbyPrefab);
                        ((InnerNetClient)AmongUsClient.Instance).Spawn((InnerNetObject)(object)LobbyBehaviour.Instance, -2, (SpawnFlags)0);
                        Debug.Log(Object.op_Implicit("[MapCheats] LobbyBehaviour spawned via prefab."));
                    }
                    else
                    {
                        Debug.LogWarning(Object.op_Implicit("[MapCheats] LobbyPrefab not found in GameStartManager."));
                    }
                }
                else
                {
                    Debug.LogWarning(Object.op_Implicit("[MapCheats] LobbyBehaviour already exists."));
                }
            }
        }
    }
}
