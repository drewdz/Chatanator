using PE.Framework.Serialization;
using PE.Plugins.PubnubChat.Models;

using PubnubApi;

using System;
using System.Collections.Generic;
using System.Linq;

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
        #region Constants

        public const string CH_LOBBY = "Lobby";

        #endregion Constants

        #region Events

        public event EventHandler InitializedChanged;
        public event EventHandler ConnectedChanged;
        public event EventHandler<MessageEventArgs<BaseMessage>> MessageReceived;
        public event EventHandler<PresenceEventArgs> ChannelJoined;
        public event EventHandler<PresenceEventArgs> ChannelLeft;
        public event EventHandler<PresenceEventArgs> ChannelTimeout;
        public event EventHandler<PresenceEventArgs> ChannelCreated;
        public event EventHandler<PresenceEventArgs> ChannelState;

        #endregion Events

        #region Fields

        private readonly string _PublishKey = string.Empty;
        private readonly string _SubscribeKey = string.Empty;

        private IDataService _DataService;
        private string _UserId = string.Empty;

        private Pubnub _Pubnub;
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

        public List<Channel> Channels { get; private set; } = new List<Channel>();

        public Channel CurrentChannel { get; set; }

        public bool Initialized { get; private set; } = false;

        public Channel LobbyChannel { get; private set; }

        #endregion Properties

        #region Init

        public async void Initialize(string userId, IDataService dataService)
        {
            try
            {
                _DataService = dataService;

                //  we can only initialize if the user is registered
                if (string.IsNullOrEmpty(userId)) return;
                _UserId = userId;

                //  get the lobby channel
                var channels = await _DataService.GetChannelsAsync();
                LobbyChannel = channels.FirstOrDefault(ch => ch.Id.Equals(CH_LOBBY));
                if (LobbyChannel == null)
                {
                    //  create the lobby
                    LobbyChannel = new Channel { Id = CH_LOBBY, Name = CH_LOBBY };
                    await _DataService.AddAsync(LobbyChannel);
                }
                if (LobbyChannel == null) throw new Exception("Could not create or retrieve the Lobby.");
                if (Channels.FirstOrDefault(ch => ch.Id.Equals(CH_LOBBY)) == null) Channels.Add(LobbyChannel);

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
                        m.ChannelId = message.Channel;
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
                        GetState();
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
                //  by default everyone is subscribed to the lobby (each org can have their own lobby?)
                Subscribe(LobbyChannel);
                //  get all channels
                GetState();
                Initialized = true;
                InitializedChanged?.Invoke(this, new EventArgs());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("*** ChatService.Initialize - Exception: {0}", ex));
            }
        }

        #endregion Init

        #region Operations

        public void Subscribe(Channel channel)
        {
            try
            {
                //  not ready
                if (!Initialized) return;
                lock (Channels)
                {
                    if (!Channels.Contains(channel)) Channels.Add(channel);

                    var channels = Channels.Select(c => c.Id).ToArray();

                    _Pubnub
                        .Subscribe<string>()
                        .Channels(channels)
                        .WithPresence()
                        .Execute();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("*** ChatService.Subscribe - Exception: {0}", ex));
            }
        }

        public void GetHistory(Channel channel, long timeStamp)
        {
            try
            {
                if (!Initialized) return;
                _Pubnub.History()
                    .Channel(channel.Id)
                    .Start(timeStamp)
                    .Count(20)
                    .Async(new PNHistoryResultExt((result, status) =>
                    {
                        if ((result.Messages == null) || (result.Messages.Count == 0)) return;
                        foreach (var message in result.Messages)
                        {
                            //  get the message base to determine type
                            BaseMessage m = Serializer.Deserialize<BaseMessage>(message.Entry.ToString());
                            //  deserialize to actual type
                            m = (BaseMessage)Serializer.Deserialize(GetType().Assembly.GetType(m.Type), message.Entry.ToString());
                            m.ChannelId = channel.Id;
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

        public void Unsubscribe(string id)
        {
            try
            {
                var channel = Channels.FirstOrDefault(c => c.Id.Equals(id));
                if (channel == null) return;
                //  unsubscribe
                _Pubnub.Unsubscribe<string>().Channels(new string[] { id }).Execute();
                Channels.Remove(channel);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("*** ChatService.Unsubscribe - Exception: {0}", ex));
            }
        }

        public void Publish<TMessage>(string channel, TMessage message) where TMessage : BaseMessage
        {
            try
            {
                //  can't publish to the lobby and we can only publish to subscribed channels
                if (string.IsNullOrEmpty(channel)) throw new ArgumentException("Cannot publish without first subscribing.");

                //  make sure we know who it's from
                message.FromUser = _UserId;
                message.ChannelId = channel;
                //  get message as payload
                var payload = Serializer.Serialize(message);
                //  publish message
                _Pubnub.Publish()
                    .Channel(channel)
                    .Message(payload)
                    .Async(new PNPublishResultExt((result, status) =>
                    {
                        if (message == null) return;
                        //  get the message
                        message.Status = (status.Error) ? MessageStatus.Error : MessageStatus.Delivered;
                        message.TimeStamp = result.Timetoken;
                    }));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("*** ChatService.Publish - Exception: {0}", ex));
            }
        }

        /// <summary>
        /// Gets a list of all users and all channels
        /// </summary>
        public void GetState()
        {
            //  TODO: consider doing this per channel
            try
            {
                if (_Pubnub == null) return;
                _Pubnub.HereNow()
                    .IncludeState(true)
                    .IncludeUUIDs(true)
                    .Async(new PNHereNowResultEx((result, status) =>
                    {
                        if (status.Error || (result.Channels == null) || (result.Channels.Count == 0)) return;

                        foreach (var channel in result.Channels)
                        {
                            if (channel.Value.ChannelName.Equals(CH_LOBBY))
                            {
                                foreach (var occupant in channel.Value.Occupants)
                                {
                                    RaiseChannelJoined(channel.Value.ChannelName, occupant.Uuid);
                                }
                            }
                            else
                            {
                                foreach (var occupant in channel.Value.Occupants)
                                {
                                    if (occupant.Uuid.Equals(_UserId))
                                    {
                                        //  only add this channel if i'm in there
                                        RaiseChannelCreated(channel.Value.ChannelName, string.Empty);
                                        break;
                                    }
                                }
                            }
                        }
                    }));
            }
            catch (Exception ex)
            {
                //  TODO: some analytics
                System.Diagnostics.Debug.WriteLine(string.Format("*** ChatService.GetState - Exception: {0}", ex));
            }
        }

        public void SetState(Channel channel, ChatState state)
        {
            try
            {
                _Pubnub.SetPresenceState()
                    .Channels(Channels.Select(ch => ch.Id).ToArray())
                    .State(new Dictionary<string, object> { { "State", state } })
                    .Async(new PNSetStateResultExt((result, status) =>
                    {
                        //  do nothing
                    }));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("*** ChatService.SetStatus - Exception: {0}", ex));
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

        private void RaiseChannelCreated(string channel, string uuid)
        {
            ChannelCreated?.Invoke(this, new PresenceEventArgs(channel, uuid));
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
