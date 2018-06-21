using Chatanator.Core.ViewModels;
using MvvmCross.Forms.Presenters.Attributes;
using MvvmCross.Forms.Views;
using System;

namespace Chatanator.UI.Views
{
    [MvxMasterDetailPagePresentation(MasterDetailPosition.Master)]
	public partial class SfView : MvxContentPage<SfViewModel>
	{
		public SfView ()
		{
			InitializeComponent ();
            hamburgerButton.Clicked += (sender, e) =>
            {
                try
                {
                    navigationDrawer.ToggleDrawer();
                }
                catch (Exception ex)
                {

                }
            };
		}
	}
}