using AndroidHUD;

using PE.Framework.Droid;

using System;

namespace PE.Plugins.Dialogs.Droid
{
    public class ProgressDialog : IProgressDialog
    {
        #region Fields

        private Action _CancelAction;

        private string _CancelText;

        private int _PercentComplete;

        private string _Title;

        #endregion Field

        #region Methods

        #region Private

        private void OnCancelClick()
        {
            if (_CancelAction == null)
            {
                return;
            }

            Hide();
            _CancelAction();
        }

        #endregion Private

        #region Protected

        protected virtual void Refresh()
        {
            if (!IsShowing)
            {
                return;
            }

            var p = -1;
            var txt = Title;

            if (IsDeterministic)
            {
                p = PercentComplete;

                if (!string.IsNullOrWhiteSpace(txt))
                {
                    txt += "\n";
                }

                txt += p + "%\n";
            }

            if (_CancelAction != null)
            {
                txt += "\n" + _CancelText;
            }

            Utilities.Dispatch(() => AndHUD.Shared.Show(Utilities.GetActivityContext(), txt, p, MaskType.Black, null, OnCancelClick));
        }

        #endregion Protected

        #region Public

        public void Dispose()
        {
            Hide();
        }

        public virtual void Hide()
        {
            IsShowing = false;
            Utilities.Dispatch(() => AndHUD.Shared.Dismiss(Utilities.GetActivityContext()));
        }

        public virtual void SetCancel(Action onCancel, string cancel)
        {
            _CancelAction = onCancel;
            _CancelText = cancel;
        }

        public virtual void Show()
        {
            if (IsShowing)
            {
                return;
            }

            IsShowing = true;

            Refresh();
        }

        #endregion Public

        #endregion Methods

        #region Properties

        public virtual bool IsDeterministic { get; set; }

        public virtual bool IsShowing { get; private set; }

        public virtual int PercentComplete
        {
            get { return _PercentComplete; }
            set
            {
                if (_PercentComplete == value)
                {
                    return;
                }

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

        public virtual string Title
        {
            get { return _Title; }
            set
            {
                if (_Title == value)
                {
                    return;
                }

                _Title = value;

                Refresh();
            }
        }

        #endregion Properties
    }
}