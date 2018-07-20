namespace Chatanator.iOS
{
    public class LinkerPleaseInclude
    {
        public void Include(PE.Plugins.Dialogs.iOS.DialogService service)
        {
            var s = service;
        }

        public void Include(PE.Plugins.PubnubChat.Plugin service)
        {
            var s = service;
        }

        public void Include(PE.Plugins.Validation.Plugin service)
        {
            var s = service;
        }
    }
}