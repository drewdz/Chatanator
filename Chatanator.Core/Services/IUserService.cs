using PE.Plugins.PubnubChat.Models;
using System.Threading.Tasks;

namespace Chatanator.Core.Services
{
    public interface IUserService
    {
        #region Properties

        ChatUser User { get; }

        bool Initialized { get; }

        #endregion Properties

        #region Operations

        Task RegisterAsync(ChatUser user);

        #endregion Operations
    }
}
