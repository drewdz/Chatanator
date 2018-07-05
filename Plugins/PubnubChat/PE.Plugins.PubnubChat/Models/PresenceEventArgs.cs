using System;

namespace PE.Plugins.PubnubChat.Models
{
    public class PresenceEventArgs : EventArgs
    {
        #region Constructors

        public PresenceEventArgs(string channel, string uuid, ChatState state = ChatState.None)
        {
            Channel = channel;
            Uuid = uuid;
            State = state;
        }

        #endregion Constructors

        #region Properties

        public string Channel { get; set; }

        public string Uuid { get; set; }

        public ChatState State { get; set; }

        #endregion Properties
    }
}
