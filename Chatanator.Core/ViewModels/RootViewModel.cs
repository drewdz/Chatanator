using MvvmCross.Navigation;
using MvvmCross.ViewModels;

namespace Chatanator.Core.ViewModels
{
    public class RootViewModel : MvxViewModel
    {
        #region Fields

        private readonly IMvxNavigationService _NavigationService;

        #endregion Fields

        #region Constructors

        public RootViewModel(IMvxNavigationService navigationService)
        {
            _NavigationService = navigationService;
        }

        #endregion Constructors

        #region Lifecycle

        public override void ViewAppeared()
        {
            MvxNotifyTask.Create(async () =>
            {
                await _NavigationService.Navigate<SfViewModel>();
            });
        }

        #endregion Lifecycle
    }
}
