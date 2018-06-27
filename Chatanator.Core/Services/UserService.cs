using Chatanator.Core.Models;

using PE.Plugins.LocalStorage;
using PE.Plugins.Validation;

using System;
using System.Linq;
using System.Threading.Tasks;

namespace Chatanator.Core.Services
{
    public class UserService : IUserService
    {
        #region Fields

        private readonly ILocalStorageService _StorageService;
        private readonly IValidationService _ValidationService;
        private readonly ICosmosDataService _DataService;

        #endregion Fields

        #region Constructors

        public UserService(ILocalStorageService storageService, IValidationService validationService, ICosmosDataService dataService)
        {
            _StorageService = storageService;
            _ValidationService = validationService;
            _DataService = dataService;
            Initialize();
        }

        #endregion Constructors

        #region Properties

        public ChatUser User { get; set; }

        public bool Initialized
        {
            get { return ((User != null) && !string.IsNullOrEmpty(User.Id)); }
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
                User = new ChatUser { Id = string.Empty };
            }
            else
            {
                User.Initialized = true;
            }
        }

        #endregion Init

        #region Operations

        public async Task RegisterAsync(ChatUser user)
        {
            //  save the new user
            await _DataService.AddAsync(user);
            //  set up
            User = user;
            User.Initialized = true;
            //  save 
            _StorageService.Put("UserSetup", User);
        }

        #endregion Operations
    }
}
