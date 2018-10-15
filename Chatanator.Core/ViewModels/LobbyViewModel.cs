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
                if ((_UserService.User == null) || string.IsNullOrEmpty(_UserService.User.ChatUserId))
                {
                    await _NavigationService.Navigate<RegisterViewModel>();
                    return;
                }
                Username = _UserService.User.ToString();
                //  get a list of contacts
                var contacts = _DataService.GetChatUsers();
                if (contacts != null)
                {
                    Contacts = contacts.Where(c => !c.ChatUserId.Equals(_UserService.User.ChatUserId)).ToList();
                }
                IsEmpty = ((Contacts == null) || (Contacts.Count == 0));
                //  initialize the chat service
                _ChatService.InitializedChanged += _ChatService_InitializedChanged;
                _ChatService.Initialize(_UserService.User.ChatUserId);
            });
        }

        private void _ChatService_InitializedChanged(object sender, EventArgs e)
        {
            if (!_ChatService.Initialized) return;
            _ChatService.InitializedChanged -= _ChatService_InitializedChanged;
            //  get activity for recent channels since last
            _ChatService.GetHistory(_AppService.LastActivity);
        };

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
        }

        private void _ChatService_ChannelJoined(object sender, PresenceEventArgs e)
        {
        }

        private void _ChatService_MessageReceived(object sender, MessageEventArgs<BaseMessage> e)
        {
            if (e.Message is ChatMessage)
            {
                var message = (ChatMessage)e.Message;
                System.Diagnostics.Debug.WriteLine(string.Format("Message received: {0}", message.RawPayload));
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Message received!");
            }
        }

        private void _ChatService_ChannelState(object sender, PresenceEventArgs e)
        {
        }

        #endregion Event Handlers

        #region Operations

        private async Task BeginChatAsync(ChatUser user)
        {
            _AppService.CurrentUserId = user.ChatUserId;
            await _NavigationService.Navigate<BasicChatViewModel>();
        }

        private void ProcessChatMessage(ChatMessage message)
        {
            //  find the contact that sent this message
            var contact = Contacts.FirstOrDefault(c => c.ChatUserId.Equals(message.FromUser));
            if (contact == null) return;
            //  check if we already have this one
            var exists = _DataService.Provider.GetMessageById(message.MessageId);
            if (exists != null) return;
            //  add to the database
            message.Save(_DataService.Provider);
            //  indicate that there is a new message
            contact.NewContent = true;
        }

        #endregion Operations
    }
}
