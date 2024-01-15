#if UNITY_EDITOR || !UNITY_WEBGL
namespace GamePix.Ads
{
    public class UnityAds: IAds
    {
        public void InterstitialAd(Gpx.gpxCallback onSuccess = null, Gpx.gpxCallback onFail = null)
        {
            Gpx.Log("[Gpx] InterstitialAd");
            onSuccess?.Invoke();
        }

        public void RewardAd(Gpx.gpxCallback onSuccess, Gpx.gpxCallback onFail)
        {     
            Gpx.Log("[Gpx] RewardAd");       
            onSuccess();
        }
    }
}
#endif