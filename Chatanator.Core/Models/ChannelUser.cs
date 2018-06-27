using Newtonsoft.Json;

namespace Chatanator.Core.Models
{
    public class ChannelUser : IIndexable
    {
        /// <summary>
        /// Record id
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Channel Id. This property should be indexed
        /// </summary>
        [JsonProperty]
        public string ChannelId { get; set; }

        /// <summary>
        /// Channel User Id. This property should be indexed
        /// </summary>
        [JsonProperty]
        public string ChatUserId { get; set; }
    }
}
