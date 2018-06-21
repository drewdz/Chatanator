using System;
using System.Runtime.Serialization;

namespace Chatanator.Core.Models
{
    [DataContract]
    public class ChatMessage : BaseMessage
    {
        #region Constructors

        public ChatMessage()
        {
            Type = GetType().FullName;
        }

        #endregion Constructors

        #region Properties

        [DataMember]
        public string Text { get; set; }

        [DataMember]
        public string ChannelId { get; set; }

        public bool ShowStatus { get; set; } = false;

        public bool Sent { get; set; } = false;

        public string FromName { get; set; }

        #endregion Properties
    }
}
