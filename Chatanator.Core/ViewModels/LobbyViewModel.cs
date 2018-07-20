using PE.Plugins.PubnubChat.Models;
using Chatanator.Core.Services;

using MvvmCross.Navigation;
using MvvmCross.ViewModels;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PE.Plugins.PubnubChat;
using Chatanator.Core.Extensions;

namespace Chatanator.Core.ViewModels
{
    public class LobbyViewModel : MvxViewModel
    {
        #region Fields

        private readonly IMvxNavigationService _NavigationService;
        private readonly IDataService _DataService;
        private readonly IUserService _UserService;
        private readonly IChatService _ChatService;
        private readonly IAppService _AppService;

        private long _StateTimestamp = 0;

        #endregion Fields

        #region Constructors

        public LobbyViewModel(IMvxNavigationService navigationService, IDataService dataService, IUserService userService, IChatService chatService, IAppService appService)
        {
            _NavigationService = navigationService;
            _DataService = dataService;
            _UserService = userService;
            _ChatService = chatService;
            _AppService = appService;

            _ChatService.ChannelCreated += _ChatService_ChannelCreated;
            _ChatService.ChannelJoined += _ChatService_ChannelJoined;
            _ChatService.ChannelLeft += _ChatService_ChannelLeft;
            _ChatService.ConnectedChanged += _ChatService_ConnectedChanged;
            _ChatService.MessageReceived += _ChatService_MessageReceived;
            _ChatService.ChannelState += _ChatService_ChannelState;
        }

        #endregion Constructors

        #region Properties

        private List<ChatUser> _Contacts = new List<ChatUser>();
        public List<ChatUser> Contacts
        {
            get => _Contacts;
            set => SetProperty(ref _Contacts, value);
        }

        private ChatUser _Contact = null;
        public ChatUser Contact
        {
            get => _Contact;
            set
            {
                SetProperty(ref _Contact, value);
                if (value == null) return;
                Task.Run(() => BeginChatAsync(value));
                //  reset so we can reselect
                Contact = null;
            }
        }

        private bool _IsEmpty = true;
        public bool IsEmpty
        {
            get => _IsEmpty;
            set => SetProperty(ref _IsEmpty, value);
        }

        private string _Username = string.Empty;
        public string Username
        {
            get => _Username;
            set => SetProperty(ref _Username, value);
        }

        #endregion Properties

        #region Lifecycle

        public override void ViewAppeared()
        {
            MvxNotifyTask.Create(async () =>
            {
                if ((_UserService.User == null) || string.IsNullOrEmpty(_UserService.User.Id))
                {
                    await _NavigationService.Navigate<RegisterViewModel>();
                    return;
                }
                Username = _UserService.User.ToString();
                //  get a list of contacts
                var contacts = _DataService.GetChatUsers();
                if (contacts != null)
                {
                    foreach (var contact in contacts)
                    {
                        if (contact.Id.Equals(_UserService.User.Id)) continue;
                        Contacts.Add(contact);
                    }
                }
                IsEmpty = (Contacts.Count == 0);
                //  initialize the chat service
                _ChatService.InitializedChanged += (sender, e) =>
                {
                    if (!_ChatService.Initialized) return;
                    //  get activity for recent channels since last
                    GetHistory(_AppService.LastActivity);
                };
                _ChatService.Initialize(_UserService.User.Id);
            });
        }

        public override void ViewDisappearing()
        {
            base.ViewDisappearing();

            //  unsubscribe event handlers
            //_ChatService.ChannelCreated -= _ChatService_ChannelCreated;
            //_ChatService.ChannelJoined -= _ChatService_ChannelJoined;
            //_ChatService.ChannelLeft -= _ChatService_ChannelLeft;
            //_ChatService.ConnectedChanged -= _ChatService_ConnectedChanged;
            //_ChatService.MessageReceived -= _ChatService_MessageReceived;
            //_ChatService.ChannelState -= _ChatService_ChannelState;
        }

        #endregion Lifecycle

        #region Event Handlers

        private void _ChatService_ConnectedChanged(object sender, System.EventArgs e)
        {
        }

        private void _ChatService_ChannelLeft(object sender, PresenceEventArgs e)
        {
            //  we want to know when someone has joined the lobby
            if (!e.Channel.Equals(_ChatService.LobbyChannel?.Id)) return;
            SetContactStatus(e.Uuid, false);
        }

        private void _ChatService_ChannelJoined(object sender, PresenceEventArgs e)
        {
            //  we want to know when someone has joined the lobby
            if (!e.Channel.Equals(_ChatService.LobbyChannel?.Id)) return;
            //  find this user in the list
            SetContactStatus(e.Uuid, true);
        }

        private void _ChatService_ChannelCreated(object sender, PresenceEventArgs e)
        {
            //  get the channel from the backend and see if I'm in it
                        
        }

        private void _ChatService_MessageReceived(object sender, MessageEventArgs<BaseMessage> e)
        {
            if (e.Message is AdminMessage)
            {
                ProcessAdminMessage((AdminMessage)e.Message);
            }
            else if (e.Message is ChatMessage)
            {
                ProcessChatMessage((ChatMessage)e.Message);
            }
        }

