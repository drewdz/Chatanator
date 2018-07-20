using Realms;

using System.Runtime.Serialization;

namespace Chatanator.Core.Models
{
    [DataContract]
    public class AppUsage
    {
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public long LastActivity { get; set; } = 0;
    }

    [DataContract]
    public class AppUsageRealm : RealmObject
    {
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public long LastActivity { get; set; } = 0;
    }
}
