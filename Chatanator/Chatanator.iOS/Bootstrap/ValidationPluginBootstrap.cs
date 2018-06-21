using PE.Plugins.Validation;

using System;
using System.Text;

namespace Chatanator.iOS.Bootstrap
{
    public class ValidationPluginBootstrap : MvvmCross.Platform.Plugins.MvxPluginBootstrapAction<PE.Plugins.Validation.Plugin>
    {
        public static MvvmCross.Plugin.IMvxPluginConfiguration Configure()
        {
            return new ValidationConfig
            {
                CreateHash = delegate (string value)
                {
                    if (string.IsNullOrEmpty(value)) throw new ArgumentNullException("Cannot hash empty string");
                    var provider = System.Security.Cryptography.MD5.Create();
                    var hash = provider.ComputeHash(UTF8Encoding.UTF8.GetBytes(value));

                    //  returned hashed data as string
                    return Convert.ToBase64String(hash);
                }
            };
        }
    }
}