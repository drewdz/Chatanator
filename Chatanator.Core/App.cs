using Chatanator.Core.ViewModels;

using MvvmCross.IoC;
using MvvmCross.ViewModels;

namespace Chatanator.Core
{
    public class App : MvxApplication
    {
        public override void Initialize()
        {
            CreatableTypes()
                .EndingWith("Service")
                .AsInterfaces()
                .RegisterAsLazySingleton();

            RegisterAppStart<MainViewModel>();
        }
    }
}
