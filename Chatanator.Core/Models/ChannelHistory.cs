using Newtonsoft.Json;

namespace Chatanator.Core.Models
{
    public class ChannelHistory : IIndexable
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// The channel id
        /// </summary>
        [JsonProperty]
        public string ChannelId { get; set; }

        /// <summary>
        /// Id of the last read message
        /// </summary>
        [JsonProperty]
        public string LastMessageId { get; set; }

        /// <summary>
        /// Timestamp of the last read message
        /// </summary>
        [JsonProperty]
        public long TimeStamp { get; set; }
    }
}
