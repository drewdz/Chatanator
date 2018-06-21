using Windows.UI.Xaml.Controls;

namespace PE.Plugins.Dialogs.WindowsCommon.Controls
{
    public sealed partial class CustomLoading : UserControl, IUpdatablePopup
    {
        #region Constructors

        public CustomLoading()
        {
            this.InitializeComponent();
        }

        public CustomLoading(string text)
        {
            txtInfo.Text = text;
            this.InitializeComponent();
        }

        #endregion Constructors

        #region Properties

        public string Feedback
        {
            set
            {
                txtInfo.Text = (string.IsNullOrEmpty(value)) ? string.Empty : value;
            }
        }

        #endregion Properties

        #region IUpdatablePopup

        public float Progress
        {
            set { }
        }

        public string Message
        {
            set { txtInfo.Text = (string.IsNullOrEmpty(value)) ? string.Empty : value; }
        }

        public double PopupWidth
        {
            set { this.Width = value; }
        }

        public double PopupHeight
        {
            set { this.Height = value; }
        }

        #endregion IUpdatablePopup
    }
}
