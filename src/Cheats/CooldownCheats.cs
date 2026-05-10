using System.Reflection;

namespace MalumMenu;

public static class CooldownCheats
{
    private struct FieldCache
    {
        public object system;
        public FieldInfo[] fields;
        public float[] prev;
        public float[] baseline;
    }

    private static FieldCache _sabotage;
    private static FieldCache _doors;

    public static void Process()
    {
        if (!Utils.isShip) return;

        ApplyToSabotageSystem();
        ApplyToDoorSystem();
    }

    private static void ApplyToSabotageSystem()
    {
        var sys = Utils.SabotageSystem;
        if (sys == null) return;

        var noCd = CheatToggles.noSabotageCooldown;
        var reduction = MalumMenu.sabotageCooldownReductionPercent.Value;
        ApplySystem(sys, ref _sabotage, noCd, reduction);
    }

    private static void ApplyToDoorSystem()
    {
        var ship = ShipStatus.Instance;
        if (ship == null) return;

        var sysObj = ship.Systems[SystemTypes.Doors];
        if (sysObj == null) return;

        var noCd = CheatToggles.noDoorCooldown;
        var reduction = MalumMenu.doorCooldownReductionPercent.Value;
        ApplySystem(sysObj, ref _doors, noCd, reduction);
    }

    private static void ApplySystem(object sys, ref FieldCache cache, bool noCooldown, float reductionPercent)
    {
        if (sys == null) return;

        if (cache.system == null || !ReferenceEquals(cache.system, sys))
        {
            BuildCache(sys, ref cache);
        }

        if (cache.fields == null || cache.fields.Length == 0) return;

        var reduction = reductionPercent;
        if (noCooldown) reduction = 100f;
        if (reduction < 0f) reduction = 0f;
        if (reduction > 100f) reduction = 100f;

        var scale = 1f - (reduction / 100f);

        for (var i = 0; i < cache.fields.Length; i++)
        {
            var field = cache.fields[i];
            var curObj = field.GetValue(sys);
            if (curObj == null) continue;

            var cur = (float)curObj;
            if (cur <= 0f)
            {
                cache.prev[i] = 0f;
                continue;
            }

            var prev = cache.prev[i];
            var reset = cur > prev + 0.05f;
            if (reset)
            {
                cache.baseline[i] = cur;
            }

            var baseline = cache.baseline[i];
            if (baseline <= 0f) baseline = cur;

            var newValue = cur;
            if (scale <= 0f)
            {
                newValue = 0f;
            }
            else if (scale < 0.999f)
            {
                var target = baseline * scale;
                if (target < 0f) target = 0f;
                if (newValue > target) newValue = target;
            }

            if (newValue != cur)
            {
                field.SetValue(sys, newValue);
            }

            cache.prev[i] = newValue;
        }
    }

    private static void BuildCache(object sys, ref FieldCache cache)
    {
        cache.system = sys;

        var t = sys.GetType();
        var all = t.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        var matches = new System.Collections.Generic.List<FieldInfo>(all.Length);

        for (var i = 0; i < all.Length; i++)
        {
            var f = all[i];
            if (f.FieldType != typeof(float)) continue;

            var name = f.Name;
            if (string.IsNullOrEmpty(name)) continue;
            var n = name.ToLowerInvariant();

            if (n.Contains("cool") || n.Contains("cd") || n.Contains("timer") || n.Contains("delay") || n.Contains("seconds"))
            {
                matches.Add(f);
            }
        }

        cache.fields = matches.ToArray();
        cache.prev = new float[cache.fields.Length];
        cache.baseline = new float[cache.fields.Length];

        for (var i = 0; i < cache.prev.Length; i++)
        {
            cache.prev[i] = 0f;
            cache.baseline[i] = 0f;
        }

        
    }
}
