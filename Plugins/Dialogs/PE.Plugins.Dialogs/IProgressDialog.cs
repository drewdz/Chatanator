using System;

namespace PE.Plugins.Dialogs
{
    public interface IProgressDialog : IDisposable
    {
        void Hide();

        void SetCancel(Action onCancel, string cancelText = "Cancel");

        void Show();

        bool IsDeterministic { get; set; }

        bool IsShowing { get; }

        int PercentComplete { get; set; }

        string Title { get; set; }
    }
}
