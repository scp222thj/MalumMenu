# Host Feature Extract

This folder contains extracted source code from `ModMenuCrew.dll` for the host-only features shown in the screenshot.

## Included Features

- `InstantStartGame()`
- `MapCheats.DestroyMap()`
- `MapCheats.SpawnLobby()`

## Notes

- The code is decompiled from `ModMenuCrew.dll` and is intended as a reference implementation.
- It depends on Among Us game assembly types (`AmongUsClient`, `LobbyBehaviour`, `ShipStatus`, `GameStartManager`, etc.), Unity, and BepInEx.
- You can use this folder as a starting point for adding host map/lobby features to a new project.

## Files

- `GameCheatsHostFeatures.cs` — extracted methods and helper class used by these features.
- `FabMenuExtracted/` — extracted `PlayerModifier` and chat dark mode source with dependencies.
- `HostFeatureLibrary.csproj` — simple project file for organizing the extracted source.
