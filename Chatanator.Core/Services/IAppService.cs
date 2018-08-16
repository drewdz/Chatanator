using System;

namespace Chatanator.Core.Services
{
    public interface IAppService
    {
        #region Properties

        long LastActivity { get; set; }

        string CurrentUserId { get; set; }

        #endregion Properties
    }
}
