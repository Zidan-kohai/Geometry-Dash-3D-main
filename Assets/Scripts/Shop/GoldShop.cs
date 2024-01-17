using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GD3D
{
    public class GoldShop : MonoBehaviour
    {
        public static GoldShop Instance { get; private set; }

        public UnityEvent GetPurchase;
        public Button GetGoldByReward;
        public GameObject LacksMoney;
        private void Start()
        {
            if(Instance != null)
            {
                Destroy(gameObject);
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        public void SubscribeToPurchase(string tag)
        {
            Geekplay.Instance.RealBuyItem(tag);
        }

        public void addGold(int count)
        {
            Geekplay.Instance.PlayerData.GoldCoinsCollected += count;
            SaveData.Save();
            GetPurchase?.Invoke();
        }
        public void addDiamond(int count)
        {
            Geekplay.Instance.PlayerData.DiamondCoinsCollected += count;
            SaveData.Save();
            GetPurchase?.Invoke();
        }

    }
}
