using Chatanator.Core.ViewModels;

using MvvmCross.Forms.Presenters.Attributes;
using MvvmCross.Forms.Views;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Chatanator.UI.Views
{
    [MvxNavigationPagePresentation(Animated = true, WrapInNavigationPage = true)]
    public partial class LobbyView : MvxContentPage<LobbyViewModel>
    {
        public LobbyView()
        {
            InitializeComponent();
        }

        protected override void OnViewModelSet()
        {
            base.OnViewModelSet();
            if (ViewModel == null) return;
            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("ShowIt"))
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    var result = await DisplayActionSheet("Title", "Cancel", "Destruct", "One", "Two", "Three", "Four");
                });
            }
        }
    }
}