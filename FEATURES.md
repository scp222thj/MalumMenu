# 📋 Features

## 👱 Player

| Cheat | Description | Type | Default|
|------------|-------------|------|--------|
| NoClip     | Allows you to walk through walls like a ghost | Toggle | Off
| SpeedBoost | Doubles your player's speed | Toggle | Off |

#### Murder

| Cheat | Description | Type | Default|
|------------|-------------|------|--------|
| MurderPlayer | Select a player to kill them immediatly | Menu |
| MurderAll | Kill all players immediatly | Toggle | Off |

#### Teleport

| Cheat | Description | Type | Default|
|------------|-------------|------|--------|
| to Cursor | Teleport by right-clicking with your cursor. Works best with the ZoomOut cheat | Toggle | Off |
| to Player | Teleport to a player's position by selecting them | Menu |

## 👁️ ESP

MalumMenu's ESP cheats are completely client-side, and thus undetectable by anticheat

| Cheat | Description | Type | Default|
|------------|-------------|------|--------|
| SeeRoles | See every player's role through their nametag | Toggle | Off |
| SeeGhosts | Allows you to see ghosts, protections, and ghost chat even if you are alive | Toggle | Off
| FullBright | Removes all shadows, allowing you to see during blackouts and even through walls<br>Also, lets you see through spore clouds in the Fungle Jungle | Toggle | Off |
| RevealVotes | Reveals votes as they are being cast rather than at the end of the meeting<br>Also, lets you see colored votes even if votes are set to anonymous | Toggle | Off |
| AlwaysChat | Keeps the chat icon always enabled, allowing you to chat at any time (even while not in a meeting or the lobby) | Toggle | Off |

#### Camera
    
| Cheat | Description | Type | Default|
|------------|-------------|------|--------|
| ZoomOut | Allows you to zoom-out the player's camera using your mouse's scrollwheel | Toggle | Off
| Spectate | Allows you to pick a player to spectate with your camera | Menu |
| FreeCam | Allows you to freely move your camera around without also moving your player | Toggle | Off |

#### Tracers

| Cheat | Description | Type | Default|
|------------|-------------|------|--------|
| Crewmates | Shows tracer lines for alive crewmates (color: cyan) | Toggle | Off |
| Impostors | Shows tracer lines for alive impostors (color: red) | Toggle | Off
| Ghosts | Shows tracer lines for ghosts (color: white) | Toggle | Off |
| Dead Bodies | Shows tracer lines for dead bodies on the ground (color: yellow) | Toggle | Off |
| Color-based | Changes the color of tracer lines to the color of their players| Toggle | Off |

#### Minimap

| Cheat | Description | Type | Default|
|------------|-------------|------|--------|
| Crewmates | Changes the map so that it shows the position of every alive crewmate (color: cyan) | Toggle | Off |
| Impostors | Changes the map so that it shows the position of every alive impostor (color: red) | Toggle | Off
| Ghosts | Changes the map so that it shows the position of every ghost (color: white) | Toggle | Off |
| Color-based | Changes the color of map icons to the color of their players | Toggle | Off |

## 🎭 Roles

| Cheat | Description | Type | Default |
|------------|-------------|------|----|
| ChangeRole | Change your current role to any role you want<br>(Shapeshifter is disabled by default to prevent getting detected by the anticheat) | Menu |

#### Crewmate

| Cheat | Description | Type | Default |
|------------|-------------|------|----|
| CompleteMyTasks | Complete all of your crewmate tasks immediatly | Button |

#### Impostor

| Cheat | Description | Type | Default |
|------------|-------------|------|----|
| KillAnyone | Allows you to kill anyone, regardless if they are protected, impostors, crawling in a vent, or a ghost | Toggle | Off |
| NoKillCooldown | Removes the cooldown period after kills, allowing you to spam-kill as much as you please | Toggle | Off |
| KillReach | Allows you to kill players regardless of how far they are on the map | Toggle | Off |

#### Shapeshifter

| Cheat | Description | Type | Default |
|------------|-------------|------|----|
| NoSsAnimation | Removes the shapeshift animation, making shapeshifting much quicker | Toggle | Off |
| EndlessSsDuration | Allows you to remain shapeshifted forever | Toggle | Off |

#### Engineer

| Cheat | Description | Type | Default |
|------------|-------------|------|----|
| EndlessVentTime | Allows you to remain inside a vent forever despite being an engineer | Toggle | Off |
| NoVentCooldown | Removes the cooldown period after coming out of a vent | Toggle | Off |

#### Scientist

