using Android.Content;
using MvvmCross.Plugin;

namespace PE.Plugins.Dialogs.Droid
{
    public class DialogConfig : DialogsConfiguration, IMvxPluginConfiguration
    {
        #region Properties

        public Context ApplicationContext { get; set; }

        public int TextViewStyle { get; set; }

        #endregion Properties
    }
}