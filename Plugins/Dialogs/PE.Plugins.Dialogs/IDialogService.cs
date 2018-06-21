using System;
using System.Threading.Tasks;

namespace PE.Plugins.Dialogs
{
    public interface IDialogService
    {
        #region Loading 

        Task ShowLoadingAsync();

        Task ShowLoadingAsync(string message);

        Task UpdateLoadingAsync(string message);

        Task HideLoadingAsync();

        #endregion Loading

        #region Progress

        Task ShowProgressAsync();

        Task ShowProgressAsync(string message);

        Task ShowProgressAsync(string message, float value);

        Task UpdateProgressAsync(float value);

        Task UpdateProgressAsync(string message, float value);

        Task HideProgressAsync();

        #endregion Progress

        #region Alert

        Task AlertAsync(string message, string title, string button, Action onOk);

        #endregion Alert

        #region Confirm

        Task ConfirmAsync(string message, string title, string positiveButton, Action onPositive, string negativeButton, Action onNegative);

        #endregion Confirm

        #region Prompt

        Task PromptAsync(string message, string title, string affirmButton, string denyButton, Action<string> onAffirm, Action onDeny, bool password = false, string placeholder = "");

        #endregion Prompt

        #region Date Picker

        Task DatePickerAsync(DateTime? date, Action<DateTime> onSet, Action onCancel);

        #endregion Date Picker
    }
}
