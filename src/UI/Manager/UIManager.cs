using BepInEx.Unity.IL2CPP;
using MalumMenu;

public sealed class UIManager
{
    public MenuUI Menu { get; private set; }
    public ConsoleUI Console { get; private set; }
    public RolesUI Roles { get; private set; }
    public DoorsUI Doors { get; private set; }
    public TasksUI Tasks { get; private set; }
    public ProtectUI Protect { get; private set; }

    public void Initialize(BasePlugin plugin)
    {
        Menu = plugin.AddComponent<MenuUI>();
        Console = plugin.AddComponent<ConsoleUI>();
        Roles = plugin.AddComponent<RolesUI>();
        Doors = plugin.AddComponent<DoorsUI>();
        Tasks = plugin.AddComponent<TasksUI>();
        Protect = plugin.AddComponent<ProtectUI>();
    }
}
