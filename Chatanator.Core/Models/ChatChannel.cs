using System.Collections.Generic;

namespace Chatanator.Core.Models
{
    public class ChatChannel
    {
        public string Id { get; set; }

        public List<ChatUser> Users { get; set; }
    }
}
