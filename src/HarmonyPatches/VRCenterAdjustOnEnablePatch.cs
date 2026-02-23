using System.Reflection;
using HarmonyLib;

#pragma warning disable IDE0051

namespace RecenterFix.HarmonyPatches;

[HarmonyPatch(typeof(VRCenterAdjust), "OnEnable")]
[HarmonyAfter("Kinsi55.BeatSaber.Cam2")]
internal static class VRCenterAdjustOnEnablePatch
{
    private static readonly MethodInfo SetRoomTransformOffsetMethod =
        typeof(VRCenterAdjust).GetMethod(
            "SetRoomTransformOffset",
            BindingFlags.NonPublic | BindingFlags.Instance);

    private static void Postfix(VRCenterAdjust __instance) =>
        SetRoomTransformOffsetMethod?.Invoke(__instance, null);
}