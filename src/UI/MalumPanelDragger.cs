using UnityEngine;
using UniverseLib.UI.Panels;

//BASE: https://github.com/yukieiji/UnityExplorer/blob/master/src/UI/Panels/UEPanelDragger.cs
public class MalumPanelDragger : PanelDragger
{
    public MalumPanelDragger(PanelBase uiPanel) : base(uiPanel) { }

    protected override bool MouseInResizeArea(Vector2 pos)
    {
        return false; //This forces panel to be not resizeable
    }
}