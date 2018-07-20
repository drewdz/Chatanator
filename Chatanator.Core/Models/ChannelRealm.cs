using Realms;
using System;
using System.Runtime.Serialization;

namespace Chatanator.Core.Models
{
    [DataContract]
    public class ChannelRealm : RealmObject
    {
        [DataMember, PrimaryKey]
        public string Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public bool Archived { get; set; }

        [DataMember]
        public int ChannelType { get; set; }

        [DataMember]
        public DateTimeOffset LastActivity { get; set; } = DateTimeOffset.MinValue;

        [DataMember]
        public string UsersKey { get; set; }
    }
}
