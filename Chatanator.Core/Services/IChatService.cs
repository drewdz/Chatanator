using Chatanator.Core.Models;
using System;
using System.Collections.Generic;

namespace Chatanator.Core.Services
{
    public interface IChatService
    {
        #region Events

        event EventHandler ConnectedChanged;
        event EventHandler<MessageEventArgs<BaseMessage>> MessageReceived;
        event EventHandler<PresenceEventArgs> ChannelJoined;
        event EventHandler<PresenceEventArgs> ChannelLeft;
        event EventHandler<PresenceEventArgs> ChannelTimeout;
        event EventHandler<PresenceEventArgs> ChannelCreated;

        #endregion Events

        #region Properties

        bool Connected { get; }

        List<ChatChannel> Channels { get; }

        ChatChannel CurrentChannel { get; set; }

        bool Initialized { get; }

        #endregion Properties

        #region Operations

        void Initialize();

        void Subscribe(ChatChannel channel);

        void Unsubscribe(string id);

        void Publish<TMessage>(string channel, TMessage message) where TMessage : BaseMessage;

        void GetState();

        #endregion Operations
    }
}
