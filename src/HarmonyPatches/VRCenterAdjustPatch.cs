using HarmonyLib;
using RecenterFix.Compatibility;
using UnityEngine;

#pragma warning disable IDE0051

namespace RecenterFix.HarmonyPatches;

[HarmonyPatch(typeof(VRCenterAdjust), "SetRoomTransformOffset")]
[HarmonyAfter("Kinsi55.BeatSaber.Cam2")]
internal static class VRCenterAdjustPatch
{
    private static void Postfix(VRCenterAdjust __instance, SettingsManager ____settingsManager)
    {
        if (PluginConfig.Instance == null) return;

        float correction = PluginConfig.Instance.FloorCalibrationY
                         + PluginConfig.Instance.RecenterCompensationY;

        if (Mathf.Abs(correction) < 0.001f)
        {
            Camera2Bridge.NotifyRoomOffsetChanged(
                ____settingsManager.settings.room.center,
                __instance.transform.localRotation);
            return;
        }

        var pos = __instance.transform.localPosition;
        pos.y += correction;
        __instance.transform.localPosition = pos;

        Camera2Bridge.NotifyRoomOffsetChanged(pos, __instance.transform.localRotation);
    }
}