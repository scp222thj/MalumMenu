namespace MalumMenu;
public static class MalumSpoof
{
    public static void SpoofLevel()
    {
        if (!SpoofingService.EnableLevelSpoof) return;

        if (!string.IsNullOrEmpty(MalumMenu.spoofLevel.Value) &&
            uint.TryParse(MalumMenu.spoofLevel.Value, out uint parsedLevel))
        {
            SpoofingService.SpoofedLevel = parsedLevel;
        }

        SpoofingService.ApplyLevelSpoof();
    }
}
