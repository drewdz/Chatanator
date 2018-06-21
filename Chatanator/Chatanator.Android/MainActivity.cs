using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;

using Chatanator.Core.ViewModels;

using MvvmCross.Forms.Platforms.Android.Views;

namespace Chatanator.Droid
{
    [Activity(
        Label = "Chatanator",
        Icon = "@mipmap/ic_launcher",
        Theme = "@style/AppTheme",
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation,
        LaunchMode = LaunchMode.SingleTask,
        WindowSoftInputMode = SoftInput.AdjustPan)]
    public class MainActivity : MvxFormsAppCompatActivity<DummyViewModel>
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;
            base.OnCreate(bundle);
        }
    }
}

