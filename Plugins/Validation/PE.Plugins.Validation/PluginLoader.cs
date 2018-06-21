using MvvmCross;
using MvvmCross.Plugin;
using System;

namespace PE.Plugins.Validation
{
    public class ValidationConfig : IMvxPluginConfiguration
    {
        public CreateHashCallback CreateHash;
    }

    //public class PluginLoader : IMvxConfigurablePluginLoader
    //{
    //    #region Fields

    //    public static PluginLoader Instance = new PluginLoader();
    //    public ValidationConfig Config = null;

    //    #endregion Fields

    //    #region Methods

    //    public void EnsureLoaded()
    //    {
    //        Mvx.LazyConstructAndRegisterSingleton<IValidationService>(() =>
    //        {
    //            return new ValidationService(Config);
    //        });
    //    }

    //    public void Configure(IMvxPluginConfiguration configuration)
    //    {

    //        if (!(configuration is ValidationConfig)) throw new ArgumentException("Configuration of incorrect type");
    //        Config = (ValidationConfig)configuration;
    //    }

    //    #endregion Methods
    //}
}
