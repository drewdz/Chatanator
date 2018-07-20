using Chatanator.Core.Extensions;
using Chatanator.Core.Services;

using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;

using PE.Plugins.Dialogs;
using PE.Plugins.PubnubChat.Models;
using PE.Plugins.Validation;
using PE.Plugins.Validation.Validators;

using System;
using System.Collections.Generic;

namespace Chatanator.Core.ViewModels
{
    public class RegisterViewModel : MvxViewModel
    {
        #region Fields

        private readonly IUserService _UserService;
        private readonly IValidationService _ValidationService;
        private readonly IDialogService _DialogService;
        private readonly IMvxNavigationService _NavigationService;
        private readonly IDataService _DataService;

        #endregion Fields

        #region Constructors

        public RegisterViewModel(IUserService userService, IValidationService validationService, IDialogService dialogService, IMvxNavigationService navigationService, IDataService dataService)
        {
            _UserService = userService;
            _ValidationService = validationService;
            _DialogService = dialogService;
            _NavigationService = navigationService;
            _DataService = dataService;
        }

        #endregion Constructors

        #region Properties

        private List<ChatUser> _Users = null;
        public List<ChatUser> Users
        {
            get => _Users;
            set => SetProperty(ref _Users, value);
        }

        private ChatUser _User = null;
        [SelectedValueValidator("", Message = "Be somebody... If you can't be batman, then be yourself!")]
        public ChatUser User
        {
            get => _User;
            set => SetProperty(ref _User, value);
        }

        private string _UserInvalid = string.Empty;
        public string UserInvalid
        {
            get => _UserInvalid;
            set
            {
                SetProperty(ref _UserInvalid, value);
                UserIsValid = string.IsNullOrEmpty(value);
            }
        }

        private bool _UserIsValid = true;
        public bool UserIsValid
        {
            get => _UserIsValid;
            set => SetProperty(ref _UserIsValid, value);
        }


        #endregion Properties

        #region Commands

        private IMvxCommand _RegisterCommand = null;
        public IMvxCommand RegisterCommand => _RegisterCommand ?? new MvxCommand(Register);

        #endregion Commands

        #region Lifecycle

        public override void ViewAppeared()
        {
            Users = _DataService.CreateUsers();
        }

        #endregion Lifecycle

        #region Actions

        private async void Register()
        {
            try
            {
                //  validate
                if (!_ValidationService.Validate(this)) return;
                //  register
                await _UserService.RegisterAsync(User);
                //  all done
                if  (!_NavigationService.Close(this).Result) await _NavigationService.Navigate<LobbyViewModel>();
            }
            catch (Exception ex)
            {
                await _DialogService.AlertAsync("An error has prevented us from registering you.", "Register", AppConstants.DLG_ACCEPT, null);
                System.Diagnostics.Debug.WriteLine(string.Format("*** RegisterViewModel.Regsiter - Exception: {0}", ex));
            }
        }

        #endregion Actions
    }
}
