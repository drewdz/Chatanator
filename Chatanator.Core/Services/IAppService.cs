using System;

namespace Chatanator.Core.Services
{
    public interface IAppService
    {
        #region Properties

        long LastActivity { get; set; }

        #endregion Properties
    }
}
