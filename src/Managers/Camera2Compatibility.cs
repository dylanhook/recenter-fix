using System;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using IPA.Loader;

namespace RecenterFix;

internal static class Camera2Compatibility
{
    private static Type _cam2Type;
    private static MethodInfo _setOriginMethod;
    private static Transform _roomOrigin;

    public static void Init()
    {
        var camera2 = PluginManager.GetPluginFromId("Camera2");
        if (camera2 == null) return;

        try
        {
            var assembly = camera2.Assembly;
            _cam2Type = assembly.GetType("Camera2.Behaviours.Cam2");
            _setOriginMethod = _cam2Type?.GetMethod("SetOrigin", BindingFlags.Public | BindingFlags.Instance);

            if (_cam2Type != null)
            {
                var initMethod = _cam2Type.GetMethod("Init", BindingFlags.Public | BindingFlags.Instance);
                if (initMethod != null)
                {
                    Plugin.Harmony.Patch(initMethod, postfix: new HarmonyMethod(typeof(Camera2Compatibility), nameof(OnCameraInit)));
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"[RecenterFix] camera 2 linking error: {e.Message}");
        }
    }

    public static void SetRoomOrigin(Transform origin)
    {
        _roomOrigin = origin;
        if (_cam2Type == null || origin == null) return;

        var cams = Resources.FindObjectsOfTypeAll(_cam2Type);
        foreach (var cam in cams)
        {
            LinkCamera(cam as MonoBehaviour);
        }
    }

    private static void OnCameraInit(MonoBehaviour __instance)
    {
        LinkCamera(__instance);
    }

    private static void LinkCamera(MonoBehaviour cam)
    {
        if (cam == null || _roomOrigin == null || _setOriginMethod == null) return;

        if (cam.transform.parent == null || cam.transform.parent.name.StartsWith("Cam2_Viewport"))
        {
            try
            {
                _setOriginMethod.Invoke(cam, new object[] { _roomOrigin, false, false });
            }
            catch (Exception e)
            {
                Console.WriteLine($"[RecenterFix] failed to link to cam {cam.name}: {e.Message}");
            }
        }
    }
}