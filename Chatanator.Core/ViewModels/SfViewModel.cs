using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;

namespace Chatanator.Core.ViewModels
{
    public class SfViewModel : MvxViewModel
    {
        #region Fields

        private readonly IMvxNavigationService _NavigationService;

        #endregion Fields

        #region Constructors

        public SfViewModel(IMvxNavigationService navigationService)
        {
            _NavigationService = navigationService;
        }

        #endregion Constructors

        #region Commands

        private IMvxCommand _ChatCommand = null;
        public IMvxCommand ChatCommand => _ChatCommand ?? new MvxAsyncCommand(async () => await _NavigationService.Navigate<BasicChatViewModel>());

        private IMvxCommand _TabCommand = null;
        public IMvxCommand TabCommand => _TabCommand ?? new MvxAsyncCommand(async () => await _NavigationService.Navigate<TabViewModel>());

        #endregion Commands
    }
}
