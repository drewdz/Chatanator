using PE.Plugins.PubnubChat.Models;
using System;
using System.Collections.Generic;

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
        event EventHandler<PresenceEventArgs> ChannelCreated;
        event EventHandler<PresenceEventArgs> ChannelState;

        #endregion Events

        #region Properties

        bool Connected { get; }

        Channel CurrentChannel { get; set; }

        bool Initialized { get; }

        Channel LobbyChannel { get; }

        #endregion Properties

        #region Operations

        void Initialize(string userId, long lastActivity = 0);

        void Subscribe(Channel channel);

        void Unsubscribe(string id);

        void Publish<TMessage>(string channel, TMessage message) where TMessage : BaseMessage;

        void GetState(string channel = "");

        void GetHistory(Channel channel, long timeStamp);

        void SetState(Channel channel, ChatState state);

        void AddChannelToGroup(List<Channel> channels);

        void RemoveFromGroup(List<Channel> channels);

        #endregion Operations
    }
}
