using MvvmCross;
using MvvmCross.Plugin;

namespace PE.Plugins.PubnubChat
{
    [MvxPlugin]
    public class Plugin : IMvxConfigurablePlugin
    {
        private ChatConfiguration _Configuration;

        public void Configure(IMvxPluginConfiguration configuration)
        {
            if (!(configuration is ChatConfiguration)) throw new System.Exception("Configuration is not a valid ChatConfiguration");
            _Configuration = (ChatConfiguration)configuration;
        }

        public void Load()
        {
            Mvx.LazyConstructAndRegisterSingleton<IChatService>(() => new ChatService(_Configuration));
        }
    }
}
