using Chatanator.Core.ViewModels;
using MvvmCross.Forms.Presenters.Attributes;
using MvvmCross.Forms.Views;

namespace Chatanator.UI.Views
{
    [MvxNavigationPagePresentation(WrapInNavigationPage = true, Animated = true, NoHistory = false, Title = "Chat")]
    public partial class BasicChatView : MvxContentPage<BasicChatViewModel>
	{
		public BasicChatView ()
		{
			InitializeComponent ();
		}
	}
}