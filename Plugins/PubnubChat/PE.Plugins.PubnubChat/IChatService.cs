using PE.Plugins.PubnubChat.Models;

using System;

namespace PE.Plugins.PubnubChat
{
    public interface IChatService
    {
        #region Events

        event EventHandler InitializedChanged;
        event EventHandler ConnectedChanged;
        event EventHandler<MessageEventArgs<BaseMessage>> MessageReceived;
        event EventHandler<PresenceEventArgs> ChannelJoined;
        event EventHandler<PresenceEventArgs> ChannelLeft;
        event EventHandler<PresenceEventArgs> ChannelTimeout;
        event EventHandler<PresenceEventArgs> ChannelState;

        #endregion Events

        #region Properties

        bool Connected { get; }

        bool Initialized { get; }

        #endregion Properties

        #region Operations

        void Initialize(string userId, long lastActivity = 0);

        void Publish<TMessage>(TMessage message) where TMessage : BaseMessage;

        void GetHistory(long timeStamp);

        #endregion Operations
    }
}
