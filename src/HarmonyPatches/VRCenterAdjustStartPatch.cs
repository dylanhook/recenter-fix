using HarmonyLib;
using RecenterFix.Compatibility;

#pragma warning disable IDE0051

namespace RecenterFix.HarmonyPatches;

[HarmonyPatch(typeof(VRCenterAdjust), "Start")]
[HarmonyAfter("Kinsi55.BeatSaber.Cam2")]
internal static class VRCenterAdjustStartPatch
{
    private static void Postfix(
        VRCenterAdjust __instance,
        SettingsManager ____settingsManager,
        SettingsApplicatorSO ____settingsApplicator)
    {
        Plugin.SettingsManager = ____settingsManager;

        if (____settingsApplicator != null)
            Plugin.SettingsApplicator = ____settingsApplicator;

        Camera2Bridge.NotifyRoomOffsetChanged(
            __instance.transform.localPosition,
            __instance.transform.localRotation);
    }
}