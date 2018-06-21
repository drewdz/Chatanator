namespace PE.Plugins.Dialogs
{
    public interface IUpdatablePopup
    {
        #region Properties

        float Progress { set; }

        string Message { set; }

        double PopupWidth { set; }

        double PopupHeight { set; }

        #endregion Properties
    }
}
