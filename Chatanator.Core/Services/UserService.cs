using Chatanator.Core.Extensions;

using PE.Plugins.PubnubChat.Models;
using PE.Plugins.Validation;

using System;
using System.Threading.Tasks;

namespace Chatanator.Core.Services
{
    public class UserService : IUserService
    {
        #region Fields

        private readonly IValidationService _ValidationService;
        private readonly IDataService _DataService;
        private readonly IAppService _AppService;

        #endregion Fields

        #region Constructors

        public UserService(IValidationService validationService, IDataService dataService, IAppService appService)
        {
            _ValidationService = validationService;
            _DataService = dataService;
            _AppService = appService;
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
                User = _DataService.GetAppUser();
                //  get usage
                var usage = _DataService.GetAppUsage();
                if (usage == null) return;
                _AppService.LastActivity = usage.LastActivity;
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
            user.SaveAppUser(_DataService);
            //  set up
            User = user;
            User.Initialized = true;
        }

        #endregion Operations
    }
}
