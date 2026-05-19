using System.Collections.Generic;
using UnityEngine;

namespace MalumMenu;

public class MeetingHistoryUI : MonoBehaviour
{
    public static int windowHeight = 360;
    public static int windowWidth  = 460;
    private Rect    _windowRect;
    private Vector2 _scrollPos;

    // --- Vote history ---
    public struct VoteEntry  { public string Voter; public string VotedFor; }
    public struct RoundRecord { public int Round; public List<VoteEntry> Votes; }
    public static readonly List<RoundRecord> History     = new();
    public static          int               RoundNumber = 0;
    private static MeetingHud _lastRecordedHud;

    private void Start()
    {
        _windowRect = new(
            Screen.width / 2f - windowWidth / 2f - 360f,
            Screen.height / 2f - windowHeight / 2f,
            windowWidth,
            windowHeight
        );
    }

    private void OnGUI()
    {
        if (MalumMenu.isPanicked || CheatToggles.streamerMode) return;

        // Meeting timer overlay — always shown when meeting is active and toggle is on
        if (CheatToggles.showMeetingTimer && Utils.isMeeting)
        {
            try
            {
                var hud = MeetingHud.Instance;
                float remaining = hud.state == MeetingHud.VoteStates.NotVoted || hud.state == MeetingHud.VoteStates.Voted
                    ? hud.discussionTimer
                    : 0f;
                GUI.color = Color.white;
                GUI.Label(new Rect(Screen.width / 2f - 50f, 8f, 100f, 26f), $"⏱ {Mathf.CeilToInt(remaining)}s");
            }
            catch { }
        }

        // Sub-window
        if (!CheatToggles.showMeetingHistory || !(MenuUI.isGUIActive || MalumMenu.menuKeepSubwindowsOpen.Value)) return;

        UIHelpers.ApplyUIColor();
        _windowRect = GUI.Window((int)WindowId.MeetingHistoryUI, _windowRect, (GUI.WindowFunction)DrawWindow, "Meeting History");
    }

    private void DrawWindow(int id)
    {
        GUILayout.BeginVertical();
        _scrollPos = GUILayout.BeginScrollView(_scrollPos, false, true);

        if (History.Count == 0)
        {
            GUILayout.Label("No meetings recorded yet.", GUIStylePreset.TabSubtitle);
        }
        else
        {
            foreach (var round in History)
            {
                GUILayout.Label($"Round {round.Round}", GUIStylePreset.TabSubtitle);
                foreach (var v in round.Votes)
                    GUILayout.Label($"  {v.Voter}  →  {v.VotedFor}");
                GUILayout.Space(4f);
            }
        }

        GUILayout.EndScrollView();
        GUILayout.Box("", GUIStylePreset.Separator, GUILayout.Height(1f), GUILayout.ExpandWidth(true));
        GUILayout.Space(1f);
        if (GUILayout.Button("Clear History", GUIStylePreset.NormalButton))
        {
            History.Clear();
            RoundNumber = 0;
        }
        GUILayout.EndVertical();
        GUI.DragWindow();
    }

    // Called from MeetingHudPatches when voting completes
    public static void RecordRound(MeetingHud hud)
    {
        if (hud == _lastRecordedHud) return;
        _lastRecordedHud = hud;
        RoundNumber++;
        var votes = new List<VoteEntry>();
        foreach (var ps in hud.playerStates)
        {
            if (ps == null) continue;
            var voter = GameData.Instance.GetPlayerById(ps.TargetPlayerId);
            if (voter == null || voter.Disconnected) continue;

            string votedFor;
            if      (ps.VotedFor == PlayerVoteArea.SkippedVote) votedFor = "Skip";
            else if (ps.VotedFor == PlayerVoteArea.HasNotVoted)  votedFor = "(no vote)";
            else if (ps.VotedFor < 0)                            continue; // DeadVote, MissedVote, or any negative sentinel
            else
            {
                var t = GameData.Instance.GetPlayerById((byte)ps.VotedFor);
                votedFor = t?.PlayerName ?? $"#{ps.VotedFor}";
            }
            votes.Add(new VoteEntry { Voter = voter.PlayerName, VotedFor = votedFor });
        }
        History.Add(new RoundRecord { Round = RoundNumber, Votes = votes });
    }
}
