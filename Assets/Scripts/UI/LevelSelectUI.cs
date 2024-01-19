using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GD3D
{
    public class LevelSelectUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text goldCoinText;
        [SerializeField] private TMP_Text diamondCoinText;

        [SerializeField] private Button getGoldByReward1;

        [SerializeField] private GameObject UIHolder;

        private void Start()
        {
            goldCoinText.text = Geekplay.Instance.PlayerData.GoldCoinsCollected.ToString();
            diamondCoinText.text = Geekplay.Instance.PlayerData.DiamondCoinsCollected.ToString();

            getGoldByReward1.onClick.AddListener(() =>
            {
                Geekplay.Instance.ShowRewardedAd("GetFiveGoldCoin");
            });

            GoldShop.Instance.GetGoldByReward.onClick.AddListener(() =>
            {
                Geekplay.Instance.ShowRewardedAd("GetFiveGoldCoin");
            });

            Geekplay.Instance.SubscribeOnReward("GetFiveGoldCoin", GetGold);
            Geekplay.Instance.SubscribeOnReward("GetFiveGoldCoin", ChangeCoin);

            GoldShop.Instance.GetPurchase.AddListener(ChangeCoin);

            GoldShop.Instance.OpenShop.AddListener(() =>
            {
                UIHolder.SetActive(false);
            });

            GoldShop.Instance.CloseShop.AddListener(() =>
            {
                UIHolder.SetActive(true);
            });

        }

        private void GetGold()
        {
            Geekplay.Instance.PlayerData.GoldCoinsCollected += 5000;

            SaveData.Save();
        }

        public void ChangeCoin()
        {
            goldCoinText.text = Geekplay.Instance.PlayerData.GoldCoinsCollected.ToString();

            diamondCoinText.text = Geekplay.Instance.PlayerData.DiamondCoinsCollected.ToString();
        }
    }
}
    
