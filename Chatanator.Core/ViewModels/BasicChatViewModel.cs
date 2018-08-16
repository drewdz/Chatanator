using Chatanator.Core.Extensions;
using Chatanator.Core.Services;

using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;

using PE.Plugins.PubnubChat;
using PE.Plugins.PubnubChat.Models;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Chatanator.Core.ViewModels
{
    public class BasicChatViewModel : MvxViewModel
    {
        #region Fields

        private readonly IChatService _ChatService;
        private readonly IUserService _UserService;
        private readonly IMvxNavigationService _NavigationService;
        private readonly Services.IDataService _DataService;
        private readonly IAppService _AppService;

        private long _SentTimestamp = 0;
        private long _ActivityTimestamp = 0;
        private bool _StateSent = false;

        private long _StateTimestamp = 0;

        private string _withUser = string.Empty;

        #endregion Fields

        #region Constructors

        public BasicChatViewModel(IChatService chatService, IUserService userService, IMvxNavigationService navigationService, Services.IDataService dataService, IAppService appService)
        {
            _UserService = userService;
            _NavigationService = navigationService;
            _ChatService = chatService;
            _DataService = dataService;
            _AppService = appService;

            _ChatService.ConnectedChanged += (sender, e) =>
            {
                Online = _ChatService.Connected;
            };
            _ChatService.MessageReceived += (sender, e) =>
            {
                //  only interested in chat messages
                if (!(e.Message is ChatMessage)) return;
                //  only interested in messages to/from this user
                var newMessage = (ChatMessage)e.Message;
                if (!newMessage.FromUser.Equals(_withUser) && !newMessage.ToUser.Equals(_withUser)) return;
                //  now we're in business
                var existing = Messages.FirstOrDefault(mm => mm.MessageId.Equals(e.Message.MessageId));
                if (existing == null)
                {
                    existing = newMessage;
                }
                else
                {
                    existing.FromUser = newMessage.FromUser;
                    existing.MessageId = newMessage.MessageId;
                    existing.ShowStatus = newMessage.ShowStatus;
                    existing.RawPayload = newMessage.RawPayload;
                    existing.Type = newMessage.Type;
                    existing.ToUser = newMessage.ToUser;
                }

                existing.Sent = existing.FromUser.Equals(_UserService.User.ChatUserId);
                existing.Status = (existing.FromUser.Equals(_UserService.User.ChatUserId)) ? MessageStatus.Sent : MessageStatus.Delivered;

                Messages.Add(existing);
            };
        }

        #endregion Constructors

        #region Properties

        private ObservableCollection<ChatMessage> _Messages = new ObservableCollection<ChatMessage>();
        public ObservableCollection<ChatMessage> Messages
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
            set => SetProperty(ref _Message, value);
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
                //  create a new message to send
                var message = new ChatMessage()
                {
                    RawPayload = Message,
                    FromUser = _UserService.User.ChatUserId,
                    ToUser = _withUser,
                    TimeStamp = DateTime.Now.Ticks,
                    Sent = true
                };
                //  save the message
                message.Save(_DataService.Provider);
                _ChatService.Publish(message);
                Messages.Add(message);

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
                _withUser = _AppService.CurrentUserId;
                //  get history for this chat
                var messages = _DataService.Provider.GetMessagesUser(_withUser);
                if (messages != null)
                {
                    Messages = new ObservableCollection<ChatMessage>(messages.OrderBy(m => m.TimeStamp));
                }
            });
        }

        public override void ViewDisappearing()
        {
            _AppService.CurrentUserId = string.Empty;
        }

        #endregion Lifecycle

        #region Operations

        #endregion Operations
    }
}