| Cheat | Description | Type | Default |
|------------|-------------|------|----|
| EndlessBattery | The battery on your vitals panel will never run out | Toggle | Off |
| NoVitalsCooldown | Removes the cooldown period after closing vitals panel  | Toggle | Off |
    
## 🚀 Ship

| Cheat | Description | Type | Default |
|------------|-------------|------|----|
| UnfixableLights | Disables lights completely (they cannot be fixed manually by players)<br>You can enable them again by clicking the button | Toggle | Off |
| ReportBody | Report any player as a dead body to start a meeting | Button |
| CloseMeeting | Forcefully closes the meeting window (only for you), allowing you to move and interact with the game during meetings | Button |

#### Sabotage

MalumMenu's Sabotage cheats work even if you aren't impostor and are subject to no cooldown.

Moreover, different sabotages can be enabled at the same time, and they even work during meetings.

| Cheat | Description | Type | 
|------------|-------------|------|
| Reactor | Allows you to enable/disable Reactor sabotage | Toggle | Off |
| Oxygen | Allows you to enable/disable Oxygen sabotage | Toggle | Off |
| Lights | Allows you to enable/disable Lights sabotage | Toggle | Off |
| Comms | Allows you to enable/disable Communications sabotage | Toggle | Off |
| Doors | Immediatly locks all doors on the ship | Button |
| MushroomMixup | Induces Mushroom Mixup sabotage on Fungle map | Button |

#### Vents

| Cheat | Description | Type | Default|
|------------|-------------|------|--------|
| UseVents | Allows you to use vents even if you are not an impostor or an engineer | Toggle | Off
| KickVents | Forcefully kicks all players from vents | Button |
| WalkInVents | Allows you to move and interact with the game even though you are inside of a vent<br>This gives you a sort of invisibility until you disable the setting and leave the vent<br>(*Some activites such as killing will forcefully make you visible again*) | Toggle | Off

## 💤 Passive

These cheats are constantly running in the background and **cannot be disabled to avoid problems.**

| Cheat | Description | Type | Default|
|------------|-------------|------|--------|
| FreeCosmetics | Gives you access to all of the game's cosmetics for free, including:<br><br>- Hats<br>- Visors<br>- Skins<br>- Pets<br>- Nameplates<br>- Bundles<br>- Cosmicubes | Toggle | On |
| AvoidPenalties | Removes the penalty you receive when disconnecting from games early | Toggle | On |
| UnlockFeatures | Unlocks many of the game's special features automatically, including:<br><br>- Freechat<br>- Friend list<br>- Custom name<br>- Online gameplay | Toggle | On |

## 📃 Config

You can change all of the following settings in `BepInEx/config/MalumMenu.cfg`

| Config          | Description                                                                                                                                                         | Type   | Default |
|-----------------|---------------------------------------------------------------------------------------------------------------------------------------------------------------------|--------|---------|
| GuestMode.GuestMode | When enabled, a new guest account will generate every time you start the game<br><br>Allows you to bypass account bans and PUID detection | Boolean | false |
| GuestMode.FriendName | The username that will be used when setting a friend code for your guest account<br><br>**IMPORTANT**: <br>- Can only be used with GuestMode<br>- Needs to be ≤ 10 characters<br>- Cannot include special characters/discriminator (#1234) | String |  |
| GUI.Keybind | Specifies the keyboard key that toggles the GUI on/off<br><br>**IMPORTANT**: You may only use keycodes from this [list](https://docs.unity3d.com/Packages/com.unity.tiny@0.16/api/Unity.Tiny.Input.KeyCode.html) | String | Delete |
| GUI.Color | Sets the color of MalumMenu's GUI using HTML color codes | String | |
| Privacy.HideDeviceId | When enabled, it will hide your unique deviceId from Among Us<br><br>Could **potentially** help bypass hardware bans in the future | Boolean | true |
| Privacy.NoTelemetry | When enabled, it will stop Among Us from collecting analytics of your games using Unity Analytics and sending them to Innersloth | Boolean | true |
| Spoofing.Level | Sets a custom player level to display to others in online games, masking your real level<br><br>**IMPORTANT**: Only integers between 0 and 4294967295 are valid. Decimal values are not accepted | String | |
| Spoofing.Platform | Sets a different gaming platform in online lobbies to disguise your actual platform<br><br>**IMPORTANT**: You may only use platform names from this [list](https://skeld.js.org/enums/constant.Platform.html) | String | |

## Other relevant features of MalumMenu:

- MalumMenu has a simple **GUI** that is easy to navigate and can be toggled using the **DELETE** key on your keyboard
- [**TEMPORARILY BROKEN**] MalumMenu comes with **custom announcements** that it will automatically fetch online at launch.
