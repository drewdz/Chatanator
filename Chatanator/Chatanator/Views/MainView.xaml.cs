using Chatanator.Core.ViewModels;

using MvvmCross.Forms.Presenters.Attributes;
using MvvmCross.Forms.Views;

namespace Chatanator.UI.Views
{
    [MvxContentPagePresentation(WrapInNavigationPage = false, Title = "Chatanator")]
    public partial class MainView : MvxContentPage<MainViewModel>
    {
        public MainView()
        {
            InitializeComponent();
        }
    }
}
