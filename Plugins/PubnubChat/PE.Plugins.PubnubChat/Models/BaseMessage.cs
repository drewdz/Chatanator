using Newtonsoft.Json;
using System;

namespace PE.Plugins.PubnubChat.Models
{
    public enum MessageStatus
    {
        New,
        Sent,
        Delivered,
        Error
    }

    public class BaseMessage
    {
        /// <summary>
        /// Unique identifier for a message
        /// </summary>
        [JsonProperty]
        public string MessageId { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// To identify a group when sent as part of a group chat
        /// </summary>
        [JsonProperty]
        public string GroupId { get; set; }

        /// <summary>
        /// When the message was sent
        /// </summary>
        [JsonProperty]
        public long TimeStamp { get; set; }

        /// <summary>
        /// Used internally to track the message progress
        /// </summary>
        [JsonProperty]
        public MessageStatus Status { get; set; } = MessageStatus.New;

        /// <summary>
        /// Fully qualified name of the message class
        /// </summary>
        [JsonProperty]
        public string Type { get; set; }

        /// <summary>
        /// The user this message is sent to. This is equivilent to the user's inbound channel id
        /// </summary>
        [JsonProperty]
        public string ToUser { get; set; }
    }
}
