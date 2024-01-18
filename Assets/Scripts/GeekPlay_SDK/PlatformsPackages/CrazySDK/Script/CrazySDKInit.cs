using UnityEngine;

namespace CrazyGames
{
    class CrazySDKInit
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnRuntimeMethodLoad()
        {
#if UNITY_WEBGL
            CrazySDK.ResetDomain();
            CrazyAds.ResetDomain();
            CrazyEvents.ResetDomain();
            CrazyUser.ResetDomain();
           //SiteLock.Check();
            var sdk = CrazySDK.Instance; // this will create the SDK GameObject
#endif
        }
    }
}