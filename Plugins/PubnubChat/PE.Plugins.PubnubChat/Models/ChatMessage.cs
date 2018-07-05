using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace PE.Plugins.PubnubChat.Models
{
    public class ChatMessage : BaseMessage, IIndexable
    {
        #region Constructors

        public ChatMessage()
        {
            Type = GetType().FullName;
        }

        #endregion Constructors

        #region Properties

        [JsonProperty]
        public string Text { get; set; }

        [JsonProperty]
        public string FromName { get; set; }

        [JsonIgnore]
        public bool ShowStatus { get; set; } = false;

        [JsonIgnore]
        public bool Sent { get; set; } = false;

        #endregion Properties
    }
}
