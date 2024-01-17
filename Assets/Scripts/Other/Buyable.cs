using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace GD3D
{
    public class Buyable : MonoBehaviour
    {
        [SerializeField] private int cost;

        [SerializeField] private TextMeshProUGUI textMeshPro;

        public bool IsBuyed = false;
        private void Start()
        {
            textMeshPro = GetComponentInChildren<TextMeshProUGUI>();
        }
        public int Cost 
        { 
            get 
            {
                return cost; 
            }
            set
            {
                cost = value;
                cost = Mathf.Clamp(cost, 0, int.MaxValue);
            }
        }

        public bool TryBuyForGold(int Coin)
        {
            if (Coin < cost)
            {
                GoldShop.Instance.LacksMoney.SetActive(true);
                return false;
            }

            return true;
        }

        public int buyForFold(int coin)
        {
            if (textMeshPro != null)
                textMeshPro.gameObject.SetActive(false);

            Geekplay.Instance.PlayerData.GoldCoinsCollected = coin - cost;
            IsBuyed = true;

            return coin - cost;
        }

        public void buyed()
        {
            if (textMeshPro != null)
            {
                textMeshPro.gameObject.SetActive(false);
            }
            else
            {
                Debug.Log(gameObject.name);
            }

            IsBuyed = true;
        }
    }
}
