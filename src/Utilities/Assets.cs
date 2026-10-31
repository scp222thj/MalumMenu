using UnityEngine;

namespace MalumMenu;

// Lazy-loaded Texture2D asset
public class TextureAsset(string path, HideFlags hideFlags = HideFlags.HideAndDontSave)
{
    private Texture2D LoadedAsset { get; set; }

    private Texture2D LoadAsset()
    {
        if (LoadedAsset != null)
        {
            return LoadedAsset;
        }

        LoadedAsset = Utils.LoadTextureFromResources(path);
        LoadedAsset.hideFlags = hideFlags;
        return LoadedAsset;
    }

    public static implicit operator Texture2D(TextureAsset asset)
    {
        return asset.LoadAsset();
    }
}

public abstract class Assets
{
    private const string BasePath = "MalumMenu.Assets";

    // Icons used for the tab selector buttons in the main window
    public abstract class TabIcons
    {
        public static readonly Texture2D Movement = new TextureAsset($"{BasePath}.movement.png");
        public static readonly Texture2D ESP = new TextureAsset($"{BasePath}.esp.png");
        public static readonly Texture2D Roles = new TextureAsset($"{BasePath}.roles.png");
        public static readonly Texture2D Ship = new TextureAsset($"{BasePath}.ship.png");
        public static readonly Texture2D Chat = new TextureAsset($"{BasePath}.chat.png");
        public static readonly Texture2D Animations = new TextureAsset($"{BasePath}.animations.png");
        public static readonly Texture2D Console = new TextureAsset($"{BasePath}.console.png");
        public static readonly Texture2D HostOnly = new TextureAsset($"{BasePath}.host-only.png");
        public static readonly Texture2D Passive = new TextureAsset($"{BasePath}.passive.png");
        public static readonly Texture2D Modes = new TextureAsset($"{BasePath}.modes.png");
        public static readonly Texture2D Config = new TextureAsset($"{BasePath}.config.png");
        public static readonly Texture2D Overload = new TextureAsset($"{BasePath}.overload.png");
    }
}
