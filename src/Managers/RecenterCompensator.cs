using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.XR;

#pragma warning disable IDE0051

namespace RecenterFix.Managers;

internal sealed class RecenterCompensator : MonoBehaviour
{
    internal static RecenterCompensator Instance { get; private set; }

    private static readonly MethodInfo SetRoomTransformOffsetMethod =
        typeof(VRCenterAdjust).GetMethod(
            "SetRoomTransformOffset",
            BindingFlags.NonPublic | BindingFlags.Instance);

    private readonly List<XRInputSubsystem> _subsystemsCache = [];

    private float _lastHmdY;
    private bool _hasLastHmdY;
    private bool _subscribed;

    private void Awake() => Instance = this;

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
        UnsubscribeAll();
    }

    private void OnEnable() => SubscribeToSubsystems();

    private void OnDisable() => UnsubscribeAll();

    private void Update()
    {
        if (!_subscribed)
            SubscribeToSubsystems();

        var hmd = InputDevices.GetDeviceAtXRNode(XRNode.Head);
        if (!hmd.TryGetFeatureValue(CommonUsages.devicePosition, out var pos)) return;

        _lastHmdY = pos.y;
        _hasLastHmdY = true;
    }

    private void OnTrackingOriginUpdated(XRInputSubsystem subsystem)
    {
        if (!_hasLastHmdY || PluginConfig.Instance == null)
        {
            Plugin.Log?.Warn("Recenter detected but not ready");
            return;
        }

        float beforeY = _lastHmdY;
        var hmd = InputDevices.GetDeviceAtXRNode(XRNode.Head);

        if (!hmd.TryGetFeatureValue(CommonUsages.devicePosition, out var pos))
        {
            Plugin.Log?.Warn("Recenter detected but couldn't read HMD");
            return;
        }

        float afterY = pos.y;
        float delta = afterY - beforeY;

        Plugin.Log?.Info($"Recenter: before={beforeY:F4} after={afterY:F4} delta={delta:F4}");

        if (Mathf.Abs(delta) < 0.01f)
        {
            Plugin.Log?.Info("Delta too small, skipping");
            return;
        }

        PluginConfig.Instance.RecenterCompensationY -= delta;
        _lastHmdY = afterY;

        Plugin.Log?.Info($"Compensation={PluginConfig.Instance.RecenterCompensationY:F4}");

        ForceUpdateAllVRCenterAdjusts();
    }

    internal static void ForceUpdateAllVRCenterAdjusts()
    {
        if (SetRoomTransformOffsetMethod == null)
        {
            Plugin.Log?.Error("SetRoomTransformOffset method not found");
            return;
        }

        var instances = FindObjectsOfType<VRCenterAdjust>();
        foreach (var vca in instances)
            SetRoomTransformOffsetMethod.Invoke(vca, null);
    }

    private void SubscribeToSubsystems()
    {
        _subsystemsCache.Clear();
        SubsystemManager.GetSubsystems(_subsystemsCache);

        foreach (var sub in _subsystemsCache)
        {
            sub.trackingOriginUpdated -= OnTrackingOriginUpdated;
            sub.trackingOriginUpdated += OnTrackingOriginUpdated;
        }

        _subscribed = _subsystemsCache.Count > 0;
    }

    private void UnsubscribeAll()
    {
        _subsystemsCache.Clear();
        SubsystemManager.GetSubsystems(_subsystemsCache);

        foreach (var sub in _subsystemsCache)
            sub.trackingOriginUpdated -= OnTrackingOriginUpdated;

        _subscribed = false;
    }
}