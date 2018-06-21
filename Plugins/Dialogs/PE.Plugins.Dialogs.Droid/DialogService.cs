using Android.App;
using Android.Content;
using Android.Widget;
using MvvmCross;
using PE.Framework.Droid;

using System;
using System.Threading.Tasks;
using MvvmCross.Platform.Droid.Platform;
using PE.Framework.AndroidApp.AppVersion;

namespace PE.Plugins.Dialogs.Droid
{
    public class DialogService : IDialogService
    {
        #region Fields

        private IProgressDialog _Progress;

        readonly DialogConfig _Config;

        #endregion Fields

        #region Constructors

        public DialogService(DialogConfig config)
        {
            _Config = config;
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
                _Progress = (_Config.CustomLoadingDialog != null) ? _Config.CustomLoadingDialog(message) : Loading(message, null, null, true);
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
            Utilities.Dispatch(() =>
            {
                Context context = GetActivityContext();

                var txt = new TextView(context)
                {
                    Text = string.Format("\n{0}\n", message),
                    Gravity = Android.Views.GravityFlags.Center,
                    TextSize = 18
                };

                AlertDialog.Builder builder = new AlertDialog.Builder(context);
                AlertDialog dialog = builder
                    .SetCancelable(false)
                    .SetView(txt)
                    .SetTitle(title)
                    .SetPositiveButton(button, (o, e) =>
                    {
                        if (onOk != null) onOk();
                    }).Show();

                //dialog.Show();
            });
        }

        #endregion Alert

        #region Confirm

        public async Task ConfirmAsync(string message, string title, string positiveButton, Action onPositive, string negativeButton, Action onNegative)
        {
            Utilities.Dispatch(() =>
            {
                Context context = GetActivityContext();
                var txt = new TextView(context)
                {
                    Text = string.Format("\n{0}\n", message),
                    Gravity = Android.Views.GravityFlags.Center,
                    TextSize = 18
                };

                new AlertDialog.Builder(context)
                    .SetCancelable(false)
                    .SetView(txt)
                    .SetTitle(title)
                    .SetPositiveButton(positiveButton, (o, e) =>
                    {
                        if (onPositive != null) onPositive();
                    })
                    .SetNegativeButton(negativeButton, (o, e) =>
                    {
                        if (onNegative != null) onNegative();
                    })
                    .Show();
            });
        }

        #endregion Confirm

        #region Prompt

        public async Task PromptAsync(string message, string title, string affirmButton, string denyButton, Action<string> onAffirm, Action onDeny, bool password = false, string placeholder = "")
        {
            Utilities.Dispatch(() =>
            {
                Context context = GetActivityContext();
                var txt = new EditText(context)
                {
                    Hint = placeholder
                };
                if (password) txt.InputType = Android.Text.InputTypes.TextVariationPassword;

                new AlertDialog
                    .Builder(context)
                    .SetMessage(message)
                    .SetTitle(title)
                    .SetView(txt)
                    .SetPositiveButton(affirmButton, (o, e) =>
                    {
                        if (onAffirm != null) onAffirm(txt.Text);
                    })
                    .SetNegativeButton(denyButton, (o, e) =>
                    {
                        if (onDeny != null) onDeny();
                    })
                    .Show();
            });
        }

        #endregion Prompt

        #region Date Picker

        public async Task DatePickerAsync(DateTime? date, Action<DateTime> onSet, Action onCancel)
        {
            var dt = (date == null) ? DateTime.Now : date.Value;

            Utilities.Dispatch(() =>
            {
                var dlg = new DatePickerDialog(GetActivityContext(), (o, e) =>
                {
                    onSet?.Invoke(e.Date);
                }, dt.Year, dt.Month, dt.Day);
                dlg.Show();
            });
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

        private Context GetActivityContext()
        {
            Context context = Utilities.GetActivityContext();
            if (context == null)
            {
                IMvxAndroidCurrentTopActivity topActivity;
                bool canResolve = Mvx.TryResolve(out topActivity);

                IAndroidApp app = Mvx.Resolve<IAndroidApp>();
                if (app != null)
                {
                    context = (Context)app.TopActivity;
                }
            }
            return context;
        }

        #endregion Private
    }
}