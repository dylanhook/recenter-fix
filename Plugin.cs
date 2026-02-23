using IPA;
using IPA.Config;
using IPA.Config.Stores;
using JetBrains.Annotations;
using RecenterFix.HarmonyPatches;
using RecenterFix.UI;
using UnityEngine;
using IPALogger = IPA.Logging.Logger;

namespace RecenterFix;

[Plugin(RuntimeOptions.SingleStartInit), UsedImplicitly]
public class Plugin
{
    internal const string HarmonyId = "dylan.RecenterFix";
    internal const string FancyName = "Recenter Fix";
    internal const string ResourcesPath = "RecenterFix.src.Resources";

    internal static IPALogger Log { get; private set; }
    internal static SettingsManager SettingsManager { get; set; }
    internal static SettingsApplicatorSO SettingsApplicator { get; set; }

    [Init]
    public Plugin(IPALogger logger, Config config)
    {
        Log = logger;
        PluginConfig.Instance = config.Generated<PluginConfig>();
    }

    [OnStart, UsedImplicitly]
    public void OnApplicationStart()
    {
        HarmonyHelper.ApplyPatches();

        var compensatorObj = new GameObject("RecenterFix_Compensator");
        compensatorObj.AddComponent<RecenterFix.Managers.RecenterCompensator>();
        Object.DontDestroyOnLoad(compensatorObj);

        BeatSaberMarkupLanguage.Util.MainMenuAwaiter.MainMenuInitializing += OnMainMenuInitializing;
    }

    private static void OnMainMenuInitializing()
    {
        BeatSaberMarkupLanguage.Settings.BSMLSettings.Instance.AddSettingsMenu(
            FancyName,
            $"{ResourcesPath}.BSML.SettingsUI.bsml",
            SettingsUI.Instance);
    }

    [OnExit, UsedImplicitly]
    public void OnApplicationQuit() => HarmonyHelper.RemovePatches();
}