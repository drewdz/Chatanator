using MvvmCross.Forms.Platforms.Uap.Core;
using MvvmCross.Forms.Platforms.Uap.Views;
using Windows.UI.Xaml;

namespace Chatanator.UWP
{
    sealed partial class App
    {
        public App()
        {
            InitializeComponent();
        }
    }

    public abstract class ProxyApp : MvxWindowsApplication<Setup, Core.App, UI.App, MainPage>
    {
        public static Window CurrentWindow { get; private set; }

        protected override void OnWindowCreated(WindowCreatedEventArgs args)
        {
            CurrentWindow = args.Window;
            base.OnWindowCreated(args);
        }
    }
}
