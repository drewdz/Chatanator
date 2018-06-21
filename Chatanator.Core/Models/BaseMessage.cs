using System;
using System.Runtime.Serialization;

namespace Chatanator.Core.Models
{
    public enum MessageStatus
    {
        New,
        Send,
        Delivered,
        Error
    }

    [DataContract]
    public class BaseMessage
    {
        [DataMember]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [DataMember]
        public long TimeStamp { get; set; }

        [DataMember]
        public MessageStatus Status { get; set; } = MessageStatus.New;

        [DataMember]
        public string Type { get; set; }

        [DataMember]
        public string FromUser { get; set; }

        [DataMember]
        public string ToUser { get; set; }
    }
}
