using System;

namespace Chatanator.Core.Models
{
    public class MessageEventArgs<TMessageType> : EventArgs
    {
        #region Constructors

        public MessageEventArgs()
        {
            Message = default(TMessageType);
        }

        public MessageEventArgs(TMessageType message)
        {
            Message = message;
        }

        #endregion Constructors

        #region Properties

        public TMessageType Message { get; set; }

        #endregion Properties
    }
}
