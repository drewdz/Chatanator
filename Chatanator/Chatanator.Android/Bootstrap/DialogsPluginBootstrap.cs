using MvvmCross.Platform.Plugins;
using PE.Plugins.Dialogs.Droid;

namespace Chatanator.Droid.Bootstrap
{
    public class DialogsPluginBootstrap : MvxPluginBootstrapAction<PE.Plugins.Dialogs.Droid.Plugin>
    {
        public static MvvmCross.Plugin.IMvxPluginConfiguration Configure()
        {
            return new DialogConfig
            {
            };
        }
    }
}