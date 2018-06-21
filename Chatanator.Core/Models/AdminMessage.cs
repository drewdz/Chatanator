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
    [DataContract]
    public class AdminMessage : BaseMessage
    {
        #region Constructors

        public AdminMessage()
        {
            Type = GetType().FullName;
        }

        #endregion Constructors

        #region Properties

        [DataMember]
        public AdminAction Action { get; set; }

        [DataMember]
        public ChatUser User { get; set; }

        [DataMember]
        public ChatChannel Channel { get; set; }

        #endregion Properties
    }
}
