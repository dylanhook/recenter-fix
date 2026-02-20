using HarmonyLib;
using UnityEngine;
using UnityEngine.XR;

namespace RecenterFix.Patches;

[HarmonyPatch(typeof(UnityXRHelper), nameof(UnityXRHelper.GetNodePose))]
internal static class PoseOffsetPatch
{
    private static void Postfix(XRNode nodeType, ref Vector3 pos)
    {
        if (nodeType != XRNode.Head) return;

        float correction = PluginConfig.Instance?.TotalCorrectionY ?? 0f;
        if (Mathf.Abs(correction) > 0.001f)
        {
            pos.y += correction;
        }
    }
}