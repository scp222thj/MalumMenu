# 📋 Features

## 👱 Player

| Cheat | Description | Type | Default|
|------------|-------------|------|--------|
| NoClip     | Allows you to walk through walls like a ghost | Toggle | Off
| SpeedBoost | Doubles your player's speed | Toggle | Off |
| ReportBody | Allows you to make a player report their own dead body and start a meeting | Menu |

#### Murder

| Cheat | Description | Type | 
|------------|-------------|------|
| MurderPlayer | Murders the targeted player | Menu |
| MurderAll | Murders all the players | Button |

#### Teleport

| Cheat | Description | Type | Default |
|------------|-------------|------|----|
| to Cursor | Allows you to teleport by right-clicking<br> Works best when used along with ZoomOut | Toggle | Off |
| to Player | Allows you to pick a player to teleport to | Menu |

## 👁️ ESP

MalumMenu's ESP cheats are completely client-side, and thus undetectable by anticheat

| Cheat | Description | Type | Default |
|------------|-------------|------|--------|
| SeeGhosts | Allows you to see ghosts, protections, and ghost chat even if you are alive | Toggle | Off
| FullBright | Removes all shadows, allowing you to see during blackouts and even through walls<br>Also, lets you see through spore clouds in the Fungle Jungle | Toggle | Off |
| RevealVotes | Lets you see colored votes even if votes are set to anonymous | Toggle | Off |
| AlwaysChat | Keeps the chat icon always enabled, allowing you to chat at any time, even while you're not in a meeting or the lobby | Toggle | Off |

#### NameTags
| Cheat | Description | Type | Default |
|------------|-------------|------|--------|
| SeeRoles | Reveals the roles of all players through the color of their names | Toggle | Off
| VentVision | Allows you to see the names of players inside vents | Toggle | Off |

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

| Cheat | Description | Type | 
|------------|-------------|------|
| ChangeRole | Allows you to change the role of any player | Menu |

#### Crewmate

| Cheat | Description | Type |
|------------|-------------|------|
| CompleteMyTasks | Marks all your tasks as completed | Button |

#### Impostor

| Cheat | Description | Type | Default |
|------------|-------------|------|----|
| KillAnyone | Allows you to kill anyone, including ghosts, impostors, players in vents, etc. | Toggle | Off |
| NoKillCooldown | Removes the cooldown between kills | Toggle | Off |
| KillReach | Allows you to kill the nearest player, regardless of obstacles in between | Toggle | Off |
| ImpostorTasks | Allows you to do tasks as impostor | Toggle | Off |

#### Shapeshifter

| Cheat | Description | Type | Default |
|------------|-------------|------|----|
| NoSsAnimation | Removes the animation when shapeshifting | Toggle | Off |
| EndlessSsDuration | Allows you to stay shifted forever | Toggle | Off |

#### Engineer

| Cheat | Description | Type | Default |
|------------|-------------|------|----|
| EndlessVentTime | Allows you to stay in vents forever | Toggle | Off |
| NoVentCooldown | Allows you to vent like an impostor without cooldown | Toggle | Off |

#### Scientist

| Cheat | Description | Type | Default |
|------------|-------------|------|----|
| EndlessBattery | Allows you to watch the vitals forever | Toggle | Off |
| NoVitalsCooldown | Allows you to watch the vitals without cooldown | Toggle | Off |

## 🚀 Ship

| Cheat | Description | Type | Default |
|------------|-------------|------|----|
| UnfixableLights | Disables lights completely (they cannot be fixed manually by players)<br>You can enable them again by clicking the button. | Button |
| CloseMeeting | Forcefully closes the meeting window (only for you), allowing you to move and interact with the game during meetings | Button |

#### Sabotage

MalumMenu's Sabotage cheats work even if you aren't impostor and are subject to no cooldown.

Moreover, different sabotages can be enabled at the same time, and they even work during meetings.

| Cheat | Description | Type | 
|------------|-------------|------|
| Reactor | Enables/Disables Reactor sabotage. | Button |
| Oxygen | Enables/Disables Oxygen sabotage. | Button |
| Lights | Enables/Disables Lights sabotage. | Button |
| Comms | Enables/Disables Communications sabotage. | Button |
| Doors | Locks all doors on the ship. | Button |
| MushroomMixup | Enables Mushroom Mixup sabotage on Fungle map. | Button |

#### Vents

| Cheat | Description | Type | Default|
|------------|-------------|------|--------|
| UseVents | Allows you to use vents even if you are not an impostor or an engineer | Toggle | Off
| KickVents | Forcefully kicks all players from vents | Button |
| WalkInVents | Allows you to move and interact with the game even though you are inside of a vent<br>This gives you a sort of invisibility until you disable the setting and leave the vent<br>(*Some activites such as killing will forcefully make you visible again*) | Toggle | Off

## 🕵️ Spoofing

| Cheat | Description | Type |
|------------|-------------|------|
| RandomFriendCode | Hides your real friend code from people by spoofing random fake ones<br>(*While using a spoofed friend code, players won't be able to send you friend requests*) | Toggle | Off |

#### Config

| Cheat | Description | Type | Default |
|------------|-------------|------|--------|
| Spoofed FriendCode | A custom friend code that will be used in online games.<br>(*While using a spoofed friend code, players won't be able to send you friend requests*)<br>Config file: `BepInEx/config/MalumMenu.cfg` | Config | Empty |
| Spoofed Level | A custom level that will be displayed to other players in meetings | Config | Empty |

## 💤 Passive

These cheats are constantly running in the background and **cannot be disabled to avoid problems.**

| Cheat | Description | Type | Default|
|------------|-------------|------|--------|
| FreeCosmetics | Gives you access to all of the game's cosmetics for free, including:<br><br>- Hats<br>- Visors<br>- Skins<br>- Pets<br>- Nameplates<br>- Bundles<br>- Cosmicubes | Toggle | On |
| AvoidPenalties | Removes the penalty you receive when disconnecting from games early | Toggle | On |
| UnlockFeatures | Unlocks many of the game's special features automatically, including:<br><br>- Freechat<br>- Friend list<br>- Custom name<br>- Online gameplay | Toggle | On |


## 📃 Config

You can change all of the following configs in `BepInEx/config/MalumMenu.cfg`

| Config | Description | Type | Default|
|------------|-------------|------|--------|
| GUIKeybind | The keyboard key used to toggle the GUI on and off | String | Delete |
| SpoofedFriendCode | A custom friend code that will be used in online games<br><br>**IMPORTANT**: When using a spoofed friend code, players won't be able to send you friend requests | String |  |
| SpoofedPuid | A custom PUID that will be used in online games<br><br>**IMPORTANT**: Only valid, active PUIDs will let you connect to lobbies | String |  |


## Other relevant features of MalumMenu:

- MalumMenu has a simple **GUI** that is easy to navigate and can be toggled using the **DELETE** key on your keyboard
- MalumMenu comes with **custom announcements** that it will automatically fetch online at launch.
