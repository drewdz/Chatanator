using Chatanator.Core.Models;
using Chatanator.Core.Services;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using System.Linq;

namespace Chatanator.Core.ViewModels
{
    public class BasicChatViewModel : MvxViewModel
    {
        #region Fields

        private readonly IChatService _ChatService;
        private readonly IUserService _UserService;
        private readonly IMvxNavigationService _NavigationService;

        #endregion Fields

        #region Constructors

        public BasicChatViewModel(IChatService chatService, IUserService userService, IMvxNavigationService navigationService)
        {
            _UserService = userService;
            _NavigationService = navigationService;
            _ChatService = chatService;
            _ChatService.ConnectedChanged += (sender, e) =>
            {

            };
            _ChatService.MessageReceived += (sender, e) =>
            {
                //  only interested in chat messages
                if (!(e.Message is ChatMessage)) return;
                //  only interested in messages to me or from me
                if (!e.Message.ToUser.Equals(_UserService.User.Id) || e.Message.FromUser.Equals(_UserService.User.Id)) return;
                //  only interested in messages on this channel
                var m = (ChatMessage)e.Message;
                if (!m.ChannelId.Equals(_ChatService.CurrentChannel.Id)) return;
                //  now we're in business
                var message = Messages.FirstOrDefault(mm => mm.Id.Equals(e.Message.Id));
                if (message == null)
                {
                    message = (ChatMessage)e.Message;
                    Messages.Add(message);
                }
                else
                {
                    message.FromUser = m.FromUser;
                    message.Id = m.Id;
                    message.ShowStatus = m.ShowStatus;
                    message.Text = m.Text;
                    message.TimeStamp = m.TimeStamp;
                    message.ToUser = m.ToUser;
                    message.Type = m.Type;
                }
                message.Sent = message.FromUser.Equals(_UserService.User.Id);
                message.FromName = (message.FromUser.Equals(_UserService.User.Id)) ? _UserService.User.UserName : ToUser.UserName;
                message.Status = (message.FromUser.Equals(_UserService.User.Id)) ? MessageStatus.Send : MessageStatus.Delivered;
            };
        }

        #endregion Constructors

        #region Properties

        private MvxObservableCollection<ChatMessage> _Messages = new MvxObservableCollection<ChatMessage>();
        public MvxObservableCollection<ChatMessage> Messages
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

        private ChatUser _ToUser = new ChatUser();
        public ChatUser ToUser
        {
            get => _ToUser;
            set => SetProperty(ref _ToUser, value);
        }

        #endregion Properties

        #region Commands

        private IMvxCommand _SendCommand = null;
        public IMvxCommand SendCommand
        {
            get => _SendCommand ?? new MvxCommand(() =>
            {
                if (!Online || string.IsNullOrEmpty(Message)) return;
                foreach (var user in _ChatService.CurrentChannel.Users)
                {
                    if (user.Id.Equals(_UserService.User.Id)) continue;
                    //  TODO: we assume there are exactly two users in this chat - no groups yet
                    var message = new ChatMessage
                    {
                        Text = Message,
                        FromUser = _UserService.User.Id,
                        ToUser = user.Id,
                        ChannelId = _ChatService.CurrentChannel.Id,
                        FromName = _UserService.User.UserName,
                        Sent = true
                    };
                    _ChatService.Publish(_ChatService.CurrentChannel.Id, message);
                    //  add this message to the list
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
            base.ViewAppeared();
            //  check if we need to register
            if (!_UserService.Initialized)
            {
                _NavigationService.Navigate<RegisterViewModel>();
                return;
            }
            //  get the user we're talking to
            foreach (var user in _ChatService.CurrentChannel.Users)
            {
                if (user.Id.Equals(_UserService.User.Id)) continue;
                ToUser = user;
                //  we're currently only expecting one other user
                break;
            }
        }

        #endregion Lifecycle
    }
}
