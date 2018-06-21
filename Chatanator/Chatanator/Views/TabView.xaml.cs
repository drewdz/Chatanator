using Chatanator.Core.ViewModels;
using MvvmCross.Forms.Presenters.Attributes;
using MvvmCross.Forms.Views;

namespace Chatanator.UI.Views
{
	[MvxMasterDetailPagePresentation(MasterDetailPosition.Master)]
	public partial class TabView : MvxContentPage<TabViewModel>
	{
		public TabView ()
		{
			InitializeComponent ();
		}
	}
}