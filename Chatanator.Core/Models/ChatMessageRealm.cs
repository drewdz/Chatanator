using Realms;
using System.Runtime.Serialization;

namespace Chatanator.Core.Models
{
    [DataContract]
    public class ChatMessageRealm : RealmObject
    {
        [DataMember]
        [PrimaryKey]
        public string MessageId { get; set; }

        [DataMember]
        public string GroupId { get; set; }

        [DataMember]
        public long TimeStamp { get; set; }

        [DataMember]
        public string Type { get; set; }

        [DataMember]
        public string ToUser { get; set; }

        [DataMember]
        public string RawPayload { get; set; }

        [DataMember]
        public string PayloadType { get; set; }

        [DataMember]
        public string FromUser { get; set; }
    }
}
