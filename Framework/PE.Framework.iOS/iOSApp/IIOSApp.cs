using OpenId.AppAuth;

namespace PE.Framework.iOS.iOSApp
{
    public interface IIOSApp
    {
        void SetAuthorizationFlowSession(IAuthorizationFlowSession session);
    }
}
