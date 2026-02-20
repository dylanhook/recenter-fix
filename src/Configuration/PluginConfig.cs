using IPA.Config.Stores;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]

namespace RecenterFix;

internal class PluginConfig
{
    public static PluginConfig Instance { get; set; }

    public virtual float FloorCalibrationY { get; set; }

    [IPA.Config.Stores.Attributes.Ignore]
    public float RecenterCompensationY { get; set; }

    public float TotalCorrectionY => FloorCalibrationY + RecenterCompensationY;
}