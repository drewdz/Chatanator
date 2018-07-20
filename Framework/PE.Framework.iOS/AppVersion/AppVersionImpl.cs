using System;
using Foundation;
using PE.Framework.AppVersion;

namespace PE.Framework.iOS.AppVersion
{
	public class AppVersionImpl : IVersion
    {

		private readonly NSString _buildKey;
        private readonly NSString _versionKey;

        public AppVersionImpl()
        {
			_buildKey = new NSString("CFBundleVersion");
            _versionKey = new NSString("CFBundleShortVersionString");
        }

		public string Version
        {
            get
            {
                string result;
                try
                {
                    var infoDictionary = NSBundle.MainBundle.InfoDictionary;
                    NSObject nSObject = infoDictionary.ValueForKey(_buildKey);
                    NSObject nSObject2 = infoDictionary.ValueForKey(_versionKey);
                    result = $"{nSObject2}.{nSObject}";
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
