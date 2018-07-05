using Newtonsoft.Json;
using System;

namespace PE.Plugins.PubnubChat.Models
{
    public enum MessageStatus
    {
        New,
        Send,
        Delivered,
        Error
    }

    public class BaseMessage
    {
        [JsonProperty("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [JsonProperty]
        public string ChannelId { get; set; }

        [JsonProperty]
        public long TimeStamp { get; set; }

        [JsonProperty]
        public MessageStatus Status { get; set; } = MessageStatus.New;

        [JsonProperty]
        public string Type { get; set; }

        [JsonProperty]
        public string FromUser { get; set; }
    }
}
