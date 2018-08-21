using Newtonsoft.Json;

namespace PE.Plugins.PubnubChat.Models
{
    public class ChatMessage : BaseMessage
    {
        #region Constructors

        public ChatMessage()
        {
            Type = GetType().FullName;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Raw payload data in Json format
        /// </summary>
        [JsonProperty]
        public string RawPayload { get; set; }

        /// <summary>
        /// Used by the message factory to decode payload data
        /// </summary>
        [JsonProperty]
        public string PayloadType { get; set; }

        /// <summary>
        /// The user who sent the message. 
        /// </summary>
        [JsonProperty]
        public string FromUser { get; set; }

        [JsonIgnore]
        public bool ShowStatus { get; set; } = false;

        [JsonIgnore]
        public bool Sent { get; set; } = false;

        #endregion Properties
    }
}
