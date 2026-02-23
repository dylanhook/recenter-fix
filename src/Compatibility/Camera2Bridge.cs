using System;
using System.Reflection;
using UnityEngine;

namespace RecenterFix.Compatibility;

internal static class Camera2Bridge
{
    private const string HookTypeName = "Camera2.HarmonyPatches.HookRoomAdjust, Camera2";
    private const string MethodName = "ApplyCustom";

    private static bool _resolved;
    private static MethodInfo _applyCustomMethod;
    private static readonly object[] _invokeArgs = [null, null];

    internal static void NotifyRoomOffsetChanged(Vector3 position, Quaternion rotation)
    {
        if (!_resolved) Resolve();
        if (_applyCustomMethod == null) return;

        _invokeArgs[0] = position;
        _invokeArgs[1] = rotation;

        try
        {
            _applyCustomMethod.Invoke(null, _invokeArgs);
        }
        catch (Exception e)
        {
            Plugin.Log?.Warn($"Camera2 bridge call failed: {e.Message}");
        }
    }

    private static void Resolve()
    {
        _resolved = true;

        try
        {
            var hookType = Type.GetType(HookTypeName);
            if (hookType == null) return;

            _applyCustomMethod = hookType.GetMethod(
                MethodName,
                BindingFlags.Public | BindingFlags.Static,
                null,
                new[] { typeof(Vector3), typeof(Quaternion) },
                null);

            if (_applyCustomMethod != null)
                Plugin.Log?.Info("Camera2 bridge: compatibility active");
            else
                Plugin.Log?.Warn("Camera2 bridge: HookRoomAdjust found but ApplyCustom missing");
        }
        catch (Exception e)
        {
            Plugin.Log?.Warn($"Camera2 bridge init failed: {e.Message}");
        }
    }
}