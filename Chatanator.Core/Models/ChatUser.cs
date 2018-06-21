using System.Runtime.Serialization;

namespace Chatanator.Core.Models
{
    [DataContract]
    public class ChatUser
    {
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string UserName { get; set; }

        public bool Initialized { get; set; }
    }
}
