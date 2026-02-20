using HarmonyLib;
using Zenject;

namespace RecenterFix.Patches;

[HarmonyPatch(typeof(MainSettingsMenuViewControllersInstaller), nameof(MainSettingsMenuViewControllersInstaller.InstallBindings))]
internal static class MenuInstallerPatch
{
    private static void Postfix(MainSettingsMenuViewControllersInstaller __instance)
    {
        var container = Traverse.Create(__instance).Property<DiContainer>("Container").Value;
        if (container == null) return;

        Plugin.SettingsManager = container.TryResolve<SettingsManager>();
        Plugin.SettingsApplicator = container.TryResolve<SettingsApplicatorSO>();
    }
}