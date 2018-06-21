using PE.Framework.iOS;

using System;
using System.Drawing;

using UIKit;

namespace PE.Plugins.Dialogs.iOS
{
    public class LoadingOverlay : UIView
    {
        #region Fields

        private UIActivityIndicatorView activitySpinner;
        private UILabel loadingLabel;

        #endregion Fields

        #region Constructors

        public LoadingOverlay(RectangleF frame)
            : base(frame)
        {
            // configurable bits
            BackgroundColor = UIColor.Black;
            Alpha = 0.75f;
            AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;


            nfloat labelHeight = 22;
            nfloat labelWidth = Frame.Width - 20;

            // derive the center x and y
            float centerX = (float)Frame.Width / 2;
            float centerY = (float)Frame.Height / 2;

            // create the activity spinner, center it horizontall and put it 5 points above center x
            activitySpinner = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.WhiteLarge);
            activitySpinner.Frame = new RectangleF((float)(centerX - (activitySpinner.Frame.Width / 2)), (float)(centerY - activitySpinner.Frame.Height - 20), (float)activitySpinner.Frame.Width, (float)activitySpinner.Frame.Height);
            activitySpinner.AutoresizingMask = UIViewAutoresizing.FlexibleMargins;
            this.AddSubview(activitySpinner);
            activitySpinner.StartAnimating();
        }

        #endregion Constructors

        #region Methods

        public void Hide()
        {
            try
            {
                Utilities.Dispatch(() => UIView.Animate(0.5, () => Alpha = 0, () => this.RemoveFromSuperview()));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        #endregion Methods
    }
}
