using HarmonyLib;
using UnityEngine;

namespace RecenterFix.Patches;

[HarmonyPatch(typeof(VRCenterAdjust), "Start")]
internal static class VRCenterAdjustStartPatch
{
    private static void Postfix(VRCenterAdjust __instance)
    {
        Camera2Compatibility.SetRoomOrigin(__instance.transform);
    }
}

[HarmonyPatch(typeof(VRCenterAdjust), "SetRoomTransformOffset")]
internal static class VRCenterAdjustPatch
{
    private static void Postfix(VRCenterAdjust __instance)
    {
        float correction = PluginConfig.Instance?.TotalCorrectionY ?? 0f;
        if (Mathf.Abs(correction) < 0.001f) return;

        var pos = __instance.transform.localPosition;
        pos.y += correction;
        __instance.transform.localPosition = pos;
    }
}