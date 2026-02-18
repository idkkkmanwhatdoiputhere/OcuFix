using IPA;
using IPA.Config;
using IPA.Config.Stores;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BeatSaberMarkupLanguage.Settings;
using OcuFix.Configuration;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.OpenXR;
using IPALogger = IPA.Logging.Logger;

namespace OcuFix
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        internal static Plugin Instance { get; private set; }
        internal static IPALogger Log { get; private set; }

        private bool ShouldIgnore()
        {
            if (!OpenXRRuntime.name.Contains("Oculus") && PluginConfig.Instance.EnableChecks)
            {
                Plugin.Log.Warn("OpenXR runtime is not Oculus, ignoring");
                return true;
            }

            return false;
        }

        [Init]
        public void Init(Config config, IPALogger logger)
        {
            Instance = this;
            Log = logger;

            PluginConfig.Instance = config.Generated<PluginConfig>();
        }

        [OnStart]
        public void OnApplicationStart()
        {
            if (ShouldIgnore())
                return;

            AswHelper.DisableAswWrapper();
            ProcessPriorityHelper.SwapPrioritiesWrapper();
            BeatSaberMarkupLanguage.Util.MainMenuAwaiter.MainMenuInitializing += MainMenuInit;
        }
        
        public void MainMenuInit()
        {
            BSMLSettings.Instance.AddSettingsMenu("OcuFix", "OcuFix.Views.Settings.bsml", PluginConfig.Instance);
        }

        [OnExit]
        public void OnApplicationQuit()
        {
            if (ShouldIgnore())
                return;

            if (!PluginConfig.Instance.Restore)
                return;

            AswHelper.RestoreAswWrapper();
            ProcessPriorityHelper.SwapPrioritiesWrapper();
        }
    }
}
