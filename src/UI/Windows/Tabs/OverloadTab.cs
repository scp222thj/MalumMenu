using System;
using UnityEngine;

namespace MalumMenu;

public class OverloadTab : ITab
{
    public string name => "Overload";

    private GUIStyle _sliderSubtitle;
    private int _maxStrength = 100000;
    private float _maxCooldown = 1f;
    private float _fpsEstimate = 0f;
    private float _rawCooldown;
    private float _rawStrength;

    public void Draw()
    {
        InitStyles();

        GUILayout.BeginVertical(GUILayout.Width(MenuUI.windowWidth * 0.425f));

        DrawGeneral();

        GUILayout.Space(15);

        DrawSettingsToggle();

        GUILayout.EndVertical();

        if (CheatToggles.showOverloadSettings)
        {
            GUILayout.BeginVertical(GUI.skin.box, GUILayout.Width(MenuUI.windowWidth * 0.75f));

            DrawSettingsSection();

            GUILayout.EndVertical();
        }
    }

    private void InitStyles()
    {
        if (_sliderSubtitle == null)
        {
            _sliderSubtitle = new(GUIStylePreset.TabSubtitle)
            {
                fontStyle = FontStyle.Normal
            };
        }
    }

    private void DrawGeneral()
    {
        CheatToggles.showOverload = GUILayout.Toggle(CheatToggles.showOverload, " Show Overload Menu");
    }

    private void DrawSettingsToggle()
    {
        GUILayout.Label("Settings", GUIStylePreset.TabSubtitle);

        CheatToggles.showOverloadSettings = GUILayout.Toggle(CheatToggles.showOverloadSettings, " Show Overload Settings");
    }

    private void DrawSettingsSection()
    {
        GUILayout.Space(15);

        GUILayout.BeginHorizontal();

        GUILayout.Space(10);

        GUILayout.BeginVertical();

        GUILayout.BeginHorizontal();

        CheatToggles.olAutoAdapt = GUILayout.Toggle(CheatToggles.olAutoAdapt, " Auto Adapt");

        int ping = Utils.GetPing();
        string pingStr = $"PING : {ping} ms";
        GUILayout.Label(Utils.GetColoredPingText(pingStr, ping));

        int strength = OverloadHandler.strength;
        float cooldown = OverloadHandler.cooldown;

        float numExecutionsPerSec;
        string extraStr = "";

        if (cooldown > Time.unscaledDeltaTime) // numExecutionsPerSec would be under FPS
        {
            numExecutionsPerSec = 1f / cooldown;
        }
        else // numExecutionsPerSec would be over FPS
        {
            // FPS fluctuates too often so only update after significant variation (> 5)

            float fps = Utils.GetFps();
            if (Math.Abs(fps - _fpsEstimate) > 5f)
            {
                _fpsEstimate = fps;
            }

            numExecutionsPerSec = (int)_fpsEstimate; // numExecutionsPerSec is capped by FPS regardless of cooldown
            extraStr = " (FPS Cap)";
        }

        int numTargetsPerSec = OverloadUI.currentTargets.Count <= numExecutionsPerSec ? OverloadUI.currentTargets.Count : (int)numExecutionsPerSec; // numTargetsPerSec is capped by numExecutionsPerSec

        int rpcPerTarget = numTargetsPerSec > 0 ? (int)(strength * numExecutionsPerSec / numTargetsPerSec) :
                                            (int)(strength * numExecutionsPerSec);

        string rpcStr = CheatToggles.olShowRpcTotal
                        ? $"{rpcPerTarget*Math.Max(1, numTargetsPerSec)}"
                        : $"{rpcPerTarget}x{numTargetsPerSec}";

        CheatToggles.olShowRpcTotal = GUILayout.Toggle(CheatToggles.olShowRpcTotal, $" RPC/s : {rpcStr}{extraStr}");

        GUILayout.EndHorizontal();

        GUILayout.Space(15);

        DrawSettingsSliders();

        GUILayout.EndVertical();

        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        GUILayout.BeginHorizontal();

        GUILayout.Space(10);

        GUILayout.BeginVertical(GUILayout.Width(MenuUI.windowWidth * 0.35f));

        GUILayout.Label("General", GUIStylePreset.TabSubtitle);

        CheatToggles.olAutoStart = GUILayout.Toggle(CheatToggles.olAutoStart, " Auto Start when Ready");

        CheatToggles.olAutoStop = GUILayout.Toggle(CheatToggles.olAutoStop, " Auto Stop when Done");

        CheatToggles.olLockTargets = GUILayout.Toggle(CheatToggles.olLockTargets, " Lock Targets on Start");

        CheatToggles.olKillSwitch = GUILayout.Toggle(CheatToggles.olKillSwitch, " Kill Switch on Lag");

        if (CheatToggles.olKillSwitch)
        {
            Color standardBackgroundColor = GUI.backgroundColor;
            GUI.backgroundColor = Color.red;

            bool isPressed = GUILayout.Button($"{OverloadUI.killSwitchThreshold} ms", GUILayout.Width(70f));
            if (isPressed)
            {
                if (OverloadUI.killSwitchThreshold >= 3000) // Max KS = 3000 ms
                {
                    OverloadUI.killSwitchThreshold = 500; // Min KS = 500 ms
                }
                else
                {
                    OverloadUI.killSwitchThreshold = OverloadUI.killSwitchThreshold + 500; // Increment by 500 ms steps
                }
            }

            GUI.backgroundColor = standardBackgroundColor;
        }

        GUILayout.EndVertical();

        GUILayout.BeginVertical();

        GUILayout.Label("Logs", GUIStylePreset.TabSubtitle);

        CheatToggles.olLogStartStop = GUILayout.Toggle(CheatToggles.olLogStartStop, " Log START and STOP");

        CheatToggles.olLogAddRemove = GUILayout.Toggle(CheatToggles.olLogAddRemove, " Log ADD and REMOVE");

        CheatToggles.olLogAttack = GUILayout.Toggle(CheatToggles.olLogAttack, " Log Attack");

        CheatToggles.olLogDisconnect = GUILayout.Toggle(CheatToggles.olLogDisconnect, " Log Disconnect");

        CheatToggles.olVerboseLogs = GUILayout.Toggle(CheatToggles.olVerboseLogs, " Verbose Attack Logs");

        CheatToggles.olAutoClear = GUILayout.Toggle(CheatToggles.olAutoClear, " Auto Clear on Start");

        GUILayout.EndVertical();

        GUILayout.EndHorizontal();

        GUILayout.Space(15);
    }

