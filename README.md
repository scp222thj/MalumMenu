
<p align="center">
  <img width="1028" height="441" alt="banner" src="./logo.png" />
</p>


<p align="center">

  <a href="https://discord.gg/YYcYf88jAb">
    <img src="https://img.shields.io/badge/Join%20us%20on-Discord-blue?style=flat&logo=discord" alt="Discord">
  </a>

  <a href="https://github.com/astra1dev#%EF%B8%8F-support-me">
    <img src="https://img.shields.io/badge/Support-me-ff5f5f?style=flat&logo=github-sponsors">
  </a>

  <a href="https://github.com/astra1dev/MalumMenu/actions/workflows/main.yml">
    <img src="https://github.com/astra1dev/MalumMenu/actions/workflows/main.yml/badge.svg?event=push&style=plastic">
  </a>

  <a href="../../releases">
    <img src="https://img.shields.io/github/downloads/astra1dev/MalumMenu/total.svg?style=plastic&color=red">
  </a>

  <a href="../../releases/latest">
    <img src="https://img.shields.io/github/downloads/astra1dev/MalumMenu/latest/total?style=plastic">
  </a>

</p>

<p align="center">
<b>An easy-to-use Among Us cheat menu with a simple GUI and lots of useful modules. </b>


# 🎁 Releases

**PLEASE READ**:
This repository (more specifically, the `reloaded` branch) is my personal fork of the original MalumMenu project.

| Mod Version | Among Us - Version | Link                                  |
|-------------|--------------------|---------------------------------------|
| v2.6.1      | 2025.9.9 (17.0.0)  | [Download](../../releases/tag/v2.6.1) |
| v2.5.3      | 2025.9.9 (17.0.0)  | [Download](../../releases/tag/v2.5.3) |
| v2.5.2      | 2025.6.10 (16.1.0) | [Download](../../releases/tag/v2.5.2) |
| v2.5.1      | 2025.6.10 (16.1.0) | [Download](../../releases/tag/v2.5.1) |

For older (official) versions, please refer to the [original MalumMenu repository](https://github.com/scp222thj/MalumMenu).

# ⬇️ Installation

The installation process is the same as for the original MalumMenu (see [here](https://github.com/scp222thj/MalumMenu?tab=readme-ov-file#%EF%B8%8F-installation)).

Instead of downloading the latest ZIP or DLL release from the original repository, download it from the table above (or get the CI build artifact from the latest commit for more bleeding-edge features).

If you are using the DLL, make sure you have BepInEx 6.0.0-BE-735 installed. Older ones will not work.

Make sure you are only having one version of MalumMenu installed at a time, as having multiple versions can cause issues.

# 📋 Features

![](https://github.com/user-attachments/assets/e7342201-aa01-4435-8c9e-d543712842e0)

## Changes compared to the original MalumMenu
- [Full Changelog](https://github.com/scp222thj/MalumMenu/compare/main...astra1dev:MalumMenu:reloaded)
- [Original MalumMenu feature list](https://github.com/scp222thj/MalumMenu?tab=readme-ov-file#-features)
### Fixes
- v16.0.0 fix where the menu wouldn't load at all
- v17.0.0 fix where PPM and SeeRoles wouldn't work
- Fix SeeRoles nametags overlaying with colorblind text if it is enabled
- Fix killing as impostor kicking you from the lobby
- Fix detecting if the player is the lobby host
- Fix not being able to input russian characters (and possibly others) in chat
- Fix and enable previously implemented but disabled Telekill cheat

### Additions
#### New cheats
- **Player**: Fake Revive, Invert Controls
- **ESP**: Show Player Info, More Lobby Info, Show Task Arrows, Distance-based tracers
- **Roles**: Do Tasks as Impostor, Tasks Menu (to complete individual tasks and see other players' tasks), Track Reach, Interrogate Reach
- **Ship**: Call Meeting, Open Sabotage Map, Trigger Spores ([#40](https://github.com/scp222thj/MalumMenu/pull/40)), Auto-Open Doors On Use, Doors Menu (to close / open individual doors)
- **Console** (NEW!): Show Console, Log Deaths, Log Shapeshifts, Log Vents
- **Host-Only**: No Options Limits, Protect Player Menu, Force Role
  - **Meetings** (NEW!): Skip Meeting, VoteImmune, Eject Player
  - **Game State** (NEW!) ([#49](https://github.com/scp222thj/MalumMenu/pull/49)): Force Start Game, No Game End
- **Passive**: Spoof Date to April 1st, Stealth Mode, Panic (Disable MalumMenu), Copy Lobby Code on Disconnect
- **Animations**: (NEW!): Shields, Asteroids, Empty Garbage, Medbay Scan, Fake Cams In Use, Pet, Moonwalk
- **Config** (NEW!): Open plugin config, Reload plugin config, Save to Profile, Load from Profile, RGB Mode
#### New features and QoL improvements
- Added option to disable cheats in Passive category ([#164](https://github.com/scp222thj/MalumMenu/pull/164))
- Added new Viper and Detective roles to "Set Fake Role" cheat (Roles category)
- Added a new horizontal tab-based UI config option
- Changed "SpeedHack" to be a slider instead of a toggle (Player category)
- Added a keybind system to bind cheats to keyboard keys (defined in MalumProfile.txt)
- Added pasting and cutting text between the chatbox and the device's clipboard
- Changed "ZoomOut" to disable while Chat, Friends List or Game Settings Panel is open
- Added being able to kick players while in-game as host (no 3 votes required to kick)
- Added "TeleportMenuToMouse" config option

### Other changes
- Some refactoring and code style changes
- BepInEx version bump and CI updates

<hr>

<details>
  <summary>Known Issues, won't be fixed</summary>

  - Current Room Name doesn't show when NoClip is enabled
  - No "slide-in" animation plays when a PPM is opened
  - If the player opens any PPM while the shapeshift menu is open, the menus will overlay on each other
  - If the player opens any PPM while walking, the player will keep walking until the PPM is closed
  - "Complete all tasks" sometimes doesn't complete all tasks
  - NameTag ESP-related features (e.g. "Show Player Info") don't apply to previous chat messages when toggled
  - Some cheats automatically get turned off when the player leaves a game (e.g. NoClip)
</details>

# ⚠️ Disclaimer

This mod is not affiliated with Among Us or Innersloth LLC, and the content contained therein is not endorsed or otherwise sponsored by Innersloth LLC. Portions of the materials contained herein are property of Innersloth LLC.

Usage of this mod can violate the terms of service of Among Us, which may lead to punitive action including temporary or permanent bans from the game. The creator is not responsible for any consequences you may face due to usage. Use at your own risk.
