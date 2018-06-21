using Chatanator.Core.Models;
using Chatanator.Core.Services;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using PE.Plugins.Dialogs;
using PE.Plugins.Validation;
using PE.Plugins.Validation.Validators;
using System;

namespace Chatanator.Core.ViewModels
{
    public class RegisterViewModel : MvxViewModel
    {
        #region Fields

        private readonly IUserService _UserService;
        private readonly IValidationService _ValidationService;
        private readonly IDialogService _DialogService;
        private readonly IMvxNavigationService _NavigationService;

        #endregion Fields

        #region Constructors

        public RegisterViewModel(IUserService userService, IValidationService validationService, IDialogService dialogService, IMvxNavigationService navigationService)
        {
            _UserService = userService;
            _ValidationService = validationService;
            _DialogService = dialogService;
            _NavigationService = navigationService;
        }

        #endregion Constructors

        #region Properties

        private string _UserName = string.Empty;
        [RequiredValidator(Message = "User name is required.")]
        public string UserName
        {
            get => _UserName;
            set => SetProperty(ref _UserName, value);
        }

        private string _UserNameInvalid = string.Empty;
        public string UserNameInvalid
        {
            get => _UserNameInvalid;
            set
            {
                SetProperty(ref _UserNameInvalid, value);
                UserNameIsValid = string.IsNullOrEmpty(value);
            }
        }

        private bool _UserNameIsValid = true;
        public bool UserNameIsValid
        {
            get => _UserNameIsValid;
            set => SetProperty(ref _UserNameIsValid, value);
        }


        #endregion Properties

        #region Commands

        private IMvxCommand _RegisterCommand = null;
        public IMvxCommand RegisterCommand => _RegisterCommand ?? new MvxCommand(Register);

        #endregion Commands

        #region Actions

        private async void Register()
        {
            try
            {
                //  validate
                if (!_ValidationService.Validate(this)) return;
                //  register
                _UserService.Register(new ChatUser { Id = Guid.NewGuid().ToString(), UserName = UserName });
                //  all done
                await _NavigationService.Close(this);
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
