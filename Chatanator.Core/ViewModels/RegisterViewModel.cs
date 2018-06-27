using Chatanator.Core.Models;
using Chatanator.Core.Services;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using PE.Plugins.Dialogs;
using PE.Plugins.Validation;
using PE.Plugins.Validation.Validators;
using System;
using System.Linq;

namespace Chatanator.Core.ViewModels
{
    public class RegisterViewModel : MvxViewModel
    {
        #region Fields

        private readonly IUserService _UserService;
        private readonly IValidationService _ValidationService;
        private readonly IDialogService _DialogService;
        private readonly IMvxNavigationService _NavigationService;
        private readonly ICosmosDataService _DataService;

        #endregion Fields

        #region Constructors

        public RegisterViewModel(IUserService userService, IValidationService validationService, IDialogService dialogService, IMvxNavigationService navigationService, ICosmosDataService dataService)
        {
            _UserService = userService;
            _ValidationService = validationService;
            _DialogService = dialogService;
            _NavigationService = navigationService;
            _DataService = dataService;
        }

        #endregion Constructors

        #region Properties

        #region FirstName

        private string _FirstName = string.Empty;
        [RequiredValidator(Message = "First name is required.")]
        public string FirstName
        {
            get => _FirstName;
            set
            {
                SetProperty(ref _FirstName, value);
                _ValidationService.Validate(this, () => FirstName);
            }
        }

        private string _FirstNameInvalid = string.Empty;
        public string FirstNameInvalid
        {
            get => _FirstNameInvalid;
            set
            {
                SetProperty(ref _FirstNameInvalid, value);
                FirstNameIsValid = string.IsNullOrEmpty(value);
            }
        }

        private bool _FirstNameIsValid = true;
        public bool FirstNameIsValid
        {
            get => _FirstNameIsValid;
            set => SetProperty(ref _FirstNameIsValid, value);
        }

        #endregion FirstName

        #region LastName

        private string _LastName = string.Empty;
        [RequiredValidator(Message = "Last name is required.")]
        public string LastName
        {
            get => _LastName;
            set
            {
                SetProperty(ref _LastName, value);
                _ValidationService.Validate(this, () => LastName);
            }
        }

        private string _LastNameInvalid = string.Empty;
        public string LastNameInvalid
        {
            get => _LastNameInvalid;
            set
            {
                SetProperty(ref _LastNameInvalid, value);
                LastNameIsValid = string.IsNullOrEmpty(value);
            }
        }

        private bool _LastNameIsValid = true;
        public bool LastNameIsValid
        {
            get => _LastNameIsValid;
            set => SetProperty(ref _LastNameIsValid, value);
        }

        #endregion LastName

        #region Email

        private string _Email = string.Empty;
        [RequiredValidator(Message = "Email is required.")]
        public string Email
        {
            get => _Email;
            set
            {
                SetProperty(ref _Email, value);
                _ValidationService.Validate(this, () => Email);
            }
        }

        private string _EmailInvalid = string.Empty;
        public string EmailInvalid
        {
            get => _EmailInvalid;
            set
            {
                SetProperty(ref _EmailInvalid, value);
                EmailIsValid = string.IsNullOrEmpty(value);
            }
        }

        private bool _EmailIsValid = true;
        public bool EmailIsValid
        {
            get => _EmailIsValid;
            set => SetProperty(ref _EmailIsValid, value);
        }

        #endregion Email

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
                var user = _UserService.User;
                if (user == null)
                {
                    //  create the user
                    user = new ChatUser { FirstName = FirstName, LastName = LastName, Email = Email };
                    user.Id = Guid.NewGuid().ToString();
                }
                else
                {
                    if (string.IsNullOrEmpty(user.Id)) user.Id = Guid.NewGuid().ToString();
                    //  update
                    user.FirstName = FirstName;
                    user.LastName = LastName;
                    user.Email = Email;
                }
                //  register
                await _UserService.RegisterAsync(user);
                //  all done
                //await _NavigationService.Close(this);
                await _NavigationService.Navigate<LobbyViewModel>();
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
