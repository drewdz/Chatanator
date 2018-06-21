using Chatanator.Core.Models;
using Chatanator.Core.Services;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using PE.Plugins.Dialogs;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Chatanator.Core.ViewModels
{
    public class MainViewModel : MvxViewModel
    {
        #region Fields

        private readonly IUserService _UserService;
        private readonly IDialogService _DialogService;
        private readonly IChatService _ChatService;
        private readonly IMvxNavigationService _NavigationService;

        #endregion Fields

        #region Constructors

        public MainViewModel(IUserService userService, IDialogService dialogService, IChatService chatService, IMvxNavigationService navigationService)
        {
            _UserService = userService;
            _DialogService = dialogService;
            _ChatService = chatService;
            _NavigationService = navigationService;

            //  listen for events
            _ChatService.ChannelCreated += (sender, e) =>
            {
                //  do nothing for now
            };
            _ChatService.ChannelJoined += (sender, e) =>
            {
                if (!e.Channel.Equals(ChatService.CH_LOBBY) || e.Uuid.Equals(_UserService.User.Id)) return;
                //  get user details from the server
                Task.Run(() => GetUser(e.Uuid));
            };
            _ChatService.ChannelLeft += (sender, e) =>
            {
                //  do nothing for now
            };
            _ChatService.ChannelTimeout += (sender, e) =>
            {
                //  afk - -do nothing for now
            };
            _ChatService.ConnectedChanged += (sender, e) =>
            {
                //  do nothing for now
            };
            _ChatService.MessageReceived += (sender, e) =>
            {
                //  ignore messages not for me or messages I have sent
                if (e.Message.FromUser.Equals(_UserService.User.Id) || !e.Message.ToUser.Equals(_UserService.User.Id)) return;
                //  lobby/admin messages
                if (e.Message is AdminMessage)
                {
                    ProcessAdminMessage((AdminMessage)e.Message);
                }
                else if (e.Message is ChatMessage)
                {
                    //  TODO: highlight the channel for this message
                }
            };
        }

        #endregion Constructors

        #region Properties

        private MvxObservableCollection<ChatUser> _Users = new MvxObservableCollection<ChatUser>();
        public MvxObservableCollection<ChatUser> Users
        {
            get => _Users;
            set => SetProperty(ref _Users, value);
        }

        private ChatUser _User;
        public ChatUser User
        {
            get => _User;
            set
            {
                SetProperty(ref _User, value);
                if (value == null) return;
                BeginChat(value);
                User = null;
            }
        }

        private bool _IsEmpty = true;
        public bool IsEmpty
        {
            get => _IsEmpty;
            set => SetProperty(ref _IsEmpty, value);
        }

        #endregion Properties

        #region Commands

        #endregion Commands

        #region Operations

        public override void ViewAppeared()
        {
            base.ViewAppeared();
            //  check if we need to register
            if (!_UserService.Initialized)
            {
                _NavigationService.Navigate<RegisterViewModel>();
                return;
            }
            //  initialize the chat
            if (!_ChatService.Initialized) _ChatService.Initialize();
        }

        private void GetUser(string uuid)
        {
            try
            {
                _ChatService.Publish(ChatService.CH_LOBBY, new AdminMessage { Action = AdminAction.Hello, FromUser = _UserService.User.Id, ToUser = uuid, User = new ChatUser { Id = uuid } });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("*** MainViewModel.GetUser - Exceeption: {0}", ex));
            }
        }

        private async void BeginChat(ChatUser withUser)
        {
            try
            {
                //  create a new channel
                var channel = new ChatChannel { Id = Guid.NewGuid().ToString() };
                channel.Users = new System.Collections.Generic.List<ChatUser>() { _UserService.User, withUser };
                //  subscribe to this channel
                _ChatService.Subscribe(channel);
                //  send an invite to the other person
                var invite = new AdminMessage()
                {
                    Action = AdminAction.Invite,
                    Channel = channel,
                    FromUser = _UserService.User.Id,
                    ToUser = withUser.Id
                };
                _ChatService.Publish(ChatService.CH_LOBBY, invite);
                //  go to the chat
                _ChatService.CurrentChannel = channel;
                _NavigationService.Navigate<BasicChatViewModel>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("*** MainViewModel.BeginChat - Exception: {0}", ex));
                await _DialogService.AlertAsync("Could not begin chat.", "Chatanator", AppConstants.DLG_ACCEPT, null);
            }
        }

        private async void ProcessAdminMessage(AdminMessage message)
        {
            try
            {
                if (message.Action == AdminAction.Hello)
                {
                    //  requesting my details
                    if (string.IsNullOrEmpty(message.User.UserName))
                    {
                        //  someone is requesting my details
                        var returnTo = message.FromUser;
                        message = new AdminMessage() { User = _UserService.User, ToUser = returnTo, FromUser = _UserService.User.Id };
                        //  return to sender
                        _ChatService.Publish(ChatService.CH_LOBBY, message);
                        //  get their details if needed
                        if (Users.FirstOrDefault(u => u.Id.Equals(returnTo)) == null) GetUser(returnTo);
                    }
                    else
                    {
                        if (Users.FirstOrDefault(u => u.Id.Equals(message.User.Id)) != null) return;
                        //  someone has sent me their details
                        Users.Add(message.User);
                        IsEmpty = ((Users == null) || (Users.Count == 0));
                    }
                }
                else if (message.Action == AdminAction.Invite)
                {
                    var user = Users.FirstOrDefault(u => u.Id.Equals(message.FromUser));
                    var text = (user == null) ? "You have been invited to chat. Would you like to join?" : string.Format("{0} has invited you to chat. Would you like to join", user.UserName);
                    await _DialogService.ConfirmAsync(text, "Chatanator", "Join", () =>
                    {
                        //  subscribe to the channel
                        _ChatService.Subscribe(message.Channel);
                        _ChatService.CurrentChannel = message.Channel;
                        _NavigationService.Navigate<BasicChatViewModel>();
                    }, AppConstants.DLG_DECLINE, null);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("*** MainViewModel.ProcessAdminMessage - Exception: {0}", ex));
                //await _DialogService.AlertAsync("Could not begin chat.", "Chatanator", AppConstants.DLG_ACCEPT, null);
            }
        }

        #endregion Operations
    }
}
