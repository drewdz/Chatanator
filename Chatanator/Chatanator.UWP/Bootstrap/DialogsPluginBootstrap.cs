using MvvmCross.Platform.Plugins;
using PE.Plugins.Dialogs.WindowsCommon;
using Windows.UI.Xaml;

namespace Chatanator.UWP.Bootstrap
{
    public class DialogsPluginBootstrap : MvxPluginBootstrapAction<PE.Plugins.Dialogs.WindowsCommon.Plugin>
    {
        public static MvvmCross.Plugin.IMvxPluginConfiguration Configure(Window window)
        {
            return new DialogConfig
            {
                Window = window
            };
        }
    }
}
