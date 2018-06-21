using MvvmCross;
using MvvmCross.Plugin;

namespace PE.Plugins.Validation
{
    [MvxPlugin]
    public class Plugin : IMvxConfigurablePlugin
    {
        #region Fields

        private ValidationConfig _Config;

        #endregion Fields

        #region Init

        public void Configure(IMvxPluginConfiguration configuration)
        {
            if (!(configuration is ValidationConfig)) throw new System.Exception("Configuration does not appear to be a valid ValidationConfig.");
            _Config = (ValidationConfig)configuration;
        }

        public void Load()
        {
            Mvx.LazyConstructAndRegisterSingleton<IValidationService>(() => new ValidationService(_Config));
        }

        #endregion Init
    }
}
