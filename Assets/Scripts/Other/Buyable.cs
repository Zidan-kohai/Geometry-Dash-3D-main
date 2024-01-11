using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace GD3D
{
    public class Buyable : MonoBehaviour
    {
        [SerializeField] private int cost;

        private TextMeshProUGUI textMeshPro;


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

        public bool TryBuy(int Coin)
        {
            if (Coin < cost) return false;

            return true;
        }

        public int buy(int coin)
        {
            if (textMeshPro != null)
                textMeshPro.gameObject.SetActive(false);

            SaveData.SaveFile.GoldCoinsCollected = coin - cost;
            return coin - cost;
        }
    }
}
