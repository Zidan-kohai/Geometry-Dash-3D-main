using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace GD3D
{
    public class EnableRewards : MonoBehaviour
    {
        [SerializeField] private GameObject[] rewards;

        public TextMeshProUGUI goldText;
        public TextMeshProUGUI diamondText;

        private void OnEnable()
        {
            if (goldText != null)
                goldText.text = Geekplay.Instance.PlayerData.GoldCoinsCollected.ToString();
            if (diamondText != null)
                diamondText.text = Geekplay.Instance.PlayerData.DiamondCoinsCollected.ToString();

            GoldShop.Instance.GetPurchase.AddListener(ChangeCoin);
        }

        void Update()
        {
            for (int i = 0; i < rewards.Length; i++)
            {
                if (Geekplay.Instance.canReward)
                {
                    rewards[i].SetActive(true);
                }
                else
                {
                    rewards[i].SetActive(false);
                }
            }
        }

        public void addRewardsButton(GameObject rewardButton)
        {
            GameObject[] newButtons = rewards;

            rewards = new GameObject[newButtons.Length + 1];

            for(int i = 0; i < newButtons.Length; i++)
            {
                rewards[i] = newButtons[i];
            }

            rewards[rewards.Length - 1] = rewardButton;
        }

        public void ChangeCoin()
        {
            if (goldText == null || diamondText == null) return;

            goldText.text = Geekplay.Instance.PlayerData.GoldCoinsCollected.ToString();

            diamondText.text = Geekplay.Instance.PlayerData.DiamondCoinsCollected.ToString();
        }

        private void OnDisable()
        {
            GoldShop.Instance.GetPurchase.RemoveListener(ChangeCoin);
        }
    }
}
