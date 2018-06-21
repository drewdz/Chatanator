using System;

namespace Chatanator.Core.Models
{
    public class PresenceEventArgs : EventArgs
    {
        #region Constructors

        public PresenceEventArgs(string channel, string uuid)
        {
            Channel = channel;
            Uuid = uuid;
        }

        #endregion Constructors

        #region Properties

        public string Channel { get; set; }

        public string Uuid { get; set; }

        #endregion Properties
    }
}
