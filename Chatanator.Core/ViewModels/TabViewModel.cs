using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using System.Threading.Tasks;

namespace Chatanator.Core.ViewModels
{
    public class TabViewModel : MvxViewModel
    {
        #region Fields

        private readonly IMvxNavigationService _NavigationService;

        #endregion Fields

        #region Constructors

        public TabViewModel(IMvxNavigationService navigationService)
        {
            _NavigationService = navigationService;
        }

        #endregion Constructors

        #region Properties

        private int _CurrentTab = 0;
        public int CurrentTab
        {
            get => _CurrentTab;
            set
            {
                SetProperty(ref _CurrentTab, value);
                Task.Run(() => CloseDetail());
            }
        }

        #endregion Properties

        #region Commands

        private IMvxCommand _PageOneCommand = null;
        public IMvxCommand PageOneCommand => _PageOneCommand ?? new MvxAsyncCommand(async () => await _NavigationService.Navigate<PageOneViewModel>());

        private IMvxCommand _PageTwoCommand = null;
        public IMvxCommand PageTwoCommand => _PageTwoCommand ?? new MvxAsyncCommand(async () => await _NavigationService.Navigate<PageTwoViewModel>());

        #endregion Commands

        #region Actions

        private void CloseDetail()
        {

        }

        #endregion Actions
    }
}
