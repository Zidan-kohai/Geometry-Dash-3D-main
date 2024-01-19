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
        public GameObject Shop;

        public UnityEvent OpenShop;
        public UnityEvent CloseShop;

        private void Awake()
        {
            if(Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            Geekplay.Instance.SubscribeOnPurchase("kiti1", GetKiti1);
            Geekplay.Instance.SubscribeOnPurchase("kiti2", GetKiti2);
            Geekplay.Instance.SubscribeOnPurchase("kiti3", GetKiti3);
            Geekplay.Instance.SubscribeOnPurchase("kiti4", GetKiti4);
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
            Geekplay.Instance.PlayerData.DiamondCoinsCollected += 3;
            Geekplay.Instance.PlayerData.IsbyedKiti1 = true;
        }

        public void GetKiti2()
        {
            Geekplay.Instance.PlayerData.GoldCoinsCollected += 25000;
            Geekplay.Instance.PlayerData.DiamondCoinsCollected += 250;
            Geekplay.Instance.PlayerData.IsbyedKiti2 = true;
            //дать набор

        }

        public void GetKiti3()
        {
            Geekplay.Instance.PlayerData.GoldCoinsCollected += 15000;
            Geekplay.Instance.PlayerData.DiamondCoinsCollected += 3500;
            Geekplay.Instance.PlayerData.IsbyedKiti3 = true;
        }

        public void GetKiti4()
        {
            Geekplay.Instance.PlayerData.IsbyedKiti4 = true;
            //дать все цвета и наборы
        }


        public void OnOpenShop()
        {
            Debug.Log("Gold Shopp Open");
            OpenShop?.Invoke();
        }

        public void OnCloseShop()
        {
            Debug.Log("Gold Shopp Close");
            CloseShop?.Invoke();
        }
    }
}
