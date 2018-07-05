using MvvmCross;
using MvvmCross.Binding.Bindings.Target.Construction;
using MvvmCross.Droid.Support.V7.AppCompat;
using MvvmCross.Forms.Platforms.Android.Core;
using MvvmCross.Plugin;
using PE.Framework.Droid.AndroidApp.AppVersion;
using System;

namespace Chatanator.Droid
{
    public class Setup : MvxFormsAndroidSetup<Core.App, UI.App>
    {
        protected override void FillTargetFactories(IMvxTargetBindingFactoryRegistry registry)
        {
            base.FillTargetFactories(registry);
            MvxAppCompatSetupHelper.FillTargetFactories(registry);
        }

        protected override void InitializeFirstChance()
        {
            base.InitializeFirstChance();
            Mvx.LazyConstructAndRegisterSingleton<IAndroidApp>(() => new AndroidApp());
            Mvx.RegisterSingleton<MvvmCross.Platforms.Android.IMvxAndroidGlobals>(this);
        }

        protected override IMvxPluginConfiguration GetPluginConfiguration(Type plugin)
        {
            //  find the config method
            string name = plugin.FullName.Split(new char[] { '.' })[2];
            System.Diagnostics.Debug.WriteLine(string.Format("*** Setup.GetPluginConfiguration - Configuring plugin {0}", name));
            name = string.Format("Chatanator.Droid.Bootstrap.{0}PluginBootstrap", name);
            //  get this type
            var type = GetType().Assembly.GetType(name);
            if (type == null)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("*** Setup.GetPluginConfiguration - Setup: Could not find type {0}.", name));
                return base.GetPluginConfiguration(plugin);
            }
            //  find the configuration method
            var method = type.GetMethod("Configure", new Type[] { });
            if (method == null)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("*** Setup.GetPluginConfiguration - Setup: Could not find configuration method for type {0}.", name));
                return base.GetPluginConfiguration(plugin);
            }
            //  invoke the configuration method
            return (IMvxPluginConfiguration)method.Invoke(null, new object[] { });
        }
    }
}