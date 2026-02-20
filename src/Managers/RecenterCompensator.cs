using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace RecenterFix;

public class RecenterCompensator : MonoBehaviour
{
    public static RecenterCompensator Instance { get; private set; }

    private float _lastHmdY;
    private bool _hasLastHmdY;
    private bool _subscribed;

    void Awake()
    {
        Instance = this;
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
        UnsubscribeAll();
    }

    void OnEnable()
    {
        SubscribeToSubsystems();
    }

    void OnDisable()
    {
        UnsubscribeAll();
    }

    void Update()
    {
        if (!_subscribed)
        {
            SubscribeToSubsystems();
        }

        var hmd = InputDevices.GetDeviceAtXRNode(XRNode.Head);
        if (hmd.TryGetFeatureValue(CommonUsages.devicePosition, out var pos))
        {
            _lastHmdY = pos.y;
            _hasLastHmdY = true;
        }
    }

    private void OnTrackingOriginUpdated(XRInputSubsystem subsystem)
    {
        if (!_hasLastHmdY || PluginConfig.Instance == null) return;

        float beforeY = _lastHmdY;
        var hmd = InputDevices.GetDeviceAtXRNode(XRNode.Head);
        if (!hmd.TryGetFeatureValue(CommonUsages.devicePosition, out var pos)) return;

        float afterY = pos.y;
        float delta = afterY - beforeY;
        if (Mathf.Abs(delta) < 0.01f) return;

        PluginConfig.Instance.RecenterCompensationY -= delta;
        if (Plugin.SettingsApplicator != null)
        {
            Plugin.SettingsApplicator.NotifyRoomTransformOffsetWasUpdated();
        }
    }

    private void SubscribeToSubsystems()
    {
        var subsystems = new List<XRInputSubsystem>();
        SubsystemManager.GetSubsystems(subsystems);

        foreach (var sub in subsystems)
        {
            sub.trackingOriginUpdated -= OnTrackingOriginUpdated;
            sub.trackingOriginUpdated += OnTrackingOriginUpdated;
        }

        _subscribed = subsystems.Count > 0;
    }

    private void UnsubscribeAll()
    {
        var subsystems = new List<XRInputSubsystem>();
        SubsystemManager.GetSubsystems(subsystems);

        foreach (var sub in subsystems)
        {
            sub.trackingOriginUpdated -= OnTrackingOriginUpdated;
        }

        _subscribed = false;
    }
}