using MvvmCross;
using MvvmCross.Plugin;

namespace PE.Plugins.Dialogs.iOS
{
    [MvxPlugin]
    public class Plugin : IMvxConfigurablePlugin
    {
        private DialogsConfiguration _Configuration;

        public void Configure(IMvxPluginConfiguration configuration)
        {
            if (!(configuration is DialogsConfiguration)) throw new System.Exception("Configuration is not a value DialogsConfiguration.");
            _Configuration = (DialogsConfiguration)configuration;
        }

        public void Load()
        {
            Mvx.LazyConstructAndRegisterSingleton<IDialogService>(() => new DialogService(_Configuration));
        }
    }
}
