using MvvmCross.Plugin;
using System;
using Windows.UI.Xaml;

namespace PE.Plugins.Dialogs.WindowsCommon
{
    public class DialogConfig : IMvxPluginConfiguration
    {
        public Window Window { get; set; }

        public Func<string, IUpdatablePopup> CustomLoadingDialog { get; set; }
    }
}
