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
            Debug.Log("get Kiti 1");
        }

        public void GetKiti2()
        {
            Debug.Log("get Kiti 2");
        }

        public void GetKiti3()
        {
            Debug.Log("get Kiti 3");
        }

        public void GetKiti4()
        {
            Debug.Log("get Kiti 4");
        }
    }
}
