using Chatanator.Core.Extensions;
using Chatanator.Core.Services;

using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;

using PE.Plugins.PubnubChat;
using PE.Plugins.PubnubChat.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chatanator.Core.ViewModels
{
    public class BasicChatViewModel : MvxViewModel
    {
        #region Fields

        private readonly IChatService _ChatService;
        private readonly IUserService _UserService;
        private readonly IMvxNavigationService _NavigationService;
        private readonly Services.IDataService _DataService;

        private Channel _Channel;
        private List<ChatUser> _ChannelUsers;
        private long _SentTimestamp = 0;
        private long _ActivityTimestamp = 0;
        private bool _StateSent = false;

        private long _StateTimestamp = 0;

        #endregion Fields

        #region Constructors

        public BasicChatViewModel(IChatService chatService, IUserService userService, IMvxNavigationService navigationService, Services.IDataService dataService)
        {
            _UserService = userService;
            _NavigationService = navigationService;
            _ChatService = chatService;
            _DataService = dataService;

            _ChatService.ConnectedChanged += (sender, e) =>
            {
                Online = _ChatService.Connected;
            };
            _ChatService.MessageReceived += (sender, e) =>
            {
                //  only interested in chat messages
                if (!(e.Message is ChatMessage)) return;
                //  only interested in messages on this channel
                if (!e.Message.ChannelId.Equals(_Channel.Id)) return;
                //  now we're in business
                var message = Messages.FirstOrDefault(mm => mm.Id.Equals(e.Message.Id));
                var m = (ChatMessage)e.Message;
                if (message == null)
                {
                    message = m;
                    Messages.Add(message);
                }
                else
                {
                    message.FromUser = m.FromUser;
                    message.Id = m.Id;
                    message.ShowStatus = m.ShowStatus;
                    message.Text = m.Text;
                    message.Type = m.Type;
                    message.FromName = m.FromName;
                }
                //  get the sender if necessary
                if (string.IsNullOrEmpty(message.FromName))
                {
                    if (!string.IsNullOrEmpty(message.FromUser))
                    {
                        var user = _ChannelUsers.FirstOrDefault(u => u.Id.Equals(message.FromUser));
                        message.FromName = (user == null) ? "unknown" : user.Fullname;
                    }
                    else
                    {
                        message.FromName = "anonymous";
                    }
                }

                message.Sent = message.FromUser.Equals(_UserService.User.Id);
                message.Status = (message.FromUser.Equals(_UserService.User.Id)) ? MessageStatus.Send : MessageStatus.Delivered;

                //  order messages
                Messages = Messages.OrderByDescending(a => a.TimeStamp).ToList();
            };
            _ChatService.ChannelState += async (sender, e) =>
            {
                //  we know when we're typing
                if (!e.Channel.Equals(_Channel.Id) || e.Uuid.Equals(_UserService.User.Id)) return;
                //  get the user
                var user = _ChannelUsers.FirstOrDefault(u => u.Id.Equals(e.Uuid));
                if (user == null) return;
                StateMessage = string.Format("{0} is {1}", user.Fullname, e.State.ToString());
                _StateTimestamp = DateTime.Now.Ticks;
                //  let this state expire
                await Task.Delay(3000);
                if ((DateTime.Now.Ticks - _StateTimestamp) < 3000) return;
                StateMessage = string.Empty;
            };
        }

        #endregion Constructors

        #region Properties

        private List<ChatMessage> _Messages = new List<ChatMessage>();
        public List<ChatMessage> Messages
        {
            get => _Messages;
            set => SetProperty(ref _Messages, value);
        }

        private bool _Online = true;
        public bool Online
        {
            get => _Online;
            set => SetProperty(ref _Online, value);
        }

        private string _Message = string.Empty;
        public string Message
        {
            get => _Message;
            set
            {
                SetProperty(ref _Message, value);
                Task.Run(() => SetTyping());
            }
        }

        private string _ChannelName = string.Empty;
        public string ChannelName
        {
            get => _ChannelName;
            set => SetProperty(ref _ChannelName, value);
        }

        private string _StateMessage = string.Empty;
        public string StateMessage
        {
            get => _StateMessage;
            set => SetProperty(ref _StateMessage, value);
        }

        #endregion Properties

        #region Commands

        private IMvxCommand _SendCommand = null;
        public IMvxCommand SendCommand
        {
            get => _SendCommand ?? new MvxCommand(() =>
            {
                if (!Online || string.IsNullOrEmpty(Message)) return;
                foreach (var user in _Channel.Users)
                {
                    if (user.Equals(_UserService.User.Id)) continue;
                    var message = new ChatMessage
                    {
                        Text = Message,
                        FromUser = _UserService.User.Id,
                        ChannelId = _Channel.Id,
                        FromName = _UserService.User.Fullname,
                        TimeStamp = DateTime.Now.Ticks,
                        Sent = true
                    };
                    _ChatService.Publish(_Channel.Id, message);
                    Messages.Add(message);
                }
                //  clear text
                Message = string.Empty;
            });
        }

        #endregion Commands

        #region Lifecycle

        public override void ViewAppeared()
        {
            MvxNotifyTask.Create(async () =>
            {
                if (_ChatService.CurrentChannel == null) return;
                _Channel = _ChatService.CurrentChannel;
                //  remove from group and subscribe to this channel
                //_ChatService.RemoveFromGroup(new List<Channel> { _Channel });
                _ChatService.Subscribe(_Channel);
                //  channel name is the other user's name
                if (_Channel.ChannelType == ChannelType.Individual)
                {
                    var id = _Channel.Users.FirstOrDefault(i => !i.Equals(_UserService.User.Id));
                    if (!string.IsNullOrEmpty(id)) _Channel.Name = _DataService.GetChatUserById(id).ToString();
                }
                //  get the users in this channel
                _ChannelUsers = _Channel.Users.Select(s => _DataService.GetChatUserById(s)).ToList();
                //  get channel history
                _ChatService.GetHistory(_Channel, -1);
                //  get history for this channel
                var channelHistory = _DataService.GetChannelHistoryByChannelId(_Channel.Id);
                if (channelHistory != null)
                {
                    //  channel has been active in the last 30 days
                    if (DateTime.Now.Subtract(new DateTime(channelHistory.TimeStamp)).TotalDays < 30) return;
                }
            });
        }

        public override void ViewDisappearing()
        {
            MvxNotifyTask.Create(async () =>
            {
                //  unsubscribe to the channel andd add back into group
                //_ChatService.Unsubscribe(_Channel.Id);
                //_ChatService.AddChannelToGroup(new List<Channel> { _Channel });
            });
        }

        #endregion Lifecycle

        #region Operations

        private async void SetTyping()
        {
            if (!_StateSent)
            {
                _StateSent = true;
                _ChatService.SetState(_Channel, ChatState.Typing);
                _SentTimestamp = DateTime.Now.Ticks;
            }
            _ActivityTimestamp = DateTime.Now.Ticks;
            await Task.Delay(1500);
            if ((DateTime.Now.Ticks - _ActivityTimestamp) >= 1500)
            {
                //  we've stopped typing now
                _ChatService.SetState(_Channel, ChatState.Waiting);
                _StateSent = false;
            }
            else if ((DateTime.Now.Ticks - _SentTimestamp) >= 3000)
            {
                //  allow state to be resent
                _ChatService.SetState(_Channel, ChatState.None);
                _StateSent = false;
            }
        }

        #endregion Operations
    }
}
