using MalumMenu.Utilities;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UniverseLib;
using UniverseLib.UI;
using UniverseLib.UI.Models;
using UniverseLib.UI.Panels;

namespace MalumMenu;

public class MalumPanel : UniverseLib.UI.Panels.PanelBase
{
    public MalumPanel(UIBase owner) : base(owner) { }

    public override string Name => "MalumMenu v" + MalumMenu.malumVersion;
    public override int MinWidth => int.MinValue;
    public override int MinHeight => int.MinValue;
    public override Vector2 DefaultAnchorMin => new(0.25f, 0.25f);
    public override Vector2 DefaultAnchorMax => new(0.75f, 0.75f);
    public override bool CanDragAndResize => true;

    protected override PanelDragger CreatePanelDragger()
    {
        return new MalumPanelDragger(this);
    }

    protected override void ConstructPanelContent()
    {
    }
}

public class MalumPanelManager
{
    public MalumPanel malumPanel;
    public Text mousePosition;
    public ButtonRef murderAll;
    public Toggle speedBoost;
    bool isMenuActive;

    public delegate void ToggleClickHandler(bool value, ref bool value2);

    public void RegisterPanel(MalumPanel panel)
    {
        malumPanel = panel;
        GameObject mainContentRoot = malumPanel.ContentRoot;

        malumPanel.TitleBar.transform.Find("CloseHolder").gameObject.SetActive(false);

        malumPanel.Rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 260);
        malumPanel.Rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 470);
        malumPanel.Dragger.OnEndResize();

        mousePosition = UIFactory.CreateLabel(mainContentRoot, "mousePos", "Mouse position:");

        murderAll = UIFactory.CreateButton(mainContentRoot, "murderAll", "Murder All");
        UIFactory.SetLayoutElement(murderAll.Component.gameObject, minWidth: 110, minHeight: 25);
        murderAll.OnClick += () => { MalumCheats.murderAllCheat(); };

        System.Action<bool> speedBoostAction = (value => ToggleClicked(value, ref CheatToggles.speedBoost));
        speedBoost = UiUtils.CreateToggle(mainContentRoot, "speedBoost", "Speed Boost", speedBoostAction);
    }

    public void Update()
    {
        if (malumPanel is null) return;
        malumPanel.Enabled = isMenuActive;

        if (Input.GetKeyDown(Utils.stringToKeycode(MalumMenu.menuKeybind.Value)))
        {
            isMenuActive = !isMenuActive;
            Vector2 mousePosition = Input.mousePosition;
            malumPanel.Rect.position = new Vector2(mousePosition.x, mousePosition.y);
        }

        mousePosition.text = $"Mouse position: x:{Input.mousePosition.x} y:{Input.mousePosition.y}";
    }

    public void ToggleClicked(bool value, ref bool value2)
    {
        if (Utils.isShipBool(ref value2))
        {
            if (!Utils.isShip)
            {
                value2 = false;
                return;
            }
        }
        if (Utils.isPlayerBool(ref value2))
        {
            if (!Utils.isPlayer)
            {
                value2 = false;
                return;
            }
        }
        value2 = value;
    }
}