using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GD3D
{
    public class GoldShop : MonoBehaviour
    {
        public void addGold(int count)
        {
            SaveData.SaveFile.GoldCoinsCollected += count;
            SaveData.Save();
        }
        public void addDiamond(int count)
        {
            SaveData.SaveFile.DiamondCoinsCollected += count;
            SaveData.Save();
        }

    }
}
