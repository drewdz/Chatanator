using MvvmCross.Plugin;

namespace PE.Plugins.PubnubChat
{
    public class ChatConfiguration : IMvxPluginConfiguration
    {
        public string SubscribeKey { get; set; }

        public string PublishKey { get; set; }
    }
}
