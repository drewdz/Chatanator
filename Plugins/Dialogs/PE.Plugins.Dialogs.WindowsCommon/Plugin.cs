using MvvmCross;
using MvvmCross.Plugin;

namespace PE.Plugins.Dialogs.WindowsCommon
{
    [MvxPlugin]
    public class Plugin : IMvxConfigurablePlugin
    {
        private DialogConfig _Configuration;

        public void Configure(IMvxPluginConfiguration configuration)
        {
            if (!(configuration is DialogConfig)) throw new System.Exception("Configuration is not a valid DialogConfig.");
            _Configuration = (DialogConfig)configuration;
        }

        public void Load()
        {
            Mvx.LazyConstructAndRegisterSingleton<IDialogService>(() => new DialogService(_Configuration));
        }
    }
}
