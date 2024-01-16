using System;
using System.Collections;
using System.Collections.Generic;
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
        [SerializeField] private Button getGoldByReward2;

        private void Start()
        {
            goldCoinText.text = SaveData.PlayerData.GoldCoinsCollected.ToString();
            diamondCoinText.text = SaveData.PlayerData.DiamondCoinsCollected.ToString();

            getGoldByReward1.onClick.AddListener(() =>
            {
                Geekplay.Instance.ShowRewardedAd("GetFiveGoldCoin");
            });
            getGoldByReward2.onClick.AddListener(() =>
            {
                Geekplay.Instance.ShowRewardedAd("GetFiveGoldCoin");
            });

            Geekplay.Instance.SubscribeOnReward("GetFiveGoldCoin", GetGold);
            Geekplay.Instance.SubscribeOnReward("GetFiveGoldCoin", ChangeGoldCoin);
        }

        private void GetGold()
        {
            SaveData.PlayerData.GoldCoinsCollected += 5000;

            SaveData.Save();
        }

        private void ChangeGoldCoin()
        {
            goldCoinText.text = SaveData.PlayerData.GoldCoinsCollected.ToString();
        }

        private void ChangeDiamondCoin()
        {
            diamondCoinText.text = SaveData.PlayerData.DiamondCoinsCollected.ToString();
        }
    }
}
    
