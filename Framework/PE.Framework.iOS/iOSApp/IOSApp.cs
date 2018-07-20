using OpenId.AppAuth;
using UIKit;

namespace PE.Framework.iOS.iOSApp
{
    public class IOSApp : IIOSApp
    {
        public void SetAuthorizationFlowSession(IAuthorizationFlowSession session)
        {
            ((IIOSApp)UIApplication.SharedApplication.Delegate).SetAuthorizationFlowSession(session);
        }
    }
}
