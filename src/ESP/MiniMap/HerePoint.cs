using UnityEngine;

namespace MalumMenu;
public class HerePoint // a utility to create the point of a player
{
    public PlayerControl player;
    public SpriteRenderer sprite;

    public HerePoint(PlayerControl player, SpriteRenderer sprite)
    {
        this.player = player;
        this.sprite = sprite;
    }
}
