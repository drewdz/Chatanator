using PE.Framework.Serialization;
using PE.Plugins.PubnubChat.Models;

using PubnubApi;

using System;

namespace PE.Plugins.PubnubChat
{
    public enum ChatState
    {
        None,
        Waiting,
        Typing,
        InCall
    }

    public class ChatService : IChatService, IDisposable
    {
        #region Events

        public event EventHandler InitializedChanged;
        public event EventHandler ConnectedChanged;
        public event EventHandler<MessageEventArgs<BaseMessage>> MessageReceived;
        public event EventHandler<PresenceEventArgs> ChannelJoined;
        public event EventHandler<PresenceEventArgs> ChannelLeft;
        public event EventHandler<PresenceEventArgs> ChannelTimeout;
        public event EventHandler<PresenceEventArgs> ChannelState;

        #endregion Events

        #region Fields

        private Pubnub _Pubnub;

        private readonly string _PublishKey = string.Empty;
        private readonly string _SubscribeKey = string.Empty;

        private string _UserId = string.Empty;
        private bool _Disposed = false;

        #endregion Fields

        #region Constructors

        public ChatService(ChatConfiguration configuration)
        {
            _PublishKey = configuration.PublishKey;
            _SubscribeKey = configuration.SubscribeKey;
        }

        ~ChatService()
        {
            Dispose();
        }

        #endregion Constructors

        #region Properties

        public bool Connected { get; private set; }

        public bool Initialized { get; private set; } = false;

        #endregion Properties

        #region Init

        public void Initialize(string userId, long lastActivity = 0)
        {
            try
            {
                //  we can only initialize if the user is registered
                if (string.IsNullOrEmpty(userId)) return;
                _UserId = userId;

                PNConfiguration config = new PNConfiguration();
                config.PublishKey = _PublishKey;
                config.SubscribeKey = _SubscribeKey;
                config.Uuid = _UserId;
                config.Secure = true;

                _Pubnub = new Pubnub(config);

                SubscribeCallbackExt listenerSubscribeCallack = new SubscribeCallbackExt((pubnubObj, message) =>
                {
                    try
                    {
                        //  get the message base to determine type
                        BaseMessage m = Serializer.Deserialize<BaseMessage>(message.Message.ToString());
                        //  deserialize to actual type
                        m = (BaseMessage)Serializer.Deserialize(GetType().Assembly.GetType(m.Type), message.Message.ToString());
                        //  let listeners know
                        MessageReceived?.Invoke(this, new MessageEventArgs<BaseMessage>(m));
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(string.Format("*** ChatService.MessageReceived - Unable to deserialize message: {0}", message.Message.ToString()));
                    }
                }, (pubnubObj, presence) =>
                {
                    // handle incoming presence data 
                    if (presence.Event.Equals("join"))
                    {
                        RaiseChannelJoined(presence.Channel, presence.Uuid);
                    }
                    else if (presence.Event.Equals("leave"))
                    {
                        RaiseChannelLeft(presence.Channel, presence.Uuid);
                    }
                    else if (presence.Event.Equals("state-change"))
                    {
                        //  listen for status events - eg: typing, etc
                        if ((presence.State == null) || (presence.State.Count == 0)) return;
                        foreach (var key in presence.State.Keys)
                        {
                            var state = (ChatState)Enum.Parse(typeof(ChatState), presence.State[key].ToString());
                            RaiseChannelState(presence.Channel, presence.Uuid, state);
                        }
                    }
                    else if (presence.Event.Equals("timeout"))
                    {
                    }
                    else if (presence.Event.Equals("interval"))
                    {
                        //  find the ids that have joined
                        if ((presence.Join != null) && (presence.Join.Length > 0))
                        {
                            foreach (var uuid in presence.Join) RaiseChannelJoined(presence.Channel, uuid);
                        }
                        if ((presence.Leave != null) && (presence.Leave.Length > 0))
                        {
                            foreach (var uuid in presence.Leave) RaiseChannelJoined(presence.Channel, uuid);
                        }
                    }
                    else if (presence.HereNowRefresh)
                    {
                        //  TODO: request state for channels
                        //GetState();
                    }
                }, (pubnubObj, status) =>
                {
                    if (status.Operation == PNOperationType.PNHeartbeatOperation)
                    {
                        Connected = !status.Error;
                        ConnectedChanged?.Invoke(this, new EventArgs());
                    }
                    else if ((status.Operation != PNOperationType.PNSubscribeOperation) && (status.Operation != PNOperationType.PNUnsubscribeOperation))
                    {
                        return;
                    }

                    if (status.Category == PNStatusCategory.PNConnectedCategory)
                    {
                        // this is expected for a subscribe, this means there is no error or issue whatsoever
                    }
                    else if (status.Category == PNStatusCategory.PNReconnectedCategory)
                    {
                        // this usually occurs if subscribe temporarily fails but reconnects. This means
                        // there was an error but there is no longer any issue
                    }
                    else if (status.Category == PNStatusCategory.PNDisconnectedCategory)
                    {
                        // this is the expected category for an unsubscribe. This means there
                        // was no error in unsubscribing from everything
                    }
                    else if (status.Category == PNStatusCategory.PNUnexpectedDisconnectCategory)
                    {
                        // this is usually an issue with the internet connection, this is an error, handle appropriately
                    }
                    else if (status.Category == PNStatusCategory.PNAccessDeniedCategory)
                    {
                        // this means that PAM does allow this client to subscribe to this
                        // channel and channel group configuration. This is another explicit error
                    }
                });

                _Pubnub.AddListener(listenerSubscribeCallack);

                //  create and subscribe to the lobby channel
                _Pubnub
                    .Subscribe<string>()
                    .Channels(new string[] { _UserId })
                    .WithPresence()
                    .Execute();

                Initialized = true;
                InitializedChanged?.Invoke(this, new EventArgs());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("*** ChatService.Initialize - Exception: {0}", ex));
            }
        }

