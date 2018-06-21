using Chatanator.Core.Models;

namespace Chatanator.Core.Services
{
    public interface IUserService
    {
        #region Properties

        ChatUser User { get; }

        bool Initialized { get; }

        #endregion Properties

        #region Operations

        void Register(ChatUser user);

        #endregion Operations
    }
}
