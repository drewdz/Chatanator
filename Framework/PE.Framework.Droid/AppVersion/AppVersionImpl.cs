using Android.App;
using Android.Content;
using Android.Content.PM;
using PE.Framework.AppVersion;

namespace PE.Framework.Droid.AppVersion
{
	public class AppVersionImpl : IVersion
    {
        public AppVersionImpl()
        {
        }

		public string Version
        {
            get
            {
                string result;
                try
                {
					Context context = Application.Context;
					PackageInfo pInfo = context.PackageManager.GetPackageInfo(context.PackageName, 0);
					result = pInfo.VersionName;
                }
                catch
                {
                    result = string.Empty;
                }
                return result;
            }
        }
	}
}
