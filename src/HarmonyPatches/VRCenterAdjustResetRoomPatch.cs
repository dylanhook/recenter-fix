using HarmonyLib;
using RecenterFix.Compatibility;

#pragma warning disable IDE0051

namespace RecenterFix.HarmonyPatches;

[HarmonyPatch(typeof(VRCenterAdjust), "ResetRoom")]
[HarmonyAfter("Kinsi55.BeatSaber.Cam2")]
internal static class VRCenterAdjustResetRoomPatch
{
    private static void Postfix(VRCenterAdjust __instance) =>
        Camera2Bridge.NotifyRoomOffsetChanged(
            __instance.transform.localPosition,
            __instance.transform.localRotation);
}