        #endregion Init

        #region Callbacks

        #endregion Callbacks

        #region Operations

        public void GetHistory(long timeStamp)
        {
            try
            {
                if (!Initialized || string.IsNullOrEmpty(_UserId)) return;
                _Pubnub.History()
                    .Channel(_UserId)
                    .Start(timeStamp)
                    .Count(20)
                    .Async(new PNHistoryResultExt((result, status) =>
                    {
                        if ((result == null) || (result.Messages == null) || (result.Messages.Count == 0)) return;
                        foreach (var message in result.Messages)
                        {
                            //  get the message base to determine type
                            BaseMessage m = Serializer.Deserialize<BaseMessage>(message.Entry.ToString());
                            //  deserialize to actual type
                            m = (BaseMessage)Serializer.Deserialize(GetType().Assembly.GetType(m.Type), message.Entry.ToString());
                            //  let listeners know
                            MessageReceived?.Invoke(this, new MessageEventArgs<BaseMessage>(m));
                        }
                    }));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("*** ChatService.GetHistory - Exception: {0}", ex));
            }
        }

        public void Publish<TMessage>(TMessage message) where TMessage : BaseMessage
        {
            try
            {
                if (!Initialized || string.IsNullOrEmpty(_UserId)) throw new ArgumentException("Cannot publish before initialize.");

                //  get message as payload
                var payload = Serializer.Serialize(message);
                //  publish message
                _Pubnub.Publish()
                    .Channel(message.ToUser)
                    .Message(payload)
                    .Async(new PNPublishResultExt((result, status) =>
                    {
                        if (message == null) return;
                        //  get the message
                        message.Status = (status.Error) ? MessageStatus.Error : MessageStatus.Delivered;
                        message.TimeStamp = result.Timetoken;
                    }));
                //  also publish to myself for history
                _Pubnub.Publish()
                    .Channel(_UserId)
                    .Message(payload)
                    .Async(new PNPublishResultExt((result, status) => { }));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("*** ChatService.Publish - Exception: {0}", ex));
            }
        }

        #endregion Operations

        #region Event Triggers

        private void RaiseChannelJoined(string channel, string uuid)
        {
            ChannelJoined?.Invoke(this, new PresenceEventArgs(channel, uuid));
        }

        private void RaiseChannelLeft(string channel, string uuid)
        {
            ChannelLeft?.Invoke(this, new PresenceEventArgs(channel, uuid));
        }

        private void RaiseChannelTimeout(string channel, string uuid)
        {
            ChannelTimeout?.Invoke(this, new PresenceEventArgs(channel, uuid));
        }

        private void RaiseChannelState(string channel, string uuid, ChatState state)
        {
            ChannelState?.Invoke(this, new PresenceEventArgs(channel, uuid, state));
        }

        #endregion Event Triggers

        #region Cleanup

        public void Dispose()
        {
            if (_Disposed) return;
            //  TODO: check this - it looks dodgy
            SubscribeCallbackExt listenerSubscribeCallack = new SubscribeCallbackExt((pubnubObj, message) => { }, (pubnubObj, presence) => { }, (pubnubObj, status) => { });
            _Pubnub.AddListener(listenerSubscribeCallack);
            // some time later
            _Pubnub.RemoveListener(listenerSubscribeCallack);
            _Pubnub.Destroy();
            _Disposed = true;
        }

        #endregion Cleanup
    }
}
