using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class Utils : MonoBehaviour
{
    //ОБЩИЕ МЕТОДЫ//
    [DllImport("__Internal")]
    public static extern void GamePlatform();
    //ОБЩИЕ МЕТОДЫ//

    //МЕТОДЫ YANDEX//
    [DllImport("__Internal")]
    public static extern void RateGame();

    [DllImport("__Internal")]
    public static extern string GetDomain();

    [DllImport("__Internal")]
    public static extern void SaveExtern(string date);

    [DllImport("__Internal")]
    public static extern void LoadExtern();

    [DllImport("__Internal")]
    public static extern string GetLang();

    [DllImport("__Internal")]
    public static extern void AdInterstitial();

    [DllImport("__Internal")]
    public static extern void AdReward();

    [DllImport("__Internal")]
    public static extern void SetToLeaderboard(int value, string leaderboardName);

    [DllImport("__Internal")]
    public static extern void BuyItem(string idOrTag, string jsonString);

    [DllImport("__Internal")]
    public static extern void CheckBuyItem(string idOrTag);

    [DllImport("__Internal")]
    public static extern void GameReady();

    [DllImport("__Internal")]
    public static extern void GetLeaderboard(string type, int number);
    //МЕТОДЫ YANDEX//

    //МЕТОДЫ ВК//
    [DllImport("__Internal")]
    public static extern void VK_Star();

    [DllImport("__Internal")]
    public static extern void VK_Share();

    [DllImport("__Internal")]
    public static extern void VK_Invite();

    [DllImport("__Internal")]
    public static extern void VK_ToGroup();

    [DllImport("__Internal")]
    public static extern void VK_Banner();

    [DllImport("__Internal")]
    public static extern void VK_AdInterCheck();

    [DllImport("__Internal")]
    public static extern void VK_AdRewardCheck();

    [DllImport("__Internal")]
    public static extern void VK_Interstitial();

    [DllImport("__Internal")]
    public static extern void VK_Rewarded();

    [DllImport("__Internal")]
    public static extern void VK_OpenLeaderboard(int value);

    [DllImport("__Internal")]
    public static extern void VK_Load();

    [DllImport("__Internal")]
    public static extern void VK_Save(string saveData);
    //МЕТОДЫ ВК//

    //МЕТОДЫ ЛONGREGATE//
    [DllImport("__Internal")]
    public static extern void Kongregate_InApp(string itemName);
    //МЕТОДЫ ЛONGREGATE//

    //МЕТОДЫ OK//
    [DllImport("__Internal")]
    public static extern void OK_Interstitial();

    [DllImport("__Internal")]
    public static extern void OK_ShowRating();

    [DllImport("__Internal")]
    public static extern void OK_LoadRewardedAd();

    [DllImport("__Internal")]
    public static extern void OK_RequestBannerAds();

    [DllImport("__Internal")]
    public static extern void OK_ShowRewardedAd();

    [DllImport("__Internal")]
    public static extern void OK_ShowBannerAds();
    //МЕТОДЫ OK//
}
