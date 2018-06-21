using System;

namespace PE.Plugins.Dialogs
{
    public class DialogsConfiguration
    {
        public Func<string, IProgressDialog> CustomLoadingDialog { get; set; }
    }
}
