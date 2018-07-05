using PE.Plugins.PubnubChat.Models;
using Chatanator.Core.Services;

using MvvmCross.Navigation;
using MvvmCross.ViewModels;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PE.Plugins.PubnubChat;

namespace Chatanator.Core.ViewModels
{
    public class LobbyViewModel : MvxViewModel
    {
        #region Fields

        private readonly IMvxNavigationService _NavigationService;
        private readonly ICosmosDataService _DataService;
        private readonly IUserService _UserService;
        private readonly IChatService _ChatService;

        private long _StateTimestamp = 0;

        #endregion Fields

        #region Constructors

        public LobbyViewModel(IMvxNavigationService navigationService, ICosmosDataService dataService, IUserService userService, IChatService chatService)
        {
            _NavigationService = navigationService;
            _DataService = dataService;
            _UserService = userService;
            _ChatService = chatService;

            _ChatService.ChannelCreated += _ChatService_ChannelCreated;
            _ChatService.ChannelJoined += _ChatService_ChannelJoined;
            _ChatService.ChannelLeft += _ChatService_ChannelLeft;
            _ChatService.ConnectedChanged += _ChatService_ConnectedChanged;
            _ChatService.MessageReceived += _ChatService_MessageReceived;
            _ChatService.ChannelState += _ChatService_ChannelState;
        }

        #endregion Constructors

        #region Properties

        private MvxObservableCollection<ChatUser> _Contacts = new MvxObservableCollection<ChatUser>();
        public MvxObservableCollection<ChatUser> Contacts
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
                BeginChat(value);
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
                //  get a list of contacts
                var contacts = await _DataService.GetChatUsersAsync();
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
                    //  subscribe this user to the lobby
                    _ChatService.Subscribe(_ChatService.LobbyChannel);
                    //  get a list of channel occupants
                    _ChatService.GetState();
                };
                _ChatService.Initialize(_UserService.User.Id, (IDataService)_DataService);
            });
        }

        public override void ViewDisappearing()
        {
            base.ViewDisappearing();

            //  unsubscribe event handlers
            _ChatService.ChannelCreated -= _ChatService_ChannelCreated;
            _ChatService.ChannelJoined -= _ChatService_ChannelJoined;
            _ChatService.ChannelLeft -= _ChatService_ChannelLeft;
            _ChatService.ConnectedChanged -= _ChatService_ConnectedChanged;
            _ChatService.MessageReceived -= _ChatService_MessageReceived;
            _ChatService.ChannelState -= _ChatService_ChannelState;
        }

        #endregion Lifecycle

        #region Event Handlers

        private void _ChatService_ConnectedChanged(object sender, System.EventArgs e)
        {
        }

        private void _ChatService_ChannelLeft(object sender, PresenceEventArgs e)
        {
            //  we want to know when someone has joined the lobby
            if (!e.Channel.Equals(_ChatService.LobbyChannel.Id)) return;
            SetContactStatus(e.Uuid, false);
        }

        private void _ChatService_ChannelJoined(object sender, PresenceEventArgs e)
        {
            //  we want to know when someone has joined the lobby
            if (!e.Channel.Equals(_ChatService.LobbyChannel.Id)) return;
            //  find this user in the list
            SetContactStatus(e.Uuid, true);
        }

        private void _ChatService_ChannelCreated(object sender, PresenceEventArgs e)
        {
        }

        private void _ChatService_MessageReceived(object sender, MessageEventArgs<BaseMessage> e)
        {
            if (!(e.Message is ChatMessage)) return;
            //  not interesting in messages in the lobby
            if (e.Message.ChannelId.Equals(ChatService.CH_LOBBY)) return;
            //  find the channel
            var channel = _ChatService.Channels.FirstOrDefault(ch => ch.Id.Equals(e.Message.ChannelId));
            if (channel == null)
            {
                //  TODO: create a channel
                System.Diagnostics.Debug.WriteLine(string.Format("*** LobbyViewModel.~MessageReceived - Unknown channel {0}", e.Message.ChannelId));
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

        private async void SetContactStatus(string id, bool available)
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
                else
                {
                    //  see if this contact is now in the db then add it
                    var contacts = await _DataService.GetChatUsersAsync();
                    contact = contacts.FirstOrDefault(c => c.Id.Equals(id));
                    if (contact == null)
                    {
                        System.Diagnostics.Debug.WriteLine(string.Format("*** LobbyViewModel.SetContactStatus - Could not find new user {0}", id));
                        return;
                    }
                    contact.Available = available;
                    Contacts.Add(contact);
                }
                //  get all channels (this should be parameterized to limit results for optimization)
                var channels = await _DataService.GetChannelsAsync();
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
                var histories = await _DataService.GetChannelHistoryAsync();
                var history = histories.FirstOrDefault(h => h.ChannelId.Equals(channel.Id));
                if (history == null) return;
                //  get messages sent after the last time one we read
                _ChatService.GetHistory(channel, history.TimeStamp);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("*** LobbyViewModel.SetContactStatus - Exception: {0}", ex));
            }
        }

        private async void BeginChat(ChatUser user)
        {
            //  find the channel for this user
            var channels = await _DataService.GetChannelsAsync();
            var channel = channels.FirstOrDefault(ch => (ch.ChannelType == ChannelType.Individual) && (ch.Users != null) && ch.Users.Contains(user.Id) && ch.Users.Contains(_UserService.User.Id));
            if (channel == null)
            {
                channel = new Channel { Id = Guid.NewGuid().ToString(), Name = user.Fullname, ChannelType = ChannelType.Individual, Users = new string[] { _UserService.User.Id, user.Id } };
                await _DataService.AddAsync(channel);
            }
            //  make sure we're subscribed to this channel
            _ChatService.Subscribe(channel);
            //  select and view
            _ChatService.CurrentChannel = channel;
            await _NavigationService.Navigate<BasicChatViewModel>();
        }

        #endregion Operations
    }
}
