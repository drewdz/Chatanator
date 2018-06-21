using Chatanator.Core.Models;

using PE.Plugins.LocalStorage;
using PE.Plugins.Validation;

using System;

namespace Chatanator.Core.Services
{
    public class UserService : IUserService
    {
        #region Fields

        private readonly ILocalStorageService _StorageService;
        private readonly IValidationService _ValidationService;

        #endregion Fields

        #region Constructors

        public UserService(ILocalStorageService storageService, IValidationService validationService)
        {
            _StorageService = storageService;
            _ValidationService = validationService;
            Initialize();
        }

        #endregion Constructors

        #region Properties

        public ChatUser User { get; set; }

        public bool Initialized
        {
            get { return ((User != null) && !string.IsNullOrEmpty(User.UserName)); }
        }

        #endregion Properties

        #region Init

        public void Initialize()
        {
            try
            {
                User = _StorageService.Get<ChatUser>("UserSetup");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("*** UserService.Initialize - Exception: {0}", ex));
            }
            if (User == null)
                {
                    User = new ChatUser { Id = Guid.NewGuid().ToString() };
                }
                else
                {
                    User.Initialized = true;
                }
        }

        #endregion Init

        #region Operations

        public void Register(ChatUser user)
        {
            if ((user == null) || (user.UserName == null) || string.IsNullOrEmpty(user.UserName.Trim())) throw new ArgumentException("Could not register without a name.");
            User = user;
            User.Initialized = true;
            //  save 
            _StorageService.Put("UserSetup", User);
        }

        #endregion Operations
    }
}
