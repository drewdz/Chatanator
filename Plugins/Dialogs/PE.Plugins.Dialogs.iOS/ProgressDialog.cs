using PE.Framework.iOS;

using System;
using System.Drawing;

using UIKit;

namespace PE.Plugins.Dialogs.iOS
{
    public class ProgressDialog : IProgressDialog
    {
        #region Fields

        private LoadingOverlay _LoadingOverlay;

        #endregion Fields

        #region Properties

        public virtual bool IsDeterministic { get; set; }

        public virtual bool IsShowing { get; private set; }

        private int _PercentComplete;
        public virtual int PercentComplete
        {
            get { return _PercentComplete; }
            set
            {
                if (_PercentComplete == value) return;

                if (value > 100)
                {
                    _PercentComplete = 100;
                }
                else if (value < 0)
                {
                    _PercentComplete = 0;
                }
                else
                {
                    _PercentComplete = value;
                }

                Refresh();
            }
        }

        private string _Title = string.Empty;
        public virtual string Title
        {
            get { return _Title; }
            set
            {
                if (_Title.Equals(value)) return;

                _Title = value;
                Refresh();
            }
        }

        #endregion Properties

        #region Private Methods

        protected virtual void Refresh()
        {
            if (IsShowing) return;

            var txt = Title;
            float p = -1;

            if (IsDeterministic)
            {
                p = (float)PercentComplete / 100;

                if (!string.IsNullOrWhiteSpace(txt)) txt += "... ";
                txt += PercentComplete + "%";
            }

            var bounds = new RectangleF((float)UIScreen.MainScreen.Bounds.Left, (float)UIScreen.MainScreen.Bounds.Top, (float)UIScreen.MainScreen.Bounds.Width, (float)UIScreen.MainScreen.Bounds.Height);
            if ((UIApplication.SharedApplication.StatusBarOrientation == UIInterfaceOrientation.LandscapeLeft) || (UIApplication.SharedApplication.StatusBarOrientation == UIInterfaceOrientation.LandscapeRight))
            {
                bounds.Size = new SizeF((float)UIScreen.MainScreen.Bounds.Width, (float)UIScreen.MainScreen.Bounds.Height);
            }

            _LoadingOverlay = new LoadingOverlay(bounds);
            UIView view = Utilities.GetTopView();
            view.Add(this._LoadingOverlay);
        }

        #endregion Private Methods

        #region Public Methods

        public virtual void Hide()
        {
            if (!IsShowing) return;

            IsShowing = false;
            _LoadingOverlay.Hide();
        }

        public virtual void SetCancel(Action onCancel, string cancel)
        {
        }

        public virtual void Show()
        {
            if (IsShowing) return;

            IsShowing = true;
            Refresh();
        }

        #endregion Public Methods

        #region Cleanup

        public void Dispose()
        {
            Hide();
        }

        #endregion Cleanup
    }
}
