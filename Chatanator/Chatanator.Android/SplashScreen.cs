using Android.App;
using Android.Content.PM;
using Android.OS;
using MvvmCross.Forms.Platforms.Android.Views;

namespace Chatanator.Droid
{
    // No Splash Screen: To remove splash screen, remove this class and uncomment lines in MainActivity
    [Activity(
        Label = "Chatanator"
        , MainLauncher = true
        , Icon = "@mipmap/ic_launcher"
        , Theme = "@style/AppTheme.Splash"
        , NoHistory = true
        , ScreenOrientation = ScreenOrientation.Portrait)]
    public class SplashScreen : MvxFormsSplashScreenActivity<Setup, Core.App, UI.App>
    {
        protected override void RunAppStart(Bundle bundle)
        {
            StartActivity(typeof(MainActivity));
            base.RunAppStart(bundle);
        }
    }
}