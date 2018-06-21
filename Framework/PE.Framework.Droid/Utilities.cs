using Android.App;
using Android.Content;

using MvvmCross.Platform;
using MvvmCross.Platform.Droid.Platform;
using System;
using System.Threading;

namespace PE.Framework.Droid
{
    public static class Utilities
    {
        public static Context GetActivityContext()
        {
			IMvxAndroidCurrentTopActivity topActivity;
			bool canResolve = Mvx.TryResolve(out topActivity);
			if(canResolve){
				return topActivity.Activity;
			}
			return null;

        }

        public static void MaskException(Action action)
        {
            try
            {
                action();
            }
            catch
            {

            }
        }

        public static void Dispatch(Action action)
        {
            if (Application.SynchronizationContext == SynchronizationContext.Current)
            {
                action();
            }
            else
            {
                Application.SynchronizationContext.Post(x => MaskException(action), null);
            }
        }
    }
}