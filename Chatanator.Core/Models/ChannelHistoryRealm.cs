using Realms;
using System.Runtime.Serialization;

namespace Chatanator.Core.Models
{
    [DataContract]
    public class ChannelHistoryRealm : RealmObject
    {
        [DataMember, PrimaryKey]
        public string Id { get; set; }

        [DataMember]
        public string ChannelId { get; set; }

        [DataMember]
        public string LastMessageId { get; set; }

        [DataMember]
        public long TimeStamp { get; set; }
    }
}
