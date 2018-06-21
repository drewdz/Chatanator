using Foundation;
using MvvmCross.Forms.Platforms.Ios.Core;
using UIKit;

namespace Chatanator.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : MvxFormsApplicationDelegate<Setup, Core.App, UI.App>
    {
        public override bool FinishedLaunching(UIApplication uiApplication, NSDictionary launchOptions)
        {
            new Syncfusion.SfNavigationDrawer.XForms.iOS.SfNavigationDrawerRenderer();
            Syncfusion.XForms.iOS.TabView.SfTabViewRenderer.Init();
            return base.FinishedLaunching(uiApplication, launchOptions);
        }
    }
}
