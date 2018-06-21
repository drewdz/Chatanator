using PE.Plugins.Dialogs.WindowsCommon.Controls;

using System;
using System.Threading.Tasks;

using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;

namespace PE.Plugins.Dialogs.WindowsCommon
{
    public class DialogService : IDialogService
    {
        #region Fields

        private readonly DialogConfig _Configuration;

        private bool _RestoreHidden = false;
        private Popup _Popup;
        private IUpdatablePopup _CustomDialog;

        #endregion Fields

        #region Constructors

        public DialogService(DialogConfig config)
        {
            _Configuration = config;
        }

        #endregion  Constructors

        #region Operations

        #region Loading

        public async Task ShowLoadingAsync()
        {
            CreateLoading(string.Empty);
        }

        public async Task ShowLoadingAsync(string message)
        {
            CreateLoading(message);
        }

        public async Task UpdateLoadingAsync(string message)
        {
            if (_CustomDialog == null) return;
            _CustomDialog.Message = message;
        }

        public async Task HideLoadingAsync()
        {
            if (_Popup == null) return;
            RemovePopup();
        }

        #endregion Loading

        #region Progress

        public async Task ShowProgressAsync()
        {
            CreateProgress(string.Empty, 0);
        }

        public async Task ShowProgressAsync(string message)
        {
            CreateProgress(message, 0);
        }

        public async Task ShowProgressAsync(string message, float value)
        {
            CreateProgress(message, value);
        }

        public async Task UpdateProgressAsync(float value)
        {
            if (_CustomDialog == null) return;
            _CustomDialog.Progress = value;
        }

        public async Task UpdateProgressAsync(string message, float value)
        {
            _CustomDialog.Message = message;
            _CustomDialog.Progress = value;
        }

        public async Task HideProgressAsync()
        {
            if (_CustomDialog == null) return;
            RemovePopup();
        }

        #endregion Progress

        #region Alert

        public async Task AlertAsync(string message, string title, string button, Action onOk)
        {
            //  show message dialog
            var dialog = new MessageDialog(message, title);
            dialog.Commands.Add(new UICommand(button));
            await dialog.ShowAsync();
            if (onOk != null) onOk();
        }

        #endregion Alert

        #region Confirm 

        public async Task ConfirmAsync(string message, string title, string positiveButton, Action onPositive, string negativeButton, Action onNegative)
        {
            //  show message dialog
            var dialog = new MessageDialog(message, title);
            dialog.Commands.Add(new UICommand(positiveButton, (cmd) =>
            {
                if (onPositive != null) onPositive();
            }));
            dialog.Commands.Add(new UICommand(negativeButton, (cmd) =>
            {
                if (onNegative != null) onNegative();
            }));
            dialog.DefaultCommandIndex = 0;
            dialog.CancelCommandIndex = 1;
            await _Configuration.Window.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                await dialog.ShowAsync();
            });
        }

        #endregion Confirm

        #region Prompt

        public async Task PromptAsync(string message, string title, string affirmButton, string denyButton, Action<string> onAffirm, Action onDeny, bool password = false, string placeholder = "")
        {
            throw new NotImplementedException();
        }

        #endregion Prompt

        #region Date Picker

        public Task DatePickerAsync(DateTime? date, Action<DateTime> onSet, Action onCancel)
        {
            throw new NotImplementedException();
        }

        #endregion Date Picker

        #endregion Operations

        #region Private Methods

        private void CreateLoading(string text)
        {
            if (_Popup != null)
            {
                _Popup.IsOpen = false;
                _Popup.Child = null;
                _CustomDialog = null;
                _Popup = null;
            }

            _Popup = new Popup();
            _CustomDialog = (_Configuration.CustomLoadingDialog != null) ? _Configuration.CustomLoadingDialog(text) : new CustomLoading();
            _Popup.Child = (UIElement)_CustomDialog;
            ApplicationView window = ApplicationView.GetForCurrentView();
            _Popup.Width = window.VisibleBounds.Width;
            _Popup.Height = window.VisibleBounds.Height;
            _CustomDialog.PopupWidth = window.VisibleBounds.Width;
            _CustomDialog.PopupHeight = window.VisibleBounds.Height;
            _CustomDialog.Message = text;

            _Popup.IsOpen = true;
        }

        private void CreateProgress(string text, float value)
        {
            if (_Popup != null)
            {
                _Popup.IsOpen = false;
                _Popup.Child = null;
                _CustomDialog = null;
                _Popup = null;
            }

            _Popup = new Popup();
            _CustomDialog = new CustomProgress();
            _Popup.Child = (UIElement)_CustomDialog;
            var window = ApplicationView.GetForCurrentView();
            _Popup.Width = window.VisibleBounds.Width;
            _Popup.Height = window.VisibleBounds.Height;
            _CustomDialog.PopupWidth = window.VisibleBounds.Width;
            _CustomDialog.PopupHeight = window.VisibleBounds.Height;
            _CustomDialog.Message = text;
            _CustomDialog.Progress = value;

            _Popup.IsOpen = true;
        }

        private void RemovePopup()
        {
            if (_Popup != null)
            {
                _Popup.IsOpen = false;
                _Popup.Child = null;
                _CustomDialog = null;
                _Popup = null;
            }
        }

        #endregion Private Methods
    }
}
