using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PE.Plugins.PubnubChat.Models
{
    public class ChatUser : IIndexable, INotifyPropertyChanged
    {
        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Events

        #region Properties

        private string _Id;
        [JsonProperty("id")]
        public string Id
        {
            get => _Id;
            set
            {
                _Id = value ?? Guid.NewGuid().ToString();
                RaisePropertyChanged();
            }
        }

        private string _FirstName = string.Empty;
        [JsonProperty]
        public string FirstName
        {
            get => _FirstName;
            set
            {
                _FirstName = value ?? string.Empty;
                RaisePropertyChanged();
            }
        }

        private string _LastName = string.Empty;
        [JsonProperty]
        public string LastName
        {
            get => _LastName;
            set
            {
                _LastName = value ?? string.Empty;
                RaisePropertyChanged();
            }
        }

        private string _Email = string.Empty;
        [JsonProperty]
        public string Email
        {
            get => _Email;
            set
            {
                _Email = value ?? string.Empty;
                RaisePropertyChanged();
            }
        }

        private bool _Available = false;
        [JsonIgnore]
        public bool Available
        {
            get => _Available;
            set
            {
                _Available = value;
                RaisePropertyChanged();
            }
        }

        private bool _NewContent = false;
        [JsonIgnore]
        public bool NewContent
        {
            get => _NewContent;
            set
            {
                _NewContent = value;
                RaisePropertyChanged();
            }
        }

        [JsonIgnore]
        public bool Initialized { get; set; }

        [JsonIgnore]
        public string Fullname => string.Format("{0} {1}", FirstName ?? string.Empty, LastName ?? string.Empty).Trim();

        private ChatState _State = ChatState.None;
        [JsonIgnore]
        public ChatState State
        {
            get => _State;
            set
            {
                _State = value;
                RaisePropertyChanged();
            }
        }

        #endregion Properties

        #region Helpers

        private void RaisePropertyChanged([CallerMemberName] string name = null)
        {
            if (string.IsNullOrEmpty(name)) return;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion Helpers
    }
}
