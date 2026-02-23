using System.ComponentModel;
using System.Runtime.CompilerServices;
using BeatSaberMarkupLanguage.Attributes;
using RecenterFix.Managers;
using UnityEngine;
using UnityEngine.XR;

#pragma warning disable IDE0051

namespace RecenterFix.UI;

internal sealed class SettingsUI : INotifyPropertyChanged
{
    internal static SettingsUI Instance { get; } = new();

    private const float ControllerFloorOffset = -0.01f;

    public event PropertyChangedEventHandler PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private string _statusValue = "Not calibrated";

    [UIValue("status-value")]
    public string StatusValue
    {
        get => _statusValue;
        set
        {
            if (_statusValue == value) return;
            _statusValue = value;
            OnPropertyChanged();
        }
    }

    [UIAction("calibrate-click")]
    private void OnCalibrateClick()
    {
        if (Plugin.SettingsManager == null)
        {
            StatusValue = "Error: settings not loaded yet";
            return;
        }

        bool hasLeft = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand)
            .TryGetFeatureValue(CommonUsages.devicePosition, out var leftPos);
        bool hasRight = InputDevices.GetDeviceAtXRNode(XRNode.RightHand)
            .TryGetFeatureValue(CommonUsages.devicePosition, out var rightPos);

        if (!hasLeft && !hasRight)
        {
            StatusValue = "Error: no controllers detected";
            return;
        }

        var (floorY, which) = (hasLeft, hasRight) switch
        {
            (true, true) => leftPos.y <= rightPos.y
                ? (leftPos.y, "Left")
                : (rightPos.y, "Right"),
            (true, false) => (leftPos.y, "Left"),
            _ => (rightPos.y, "Right")
        };

        float existingRoomY = Plugin.SettingsManager.settings.room.center.y;
        float actualFloorInVR = floorY + existingRoomY
                              + PluginConfig.Instance.RecenterCompensationY
                              + ControllerFloorOffset;

        PluginConfig.Instance.FloorCalibrationY = -actualFloorInVR;

        RecenterCompensator.ForceUpdateAllVRCenterAdjusts();

        StatusValue = $"Calibrated! {which} at {floorY:F3}m, offset {-actualFloorInVR:F3}m";
    }

    [UIAction("reset-click")]
    private void OnResetClick()
    {
        PluginConfig.Instance.FloorCalibrationY = 0f;
        RecenterCompensator.ForceUpdateAllVRCenterAdjusts();
        StatusValue = "Floor calibration reset";
    }
}