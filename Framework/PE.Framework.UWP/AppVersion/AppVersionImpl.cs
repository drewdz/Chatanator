using PE.Framework.AppVersion;
using Windows.ApplicationModel;

namespace PE.Framework.UWP.AppVersion
{
    public class AppVersionImpl : IVersion
    {
        public string Version
        {
            get
            {
                string result;
                try
                {
                    PackageVersion pv = Package.Current.Id.Version;
                    result = $"{pv.Major}.{pv.Minor}.{pv.Build}.{pv.Revision}";
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
