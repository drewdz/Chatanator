using Chatanator.Core.Models;
using PE.Framework.Serialization;
using PubnubApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Chatanator.Core.Services
{
    public class ChatService : IChatService, IDisposable
    {
        #region Constants

        public const string CH_LOBBY = "Lobby";

        #endregion Constants

        #region Events

        public event EventHandler ConnectedChanged;
        public event EventHandler<MessageEventArgs<BaseMessage>> MessageReceived;
        public event EventHandler<PresenceEventArgs> ChannelJoined;
        public event EventHandler<PresenceEventArgs> ChannelLeft;
        public event EventHandler<PresenceEventArgs> ChannelTimeout;
        public event EventHandler<PresenceEventArgs> ChannelCreated;

        #endregion Events

        #region Fields

        private readonly IUserService _UserService;
        private readonly string _KeyPublish = "pub-c-4d9191a2-eb1a-4015-8554-1d3179be699e";
        private readonly string _KeySubscribe = "sub-c-45047c26-6330-11e8-8176-12d6db7070d8";

        private Pubnub _Pubnub;

        private bool _Disposed = false;

        #endregion Fields

        #region Constructors

        public ChatService(IUserService userService)
        {
            //  TODO: provide keys via setup
            _UserService = userService;
        }

        ~ChatService()
        {
            Dispose();
        }

        #endregion Constructors

        #region Properties

        public bool Connected { get; private set; }

        public List<ChatChannel> Channels { get; private set; } = new List<ChatChannel>();

        public ChatChannel CurrentChannel { get; set; }

        public bool Initialized { get; private set; } = false;

        #endregion Properties

        #region Init

        public void Initialize()
        {
            //  we can only initialize if the user is registered
            if (!_UserService.Initialized || Initialized) return;

            PNConfiguration config = new PNConfiguration();
            config.PublishKey = _KeyPublish;
            config.SubscribeKey = _KeySubscribe;
            config.Uuid = _UserService.User.Id;
            config.Secure = true;

            config.SetPresenceTimeoutWithCustomInterval(60000, 5);

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
            Subscribe(new ChatChannel { Id = CH_LOBBY });
            //  get all channels
            GetState();
            Initialized = true;
        }

        #endregion Init

        #region Operations

        public void Subscribe(ChatChannel channel)
        {
            if (Channels.FirstOrDefault(c => c.Id.Equals(channel.Id)) != null) return;

            Channels.Add(channel);

            //  TODO: investigate adding the presence channel as well {0}-pnpres
            var channels = Channels.Select(c => c.Id).ToArray();

            _Pubnub
                .Subscribe<string>()
                .Channels(channels)
                .Execute();
        }

        public void Unsubscribe(string id)
        {
            var channel = Channels.FirstOrDefault(c => c.Id.Equals(id));
            if (channel == null) return;
            //  unsubscribe
            _Pubnub.Unsubscribe<string>().Channels(new string[] { id }).Execute();
            Channels.Remove(channel);
        }

        public void Publish<TMessage>(string channel, TMessage message) where TMessage : BaseMessage
        {
            //  can't publish to the lobby and we can only publish to subscribed channels
            if (string.IsNullOrEmpty(channel)) throw new ArgumentException("Cannot publish without first subscribing.");

            //  make sure we know who it's from
            message.FromUser = _UserService.User.Id;
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

        /// <summary>
        /// Gets a list of all users and all channels
        /// </summary>
        public void GetState()
        {
            try
            {
                if (_Pubnub == null) return;
                _Pubnub.HereNow()
                    .IncludeState(true)
                    .IncludeUUIDs(true)
                    .Async(new PNHereNowResultEx((result, status) =>
                    {
                        if (status.Error) return;
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
                                    if (occupant.Uuid.Equals(_UserService.User.Id))
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

        #endregion Event Triggers

        #region Cleanup

        public void Dispose()
        {
            if (_Disposed) return;
            //  TODO: check this - it looks dodgy
            SubscribeCallbackExt listenerSubscribeCallack = new SubscribeCallbackExt((pubnubObj, message) => { }, (pubnubObj, presence) => { }, (pubnubObj, status) => { });
            _Pubnub.AddListener(listenerSubscribeCallack);
            // some time later
            _Pubnub.RemoveListener(listenerSubscribeCallack); _Disposed = true;
        }

        #endregion Cleanup
    }
}
