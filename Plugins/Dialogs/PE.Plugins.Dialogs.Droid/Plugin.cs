using MvvmCross;
using MvvmCross.Plugin;

using System;

namespace PE.Plugins.Dialogs.Droid
{
    [MvxPlugin]
    public class Plugin : IMvxConfigurablePlugin
    {
        #region Properties

        private DialogConfig _Config;

        #endregion Properties

        #region Plugin

        public void Configure(IMvxPluginConfiguration configuration)
        {
            if (!(configuration is DialogConfig)) throw new Exception("Configuration does not appear to be a valid DialogConfig.");
            _Config = (DialogConfig)configuration;
        }

        public void Load()
        {
            Mvx.LazyConstructAndRegisterSingleton<IDialogService>(() => new DialogService(_Config));
        }

        #endregion Plugin
    }
}
