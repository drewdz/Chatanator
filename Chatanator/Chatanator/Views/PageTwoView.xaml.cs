using Chatanator.Core.ViewModels;
using MvvmCross.Forms.Presenters.Attributes;
using MvvmCross.Forms.Views;

namespace Chatanator.UI.Views
{
    [MvxMasterDetailPagePresentation(MasterDetailPosition.Detail)]
	public partial class PageTwoView : MvxContentPage<PageTwoViewModel>
	{
		public PageTwoView ()
		{
			InitializeComponent ();
		}
	}
}