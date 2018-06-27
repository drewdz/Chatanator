using Chatanator.Core.ViewModels;

using MvvmCross.Forms.Presenters.Attributes;
using MvvmCross.Forms.Views;

namespace Chatanator.UI.Views
{
    [MvxNavigationPagePresentation(Animated = true, WrapInNavigationPage = true)]
	public partial class LobbyView : MvxContentPage<LobbyViewModel>
	{
		public LobbyView ()
		{
			InitializeComponent ();
		}
	}
}