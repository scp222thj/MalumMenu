using System.Collections.Generic;
using UnityEngine;
using AmongUs.GameOptions;

namespace MalumMenu;

    public class LobbyInfoUI : MonoBehaviour
    {
        private GUIStyle _labelStyle;

        private void EnsureStyle()
        {
            if (_labelStyle == null)
            {
                _labelStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 16
                };
            }
        }

        private List<string> GetLobbyInfo()
        {
            var lines = new List<string>();
            
            if (GameManager.Instance == null || GameOptionsManager.Instance == null)
            {
                lines.Add("Join a lobby or create one to see lobby settings.");
                return lines;
            }

            var opts = GameOptionsManager.Instance.CurrentGameOptions;
            var logic = GameManager.Instance.LogicOptions;

            // Impostors
            lines.Add("—— Impostors ——");
            lines.Add("Impostor count: " + opts.NumImpostors);
            lines.Add("Kill cooldown: " + logic.GetKillCooldown() + "s");
            lines.Add("Impostor vision: " + opts.GetFloat(FloatOptionNames.ImpostorLightMod) + "x");
            var distance = logic.GetKillDistance();
            string killMode = distance switch
            {
                <= 1f => "Short",
                <= 1.8f => "Medium",
                <=2.5f => "Long",
                _ => "Now switch handles all values"
            };
            lines.Add("Kill distance: " + killMode);

            // Crewmates
            lines.Add("");
            lines.Add("—— Crewmates ——");
            lines.Add("Player speed: " + opts.GetFloat(FloatOptionNames.PlayerSpeedMod) + "x");
            lines.Add("Crew vision: " + opts.GetFloat(FloatOptionNames.CrewLightMod) + "x");

            // Meetings
            lines.Add("");
            lines.Add("—— Meetings ——");
            lines.Add("Emergency meetings: " + logic.GetNumEmergencyMeetings());
            lines.Add("Emergency cooldown: " + logic.GetEmergencyCooldown() + "s");
            lines.Add("Discussion time: " + opts.GetInt(Int32OptionNames.DiscussionTime) + "s");
            lines.Add("Voting time: " + opts.GetInt(Int32OptionNames.VotingTime) + "s");
            lines.Add("Anonymous votes: " + logic.GetAnonymousVotes());
            lines.Add("Confirm ejects: " + logic.GetConfirmImpostor());

            // Tasks
            lines.Add("");
            lines.Add("—— Tasks ——");
            int mode = opts.GetInt(Int32OptionNames.TaskBarMode);
            string taskBarMode = mode switch
            {
              0 => "Always",
              1 => "Meetings",
              2 => "Never",
              _ => "Now switch handles all values"
            };
            lines.Add("Task bar updates: " + taskBarMode);
            lines.Add("Common tasks: " + opts.GetInt(Int32OptionNames.NumCommonTasks));
            lines.Add("Long tasks: " + opts.GetInt(Int32OptionNames.NumLongTasks));
            lines.Add("Short tasks: " + opts.GetInt(Int32OptionNames.NumShortTasks));
            lines.Add("Visual tasks: " + logic.GetVisualTasks());

            // Role options
            lines.Add("");
            lines.Add("—— Scientist ——");
            lines.Add("Vitals cooldown: " + logic.GetRoleFloat(FloatOptionNames.ScientistCooldown) + "s");
            lines.Add("Battery duration: " + logic.GetRoleFloat(FloatOptionNames.ScientistBatteryCharge) + "s");

            lines.Add("");
            lines.Add("—— Guardian Angel ——");
            lines.Add("Protect cooldown: " + logic.GetRoleFloat(FloatOptionNames.GuardianAngelCooldown) + "s");
            lines.Add("Protect duration: " + logic.GetRoleFloat(FloatOptionNames.ProtectionDurationSeconds) + "s");
            lines.Add("Protect visible to impostors: " + logic.GetRoleBool(BoolOptionNames.ImpostorsCanSeeProtect));

            lines.Add("");
            lines.Add("—— Engineer ——");
            lines.Add("Vent cooldown: " + logic.GetRoleFloat(FloatOptionNames.EngineerCooldown) + "s");
            lines.Add("Max time in vents: " + logic.GetRoleFloat(FloatOptionNames.EngineerInVentMaxTime) + "s");
            
            lines.Add("");
            lines.Add("—— Noisemaker ——");
            lines.Add("Alert duration: " + logic.GetRoleFloat(FloatOptionNames.NoisemakerAlertDuration) + "s");
            lines.Add("Impostors get alert: " + logic.GetRoleBool(BoolOptionNames.NoisemakerImpostorAlert));

            lines.Add("");
            lines.Add("—— Tracker ——");
            lines.Add("Tracking cooldown: " + logic.GetRoleFloat(FloatOptionNames.TrackerCooldown) + "s");
            lines.Add("Tracking duration: " + logic.GetRoleFloat(FloatOptionNames.TrackerDuration) + "s");
            lines.Add("Tracking delay: " + logic.GetRoleFloat(FloatOptionNames.TrackerDelay) + "s");
            
            lines.Add("");
            lines.Add("—— Detective ——");
            lines.Add("Suspects per case: " + logic.GetRoleFloat(FloatOptionNames.DetectiveSuspectLimit));

            lines.Add("");
            lines.Add("—— Shapeshifter ——");
            lines.Add("Shapeshift cooldown: " + logic.GetRoleFloat(FloatOptionNames.ShapeshifterCooldown) + "s");
            lines.Add("Shapeshift duration: " + logic.GetRoleFloat(FloatOptionNames.ShapeshifterDuration) + "s");
            lines.Add("Leave shapeshifting evidence: " + logic.GetRoleBool(BoolOptionNames.ShapeshifterLeaveSkin));

            lines.Add("");
            lines.Add("—— Phantom ——");
            lines.Add("Vanish cooldown: " + logic.GetRoleFloat(FloatOptionNames.PhantomCooldown) + "s");
            lines.Add("Vanish duration: " + logic.GetRoleFloat(FloatOptionNames.PhantomDuration) + "s");
            
            lines.Add("");
            lines.Add("—— Viper ——");
            lines.Add("Dissolve time: " + logic.GetRoleFloat(FloatOptionNames.ViperDissolveTime) + "s");

            // Role amounts
            lines.Add("");
            lines.Add("—— Role amounts ——");
            lines.Add("Scientist: " + opts.RoleOptions.GetNumPerGame(RoleTypes.Scientist) + " (" + opts.RoleOptions.GetChancePerGame(RoleTypes.Scientist) + "%)");
            lines.Add("Guardian Angel: " + opts.RoleOptions.GetNumPerGame(RoleTypes.GuardianAngel) + " (" + opts.RoleOptions.GetChancePerGame(RoleTypes.GuardianAngel) + "%)");
            lines.Add("Engineer: " + opts.RoleOptions.GetNumPerGame(RoleTypes.Engineer) + " (" + opts.RoleOptions.GetChancePerGame(RoleTypes.Engineer) + "%)");
            lines.Add("Noisemaker: " + opts.RoleOptions.GetNumPerGame(RoleTypes.Noisemaker) + " (" + opts.RoleOptions.GetChancePerGame(RoleTypes.Noisemaker) + "%)");
            lines.Add("Tracker: " + opts.RoleOptions.GetNumPerGame(RoleTypes.Tracker) + " (" + opts.RoleOptions.GetChancePerGame(RoleTypes.Tracker) + "%)");
            lines.Add("Detective: " + opts.RoleOptions.GetNumPerGame(RoleTypes.Detective) + " (" + opts.RoleOptions.GetChancePerGame(RoleTypes.Detective) + "%)");
            lines.Add("Shapeshifter: " + opts.RoleOptions.GetNumPerGame(RoleTypes.Shapeshifter) + " (" + opts.RoleOptions.GetChancePerGame(RoleTypes.Shapeshifter) + "%)");
            lines.Add("Phantom: " + opts.RoleOptions.GetNumPerGame(RoleTypes.Phantom) + " (" + opts.RoleOptions.GetChancePerGame(RoleTypes.Phantom) + "%)");
            lines.Add("Viper: " + opts.RoleOptions.GetNumPerGame(RoleTypes.Viper) + " (" + opts.RoleOptions.GetChancePerGame(RoleTypes.Viper) + "%)");

            return lines;
        }

        public void DrawLobbyInfo()
        {
            EnsureStyle();

            GUILayout.BeginVertical();
            
            List<string> info = GetLobbyInfo();

            foreach (string line in info)
                GUILayout.Label(line, _labelStyle);
            

            GUILayout.EndVertical(); 
        }
    }