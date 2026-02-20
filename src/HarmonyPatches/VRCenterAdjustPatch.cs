using HarmonyLib;
using UnityEngine;

namespace RecenterFix.Patches;

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