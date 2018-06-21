using Windows.UI.Xaml.Controls;

namespace PE.Plugins.Dialogs.WindowsCommon.Controls
{
    public sealed partial class CustomProgress : UserControl, IUpdatablePopup
    {
        #region Constructors

        public CustomProgress()
        {
            this.InitializeComponent();
        }

        #endregion Constructors

        #region IUpdatablePopup

        public float Progress
        {
            set
            {
                float v = (value > 1) ? 1 : (value < 0) ? 0 : value;
                pbProgress.Value = v;
            }
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