        private async void _ChatService_ChannelState(object sender, PresenceEventArgs e)
        {
            //  we know when we're typing
            if (e.Uuid.Equals(_UserService.User.Id)) return;
            //  find the user
            var user = Contacts.FirstOrDefault(c => c.Id.Equals(e.Uuid));
            if (user == null) return;
            user.State = e.State;
            _StateTimestamp = DateTime.Now.Ticks;
            //  let this state expire
            await Task.Delay(3000);
            if ((DateTime.Now.Ticks - _StateTimestamp) < 3000) return;
            user.State = ChatState.None;
        }

        #endregion Event Handlers

        #region Operations

        private void GetHistory(long lastActivity)
        {
            try
            {
                //  get channel history from storage
                var histories = _DataService.GetChannelHistoriesByTimestampGreater(lastActivity - (new TimeSpan(30, 0, 0).Ticks)).ToList();
                if (histories == null) return;
                //  add channels that have been active in the last 30 days
                foreach (var history in histories)
                {
                    //_ChatService.AddChannelToGroup(new List<Channel> { _DataService.GetChannelById(history.ChannelId) });
                    _ChatService.Subscribe(_DataService.GetChannelById(history.ChannelId));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("***  LobbyViewModel.GetHistory - Exception: {0}", ex));
            }
        }

        private void SetContactStatus(string id, bool available)
        {
            try
            {
                //  this is me - do nothing
                if (id.Equals(_UserService.User.Id)) return;
                //  find this user in the list
                var contact = Contacts.FirstOrDefault(c => c.Id.Equals(id));
                if (contact != null)
                {
                    contact.Available = available;
                }
                //  get all channels
                var channels = _DataService.GetChannels().ToList();
                if ((channels == null) || (channels.Count == 0)) return;
                //  create a users key
                var list = new List<string> { _UserService.User.Id, id }.OrderBy(s => s).ToList();
                string users = string.Empty;
                foreach (var s in list) users += s;
                //  see if a channel exists between us
                var channel = channels.FirstOrDefault(ch => ch.ChannelType == ChannelType.Individual && ch.UsersKey.Equals(users));
                if (channel == null) return;
                //  subscribe to this channel
                _ChatService.Subscribe(channel);
                //  get history for this channel
                var history = _DataService.GetChannelHistoryByChannelId(channel.Id);
                if (history == null) return;
                //  get messages sent after the last time one we read
                _ChatService.GetHistory(channel, history.TimeStamp);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("*** LobbyViewModel.SetContactStatus - Exception: {0}", ex));
            }
        }

        private async Task BeginChatAsync(ChatUser user)
        {
            //  find the channel for this user
            var channel = _DataService.GetChannelForUsers(_UserService.User.Id, user.Id);
            if (channel == null)
            {
                channel = new Channel { Id = Guid.NewGuid().ToString(), Name = user.Fullname, ChannelType = ChannelType.Individual, Users = new string[] { _UserService.User.Id, user.Id } };
                channel.Save(_DataService);
            }
            //  notify OP that we intend to chat - on OP Lobby
            _ChatService.Publish(string.Format(ChatService.CH_LOBBY, user.Id), new AdminMessage { Action = AdminAction.Invite, ChannelId = channel.Id, User = _UserService.User, Channel = channel });
            //  make sure we're subscribed to this channel
            //_ChatService.AddChannelToGroup(new List<Channel> { channel });
            _ChatService.Subscribe(channel);
            //  select and view
            _ChatService.CurrentChannel = channel;
            await _NavigationService.Navigate<BasicChatViewModel>();
        }

        private void ProcessAdminMessage(AdminMessage message)
        {
            //  new user
            if (message.Action == AdminAction.Hello )
            {
                var contact = Contacts.FirstOrDefault(c => c.Id.Equals(message.User.Id));
                if (contact != null) return;
                //  add the contact
                Contacts.Add(message.User);
                message.User.Save(_DataService);
            }
            else if (message.Action== AdminAction.Invite)
            {
                //  make sure both users are in the channel
                message.Channel.Save(_DataService);
                //  subscribe to this channel
                _ChatService.Subscribe(message.Channel);
            }
        }

        private void ProcessChatMessage(ChatMessage message)
        {
            //  not interesting in messages in the lobby
            //  this should never happen
            if (message.ChannelId.Equals(ChatService.CH_LOBBY)) return;
            //  find the channel
            //var channel = _ChatService.Channels.FirstOrDefault(ch => ch.Id.Equals(message.ChannelId));
            var channel = _DataService.GetChannelById(message.ChannelId);
            if (channel == null)
            {
                //  TODO: create a channel
                System.Diagnostics.Debug.WriteLine(string.Format("*** LobbyViewModel.~MessageReceived - Unknown channel {0}", message.ChannelId));
                return;
            }
            //  find the contact that this message is from
            var userId = channel.Users.FirstOrDefault(s => !s.Equals(_UserService.User.Id));
            if (string.IsNullOrEmpty(userId))
            {
                System.Diagnostics.Debug.WriteLine(string.Format("*** LobbyViewModel.~MessageReceived - Could not find userid !{0} - {1}", _UserService.User.Id, channel.UsersKey));
                return;
            }
            var contact = Contacts.FirstOrDefault(c => c.Id.Equals(userId));
            contact.NewContent = true;
        }

        #endregion Operations
    }
}
