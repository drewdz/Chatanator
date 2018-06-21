using Chatanator.Core.ViewModels;

using MvvmCross.Forms.Presenters.Attributes;
using MvvmCross.Forms.Views;

namespace Chatanator.UI.Views
{
    [MvxMasterDetailPagePresentation(MasterDetailPosition.Root)]
	public partial class RootView : MvxMasterDetailPage<RootViewModel>
	{
		public RootView ()
		{
			InitializeComponent ();
		}
	}
}