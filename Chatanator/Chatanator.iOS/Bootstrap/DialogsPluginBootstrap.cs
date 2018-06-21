using MvvmCross.Plugin;
using PE.Plugins.Dialogs.iOS;

namespace Chatanator.iOS.Bootstrap
{
    public class DialogsPluginBootstrap : MvvmCross.Platform.Plugins.MvxPluginBootstrapAction<PE.Plugins.Dialogs.iOS.Plugin>
    {
        public static IMvxPluginConfiguration Configure()
        {
            return new iOSDialogsConfiguration()
            {
            };
        }
    }
}