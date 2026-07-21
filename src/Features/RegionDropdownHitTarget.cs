using InnerNet;
using UnityEngine;

namespace MalumMenu;

internal sealed class RegionDropdownHitTarget
{
    public RectTransform Rect;
    public IRegionInfo Region;

    public RegionDropdownHitTarget(RectTransform rect, IRegionInfo region)
    {
        Rect = rect;
        Region = region;
    }
}
