using Chatanator.Core.ViewModels;

using MvvmCross.Forms.Presenters.Attributes;
using MvvmCross.Forms.Views;

namespace Chatanator.UI.Views
{
    [MvxModalPresentation(Animated = true, NoHistory = true, Title = "Register", HostViewModelType = typeof(BasicChatViewModel))]
    //[MvxMasterDetailPagePresentation(MasterDetailPosition.Detail)]
    public partial class RegisterView : MvxContentPage<RegisterViewModel>
    {
		public RegisterView ()
		{
			InitializeComponent ();
		}
	}
}