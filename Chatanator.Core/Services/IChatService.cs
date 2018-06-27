using Chatanator.Core.Models;
using System;
using System.Collections.Generic;

namespace Chatanator.Core.Services
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
        event EventHandler<PresenceEventArgs> ChannelCreated;
        event EventHandler<PresenceEventArgs> ChannelState;

        #endregion Events

        #region Properties

        bool Connected { get; }

        List<Channel> Channels { get; }

        Channel CurrentChannel { get; set; }

        bool Initialized { get; }

        Channel LobbyChannel { get; }

        #endregion Properties

        #region Operations

        void Initialize();

        void Subscribe(Channel channel);

        void Unsubscribe(string id);

        void Publish<TMessage>(string channel, TMessage message) where TMessage : BaseMessage;

        void GetState();

        void GetHistory(Channel channel, long timeStamp);

        void SetState(Channel channel, ChatState state);

        #endregion Operations
    }
}
