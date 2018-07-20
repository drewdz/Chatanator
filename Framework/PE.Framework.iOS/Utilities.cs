using Foundation;

using System;
using System.Linq;

using UIKit;

namespace PE.Framework.iOS
{
    public static class Utilities
    {
        public static void Dispatch(Action action)
        {
            UIApplication.SharedApplication.InvokeOnMainThread(() => action());
        }

        public static UIView GetTopView()
        {
            return GetTopWindow().Subviews.Last();
        }

        public static UIWindow GetTopWindow()
        {
            return UIApplication.SharedApplication.Windows.Reverse().FirstOrDefault(x => x.WindowLevel == UIWindowLevel.Normal && !x.Hidden);
        }

        public static void ShowSettings()
        {
            Dispatch(() => UIApplication.SharedApplication.OpenUrl(new NSUrl(UIApplication.OpenSettingsUrlString)));
        }
    }
}
