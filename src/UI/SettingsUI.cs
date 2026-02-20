using System.ComponentModel;
using System.Runtime.CompilerServices;
using BeatSaberMarkupLanguage.Attributes;
using UnityEngine.XR;

namespace RecenterFix.UI;

public class SettingsUI : INotifyPropertyChanged
{
    public static SettingsUI Instance { get; } = new SettingsUI();

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private string _statusValue = "Not calibrated";

    [UIValue("status-value")]
    public string StatusValue
    {
        get => _statusValue;
        set
        {
            _statusValue = value;
            OnPropertyChanged();
        }
    }

    [UIAction("calibrate-click")]
    private void OnCalibrateClick()
    {
        var leftHand = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        var rightHand = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

        bool hasLeft = leftHand.TryGetFeatureValue(CommonUsages.devicePosition, out var leftPos);
        bool hasRight = rightHand.TryGetFeatureValue(CommonUsages.devicePosition, out var rightPos);

        if (!hasLeft && !hasRight)
        {
            StatusValue = "Error: no controllers detected";
            return;
        }

        float floorY;
        string which;

        if (hasLeft && hasRight)
        {
            if (leftPos.y <= rightPos.y) { floorY = leftPos.y; which = "Left"; }
            else { floorY = rightPos.y; which = "Right"; }
        }
        else if (hasLeft) { floorY = leftPos.y; which = "Left"; }
        else { floorY = rightPos.y; which = "Right"; }

        float controllerThickness = 0.035f;
        float totalNeeded = -(floorY - controllerThickness);
        float calibration = totalNeeded - PluginConfig.Instance.RecenterCompensationY;
        PluginConfig.Instance.FloorCalibrationY = calibration;

        if (Plugin.SettingsApplicator != null)
        {
            Plugin.SettingsApplicator.NotifyRoomTransformOffsetWasUpdated();
        }

        StatusValue = $"Calibrated! {which} at {floorY:F3}m, calibration {calibration:F3}m";
    }

    [UIAction("reset-click")]
    private void OnResetClick()
    {
        PluginConfig.Instance.FloorCalibrationY = 0f;

        if (Plugin.SettingsApplicator != null)
        {
            Plugin.SettingsApplicator.NotifyRoomTransformOffsetWasUpdated();
        }

        StatusValue = "Floor calibration reset";
    }
}