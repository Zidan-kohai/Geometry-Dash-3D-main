using TMPro;
using UnityEngine;

namespace GD3D
{
    public class Buyable : MonoBehaviour
    {
        [SerializeField] private int cost;
        public bool UseText;
        
        [SerializeField] private TextMeshProUGUI textMeshPro;

        public bool IsBuyed = false;
        private void Start()
        {
            if(UseText)
                textMeshPro = GetComponentInChildren<TextMeshProUGUI>();

            if(Geekplay.Instance.PlayerData.IsbyedKiti4)
            {
                buyed();
            }

            Geekplay.Instance.SubscribeOnPurchase("kiti4", () =>
            {
                buyed();
            });
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
                textMeshPro.text = cost.ToString();
            }
        }

        public bool TryBuy(int Coin)
        {
            if (Coin < cost)
            {
                GoldShop.Instance.LacksMoney.SetActive(true);
                return false;
            }

            return true;
        }
        public int buyForGold(int coin)
        {
            if (textMeshPro != null)
                textMeshPro.gameObject.SetActive(false);

            Geekplay.Instance.PlayerData.GoldCoinsCollected = coin - cost;
            IsBuyed = true;

            return coin - cost;
        }

        public int buyForDiamond(int coin)
        {
            if (textMeshPro != null)
                textMeshPro.gameObject.SetActive(false);

            Geekplay.Instance.PlayerData.DiamondCoinsCollected = coin - cost;
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
