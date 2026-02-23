using System.Reflection;
using HarmonyLib;

namespace RecenterFix.HarmonyPatches;

internal static class HarmonyHelper
{
    private static Harmony _harmony;
    private static bool _initialized;

    private static void LazyInit()
    {
        if (_initialized) return;
        _harmony = new Harmony(Plugin.HarmonyId);
        _initialized = true;
    }

    internal static void ApplyPatches()
    {
        LazyInit();
        _harmony.PatchAll(Assembly.GetExecutingAssembly());
    }

    internal static void RemovePatches()
    {
        if (!_initialized) return;
        _harmony.UnpatchSelf();
    }
}