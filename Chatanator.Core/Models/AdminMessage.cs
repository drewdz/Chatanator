using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Chatanator.Core.Models
{
    public enum AdminAction
    {
        Hello,
        Invite
    }

    /// <summary>
    /// This message is published in the lobby to request user information
    /// </summary>
    public class AdminMessage : BaseMessage, IIndexable
    {
        #region Constructors

        public AdminMessage()
        {
            Type = GetType().FullName;
        }

        #endregion Constructors

        #region Properties

        [JsonProperty]
        public AdminAction Action { get; set; }

        [JsonProperty]
        public ChatUser User { get; set; }

        [JsonProperty]
        public Channel Channel { get; set; }

        #endregion Properties
    }
}
