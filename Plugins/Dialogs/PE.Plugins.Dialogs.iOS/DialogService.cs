using PE.Framework.iOS;

using System;
using System.Threading.Tasks;

using UIKit;

namespace PE.Plugins.Dialogs.iOS
{
    public class DialogService : IDialogService
    {
        #region Fields

        private readonly DialogsConfiguration _Configuration;

        private IProgressDialog _Progress;
        private UIAlertView _Alert;

        #endregion Fields

        #region Constructors

        public DialogService(DialogsConfiguration configuration)
        {
            _Configuration = configuration;
        }

        #endregion Constructors

        #region Loading

        public async Task ShowLoadingAsync()
        {
            ShowLoadingAsync(string.Empty);
        }

        public async Task ShowLoadingAsync(string message)
        {
            if (_Progress == null)
            {
                _Progress = (_Configuration.CustomLoadingDialog != null) ? _Configuration.CustomLoadingDialog(message) : Loading(message, null, null, true);
            }
        }

        public async Task UpdateLoadingAsync(string message)
        {
            if (_Progress == null) return;
            _Progress.Title = message;
        }

        public async Task HideLoadingAsync()
        {
            if (_Progress == null) return;
            _Progress.Hide();
            _Progress.Dispose();
            _Progress = null;
        }

        #endregion Loading

        #region Progress

        public async Task ShowProgressAsync()
        {
            ShowProgressAsync(string.Empty);
        }

        public async Task ShowProgressAsync(string message)
        {
            if (_Progress == null)
            {
                _Progress = Progress(message, null, string.Empty, true, false);
            }

            _Progress.Title = message;
        }

        public async Task ShowProgressAsync(string message, float value)
        {
            if (_Progress == null)
            {
                _Progress = Progress(message, null, string.Empty, true, true);
            }

            _Progress.Title = message;
            _Progress.PercentComplete = (int)(value * 100);
        }

        public async Task UpdateProgressAsync(float value)
        {
            if (_Progress == null) return;
            _Progress.PercentComplete = (int)(value * 100);
        }

        public async Task UpdateProgressAsync(string message, float value)
        {
            if (_Progress == null) return;
            _Progress.PercentComplete = (int)(value * 100);
            _Progress.Title = message;
        }

        public async Task HideProgressAsync()
        {
            if (_Progress == null) return;
            _Progress.Hide();
            _Progress.Dispose();
            _Progress = null;
        }

        #endregion Progress

        #region Alert

        public async Task AlertAsync(string message, string title, string button, Action onOk)
        {
            if (_Alert != null)
            {
                CancelAlert();
            }

            Utilities.Dispatch(() =>
            {
                _Alert = new UIAlertView(title ?? String.Empty, message, null, null, button);

                if (onOk != null)
                {
                    _Alert.Clicked += (s, e) => onOk();
                }

                _Alert.Show();
            });
        }

        #endregion Alert

        #region Confirm

        public async Task ConfirmAsync(string message, string title, string positiveButton, Action onPositive, string negativeButton, Action onNegative)
        {
            if (_Alert != null)
            {
                CancelAlert();
            }

            Utilities.Dispatch(() =>
            {
                _Alert = new UIAlertView(title ?? String.Empty, message, null, negativeButton, positiveButton);

                _Alert.Clicked += (s, e) =>
                {
                    if (_Alert.CancelButtonIndex == e.ButtonIndex)
                    {
                        if (onNegative != null) onNegative();
                    }
                    else
                    {
                        if (onPositive != null) onPositive();
                    }
                };
                _Alert.Show();
            });
        }

        #endregion Confirm

        #region Prompt

        public async Task PromptAsync(string message, string title, string affirmButton, string denyButton, Action<string> onAffirm, Action onDeny, bool password = false, string placeholder = "")
        {
            if (_Alert != null)
            {
                CancelAlert();
            }

            Utilities.Dispatch(() =>
            {
                _Alert = new UIAlertView(title ?? String.Empty, message, null, denyButton, affirmButton)
                {
                    AlertViewStyle = password ? UIAlertViewStyle.SecureTextInput : UIAlertViewStyle.PlainTextInput
                };
                var txt = (UITextField)(_Alert.GetTextField(0));

                txt.SecureTextEntry = password;
                txt.Placeholder = placeholder;

                _Alert.Clicked += (s, e) =>
                {
                    if (_Alert.CancelButtonIndex == e.ButtonIndex)
                    {
                        if (onDeny != null) onDeny();
                    }
                    else
                    {
                        if (onAffirm != null) onAffirm(txt.Text);
                    }
                };

                _Alert.Show();
            });
        }

        #endregion Prompt

        #region Date Picker

        public async Task DatePickerAsync(DateTime? date, Action<DateTime> onSet, Action onCancel)
        {
            throw new System.NotImplementedException();
            //var dt = (date == null) ? DateTime.Now : date.Value;

            //Utilities.Dispatch(() =>
            //{
            //    var dlg = new DatePickerDialog(Utilities.GetActivityContext(), (o, e) =>
            //    {
            //        onSet?.Invoke(e.Date);
            //    }, dt.Year, dt.Month, dt.Day);
            //    dlg.Show();
            //});
        }

        #endregion Date Picker

        #region Private

        private IProgressDialog Loading(string title, Action onCancel, string cancelText, bool show)
        {
            return Progress(title, onCancel, cancelText, show, false);
        }

        private IProgressDialog Progress(string title, Action onCancel, string cancelText, bool show, bool deterministic)
        {
            var dlg = new ProgressDialog();

            dlg.Title = title;
            dlg.IsDeterministic = deterministic;

            if (onCancel != null)
            {
                dlg.SetCancel(onCancel, cancelText);
            }

            if (show)
            {
                dlg.Show();
            }

            return dlg;
        }

        public void CancelAlert()
        {
            if (_Alert == null) return;
            Utilities.Dispatch(() =>
            {
                _Alert.DismissWithClickedButtonIndex(-1, true);
            });
        }

        #endregion Private
    }
}