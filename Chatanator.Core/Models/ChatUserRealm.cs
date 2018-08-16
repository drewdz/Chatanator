using Realms;
using System;
using System.Runtime.Serialization;

namespace Chatanator.Core.Models
{
    [DataContract]
    public class ChatUserRealm : RealmObject
    {
        #region Properties

        [DataMember, PrimaryKey]
        public string ChatUserId { get; set; } = Guid.NewGuid().ToString();

        [DataMember]
        public string FirstName { get; set; }

        [DataMember]
        public string LastName { get; set; }

        [DataMember]
        public string Email { get; set; }

        [DataMember]
        public bool Available { get; set; }

        [DataMember]
        public bool Initialized { get; set; }

        [DataMember]
        public bool AppUser { get; set; }

        #endregion Properties
    }
}
