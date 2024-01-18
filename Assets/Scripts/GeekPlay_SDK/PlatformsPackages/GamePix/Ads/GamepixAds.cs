using System.Runtime.InteropServices;

namespace GamePix.Ads
{
    public class GamepixAds : IAds
    {
        [DllImport("__Internal")]
        private static extern void gpxInterstitialAd(
                    [MarshalAs(UnmanagedType.FunctionPtr)] Gpx.gpxCallback onSuccess,
                    [MarshalAs(UnmanagedType.FunctionPtr)] Gpx.gpxCallback onFail);
        
        [DllImport("__Internal")]
        private static extern void gpxRewardAd(
                    [MarshalAs(UnmanagedType.FunctionPtr)] Gpx.gpxCallback onSuccess, 
                    [MarshalAs(UnmanagedType.FunctionPtr)] Gpx.gpxCallback onFail);

        public void InterstitialAd(Gpx.gpxCallback onSuccess = null, Gpx.gpxCallback onFail = null)
        {
            gpxInterstitialAd(onSuccess, onFail);
        }

        public void RewardAd(Gpx.gpxCallback onSuccess, Gpx.gpxCallback onFail)
        {            
            gpxRewardAd(onSuccess, onFail);
        }
    }
}