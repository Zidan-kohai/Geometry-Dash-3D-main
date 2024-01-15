using GD3D.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UI;

namespace GD3D
{
    public class StaticColorPicker : MonoBehaviour
    {
        private Button button;
        private static Color SelectedColor1;
        private static Color SelectedColor2;
        public Buyable buyable;
        public bool isOpened;


        [Tooltip("Get Color from image")]
        [SerializeField] private Image image;
        [SerializeField] private WhitchColor whitchColorChange;
        [Space]
        [SerializeField] private IconKit iconKit;

        private void Awake()
        {
            button = GetComponent<Button>();
            image = GetComponent<Image>();
            buyable = GetComponent<Buyable>();
        }

        private void Start()
        {
            button.onClick.AddListener(ChangePlayerColor);
            
            if(isOpened)
            {
                buyable.buyed();
            }

            switch(whitchColorChange)
            {
                case WhitchColor.Color1:
                    if (SaveData.PlayerData.BuyedColor1.Contains(image.color))
                        buyable.buyed();
                    break;
                case WhitchColor.Color2:
                    if (SaveData.PlayerData.BuyedColor2.Contains(image.color))
                        buyable.buyed();
                    break;
            }
        }

        private void ChangePlayerColor()
        {
            switch (whitchColorChange)
            {
                case WhitchColor.Color1:
                    if (SelectedColor1 == image.color) return;
                    break;
                case WhitchColor.Color2:
                    if (SelectedColor2 == image.color) return;
                    break;
            }

            if (!buyable.TryBuyForGold(SaveData.PlayerData.GoldCoinsCollected) && !buyable.IsBuyed) return;

            if (!buyable.IsBuyed)
            {
                buyable.buyForFold(SaveData.PlayerData.GoldCoinsCollected);
                
                switch(whitchColorChange)
                {
                    case WhitchColor.Color1:
                        SaveData.PlayerData.BuyedColor1.Add(image.color);
                        break;
                    case WhitchColor.Color2:
                        SaveData.PlayerData.BuyedColor2.Add(image.color);
                        break;
                }

                SaveData.Save();
            }

            if (whitchColorChange == WhitchColor.Color1)
            {
                iconKit.UpdatePlayerColor1(image.color);
                SelectedColor1 = image.color;
            }
            else
            {
                iconKit.UpdatePlayerColor2(image.color);
                SelectedColor2 = image.color;
            }


            iconKit.ChangeGoldCoin();
        }


        public enum WhitchColor
        {
            Color1,
            Color2
        }
    }
}
