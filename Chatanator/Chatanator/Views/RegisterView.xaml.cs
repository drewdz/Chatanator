using Chatanator.Core.ViewModels;

using MvvmCross.Forms.Presenters.Attributes;
using MvvmCross.Forms.Views;

namespace Chatanator.UI.Views
{
    [MvxModalPresentation(Animated = true, NoHistory = true, Title = "Register")]
    public partial class RegisterView : MvxContentPage<RegisterViewModel>
    {
		public RegisterView ()
		{
			InitializeComponent ();
		}
	}
}