    private void DrawSettingsSliders()
    {
        GUILayout.Label($"Strength : {_rawStrength}", _sliderSubtitle);

        GUILayout.Space(1);

        GUILayout.BeginHorizontal();

        float inputStrength = GUILayout.HorizontalSlider(_rawStrength, 1, _maxStrength, GUILayout.Width(350f));

        if (inputStrength != _rawStrength)
        {
            CheatToggles.olAutoAdapt = false; // Disable AutoAdapt if user does manual input
            _rawStrength = inputStrength;
        }

        GUILayout.Space(5);

        string maxStrengthStr = _maxStrength % 1000 == 0 ? $"{_maxStrength / 1000}K" : $"{_maxStrength}";
        bool isPressedMaxStrength = GUILayout.Button(maxStrengthStr, GUILayout.Width(51f));

        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        GUILayout.Label($"Cooldown : {_rawCooldown:F2}", _sliderSubtitle);

        GUILayout.Space(1);

        GUILayout.BeginHorizontal();

        float inputCooldown = GUILayout.HorizontalSlider(_rawCooldown, 0f, _maxCooldown, GUILayout.Width(350f));

        if (inputCooldown != _rawCooldown)
        {
            CheatToggles.olAutoAdapt = false; // Disable AutoAdapt if user does manual input
            _rawCooldown = inputCooldown;
        }

        GUILayout.Space(5);

        bool isPressedMaxCooldown = GUILayout.Button($"{_maxCooldown:F0}", GUILayout.Width(51f));

        GUILayout.EndHorizontal();

        if (!CheatToggles.olAutoAdapt)
        {
            float strengthStep = _maxStrength / 100f; // Slider steps are 1/100 of max strength
            int clampStrength = Mathf.RoundToInt(Mathf.Clamp(Mathf.Round(_rawStrength / strengthStep) * strengthStep, 1, _maxStrength));
            OverloadHandler.strength = clampStrength;

            float cooldownStep = _maxCooldown / 100f; // Slider steps are 1/100 of max cooldown
            float clampCooldown = Mathf.Round(_rawCooldown / cooldownStep) * cooldownStep;
            OverloadHandler.cooldown = clampCooldown;
        }

        // Adjust bounds so sliders will never be out of bounds

        while (_maxStrength < OverloadHandler.strength)
        {
            _maxStrength *= 10;
        }

        while (_maxCooldown < OverloadHandler.cooldown)
        {
            _maxCooldown *= 10;
        }

        if (isPressedMaxStrength)
        {
            if (_maxStrength >= 100000) // Max _maxStrength = 100K RPCs
            {
                CheatToggles.olAutoAdapt = false; // Disable AutoAdapt if user does manual input

                OverloadHandler.strength = Mathf.RoundToInt(OverloadHandler.strength/1000f); // Adjust value to account for max change (÷1000)

                _maxStrength = 100; // Min _maxStrength = 100 RPCs
            }
            else
            {
                CheatToggles.olAutoAdapt = false; // Disable AutoAdapt if user does manual input

                OverloadHandler.strength *= 10; // Adjust value to account for max change (x10)

                _maxStrength *= 10; // Increment by x10 steps
            }
        }

        if (isPressedMaxCooldown)
        {
            if (_maxCooldown >= 10f) // Max _maxCooldown = 10s
            {
                CheatToggles.olAutoAdapt = false; // Disable AutoAdapt if user does manual input

                OverloadHandler.cooldown /= 10f; // Adjust value to account for max change (÷10)

                _maxCooldown = 1f; // Min _maxCooldown = 1s
            }
            else
            {
                CheatToggles.olAutoAdapt = false; // Disable AutoAdapt if user does manual input

                OverloadHandler.cooldown *= 10; // Adjust value to account for max change (x10)

                _maxCooldown *= 10; // Increment by x10 steps
            }
        }

        // Update slider values to match actual values

        _rawStrength = OverloadHandler.strength;
        _rawCooldown = OverloadHandler.cooldown;
    }
}
