using Android.App;
using Android.OS;
using Android.Runtime;

using MvvmCross;
using MvvmCross.Platforms.Android.Views;

using PE.Framework.AndroidApp.AppVersion;

using System;

using static Android.App.Application;

namespace Chatanator.Droid
{
    [Application]
    public class MainApplication : MvxAndroidApplication, IActivityLifecycleCallbacks
    {
        #region Constructors

        public MainApplication(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
            RegisterActivityLifecycleCallbacks(this);
        }

        #endregion Constructors

        #region Properties

        public static Activity Activity { get; private set; }

        #endregion Properties

        #region IActivityLifecycleCallbacks

        public void OnActivityCreated(Activity activity, Bundle savedInstanceState)
        {
            // NOP
        }

        public void OnActivityDestroyed(Activity activity)
        {
            if (Activity == activity) Activity = null;
        }

        public void OnActivityPaused(Activity activity)
        {
            // NOP
        }

        public void OnActivityResumed(Activity activity)
        {
            Activity = activity;
        }

        public void OnActivitySaveInstanceState(Activity activity, Bundle outState)
        {
            // NOP
        }

        public void OnActivityStarted(Activity activity)
        {
            IAndroidApp app = Mvx.Resolve<IAndroidApp>();
            app.TopActivity = activity;
        }

        public void OnActivityStopped(Activity activity)
        {
            IAndroidApp app = Mvx.Resolve<IAndroidApp>();
            if (activity.Equals(app.TopActivity))
            {
                app.TopActivity = null;
            }
        }

        #endregion IActivityLifecycleCallbacks
    }
}