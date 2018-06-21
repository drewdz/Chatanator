using MvvmCross;
using MvvmCross.Plugin;

namespace PE.Plugins.LocalStorage.WindowsCommon
{
    [MvxPlugin]
    public class Plugin : IMvxPlugin
    {
        public void Load()
        {
            Mvx.LazyConstructAndRegisterSingleton<ILocalStorageService>(() => new LocalStorageService());
        }
    }
}
