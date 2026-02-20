using BeatSaberMarkupLanguage.Settings;
using HarmonyLib;
using IPA;
using IPA.Config;
using IPA.Config.Stores;
using UnityEngine;

namespace RecenterFix;

[Plugin(RuntimeOptions.SingleStartInit)]
public class Plugin
{
    internal const string HarmonyId = "dylan.RecenterFix";
    internal const string FancyName = "Recenter Fix";

    internal static Harmony Harmony { get; private set; }

    internal static SettingsManager SettingsManager { get; set; }
    internal static SettingsApplicatorSO SettingsApplicator { get; set; }

    [Init]
    public Plugin(IPA.Logging.Logger logger, Config config)
    {
        PluginConfig.Instance = config.Generated<PluginConfig>();
    }

    [OnStart]
    public void OnApplicationStart()
    {
        Harmony = new Harmony(HarmonyId);
        Harmony.PatchAll(typeof(Plugin).Assembly);

        var compensatorObj = new GameObject("RecenterFix_Compensator");
        compensatorObj.AddComponent<RecenterCompensator>();
        Object.DontDestroyOnLoad(compensatorObj);

        BeatSaberMarkupLanguage.Util.MainMenuAwaiter.MainMenuInitializing += OnMainMenuInit;
    }

    private void OnMainMenuInit()
    {
        BSMLSettings.Instance.AddSettingsMenu(
            FancyName,
            "RecenterFix.Resources.BSML.SettingsUI.bsml",
            UI.SettingsUI.Instance
        );
    }

    [OnExit]
    public void OnApplicationQuit()
    {
        Harmony?.UnpatchSelf();
    }
}