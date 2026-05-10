using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MalumMenu;

internal sealed class RadarTrail
{
    public readonly Vector2[] points;
    public readonly float[] times;
    public int start;
    public int count;
    public float nextRecordTime;
    public readonly List<Image> segments;
    public Color color;

    public RadarTrail(int maxWaypoints)
    {
        points = new Vector2[maxWaypoints];
        times = new float[maxWaypoints];
        segments = new List<Image>(maxWaypoints);
    }
}

internal sealed class RadarPlayerUi
{
    public byte id;
    public Image highlight;
    public Image dot;
    public RadarTrail trail;
}

