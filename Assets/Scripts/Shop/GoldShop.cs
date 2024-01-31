using GD3D.Player;
using GD3D.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
        public GameObject Shop;


        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            Geekplay.Instance.SubscribeOnPurchase("kiti1", GetKiti1);
            Geekplay.Instance.SubscribeOnPurchase("kiti2", GetKiti2);
            Geekplay.Instance.SubscribeOnPurchase("kiti3", GetKiti3);
            //Geekplay.Instance.SubscribeOnPurchase("kiti4", GetKiti4);
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
        public void GetKiti1()
        {
            Geekplay.Instance.PlayerData.GoldCoinsCollected += 15000;
            Geekplay.Instance.PlayerData.DiamondCoinsCollected += 50;
            Geekplay.Instance.PlayerData.IsbyedKiti1 = true;
        }

        public void GetKiti2()
        {
            Geekplay.Instance.PlayerData.GoldCoinsCollected += 25000;
            Geekplay.Instance.PlayerData.DiamondCoinsCollected += 250;
            Geekplay.Instance.PlayerData.IsbyedKiti2 = true;

            Geekplay.Instance.PlayerData.SaveBuyedIconIndex(Gamemode.cube, 19);
            Geekplay.Instance.Save();
        }

        public void GetKiti3()
        {
            Geekplay.Instance.PlayerData.IsbyedKiti3 = true;
            Geekplay.Instance.PlayerData.SaveBuyedIconIndex(Gamemode.cube, 10);
            Geekplay.Instance.PlayerData.SaveBuyedIconIndex(Gamemode.cube, 11);
            Geekplay.Instance.PlayerData.SaveBuyedIconIndex(Gamemode.cube, 17);

            Geekplay.Instance.Save();
        }
    }
}
