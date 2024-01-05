using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GD3D
{
    public class Buyable : MonoBehaviour
    {
        private int cost;
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
            return coin - cost;
        }
    }
}
