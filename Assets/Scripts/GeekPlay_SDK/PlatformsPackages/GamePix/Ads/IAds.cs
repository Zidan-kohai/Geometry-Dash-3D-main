namespace GamePix.Ads
{
    public interface IAds
    {
        void InterstitialAd(Gpx.gpxCallback onSuccess = null, Gpx.gpxCallback onFail = null);
        void RewardAd(Gpx.gpxCallback onSuccess, Gpx.gpxCallback onFail);
    }
}