# Dependency Summary

The extracted host feature code depends on the following external types and systems:

- `AmongUsClient` / `InnerNetClient`
- `LobbyBehaviour`
- `ShipStatus`
- `GameStartManager`
- `LobbyBehaviour.Instance`
- `DestroyableSingleton<T>`
- `HudManager` / `GameMenu`
- `UnityEngine.Object` and `UnityEngine.Component`
- `UnityEngine.Time`
- `Hazel.SpawnFlags`
- `IL2CPP`-specific wrappers (`Il2CppObjectBase`, `InnerNetObject`)
- `IntegrityGuard` (host integrity check)
- `ChatController` / `ChatBubble` / `ChatController.AddChat`
- `PlayerControl` / `NetworkedPlayerInfo` / `PlayerOutfit`
- `Palette.PlayerColors` / `PlayerColor` / `SetColor`
- `HarmonyLib` patching for chat and player color sync

## What is extracted

- `InstantStartGame()`
- `MapCheats.DestroyMap()`
- `MapCheats.SpawnLobby()`
- `PlayerModifier.ChangePlayerColor()`
- `ChatController_AddChat` Harmony patch for `[FABMOD]` color sync messages
- `ChatBubble_SetName` Harmony patch for dark chat bubble styling

## Notes

- The code is decompiled and preserved as close as possible to the original implementation.
- It is not a complete standalone library; the external Among Us and Unity game assemblies are required to make it work.
- Use this folder as a reference when implementing the host tab features in your own project.
