using IPA.Config.Stores;
using IPA.Config.Stores.Attributes;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]

namespace RecenterFix;

internal class PluginConfig
{
    public static PluginConfig Instance { get; set; }

    public virtual float FloorCalibrationY { get; set; }

    [Ignore]
    public float RecenterCompensationY { get; set; }
}