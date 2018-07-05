using MvvmCross.Platform.Plugins;

using PE.Plugins.PubnubChat;

namespace Chatanator.UWP.Bootstrap
{
    public class PubnubChatPluginBootstrap : MvxPluginBootstrapAction<PE.Plugins.PubnubChat.Plugin>
    {
        public static MvvmCross.Plugin.IMvxPluginConfiguration Configure()
        {
            return new ChatConfiguration
            {
                SubscribeKey = "sub-c-45047c26-6330-11e8-8176-12d6db7070d8",
                PublishKey = "pub-c-4d9191a2-eb1a-4015-8554-1d3179be699e"
            };
        }
    }
}
