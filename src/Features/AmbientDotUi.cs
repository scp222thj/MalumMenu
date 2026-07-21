using UnityEngine;
using UnityEngine.UI;

namespace MalumMenu;

internal sealed class AmbientDotUi
{
    public RectTransform Rect;
    public Image Image;
    public Vector2 BasePos;
    public float Phase;
    public float Speed;
    public float Amp;

    public AmbientDotUi(RectTransform rect, Image image, Vector2 basePos, float phase, float speed, float amp)
    {
        Rect = rect;
        Image = image;
        BasePos = basePos;
        Phase = phase;
        Speed = speed;
        Amp = amp;
    }
}
