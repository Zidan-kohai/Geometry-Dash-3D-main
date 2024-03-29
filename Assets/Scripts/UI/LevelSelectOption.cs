﻿using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GD3D.Audio;

namespace GD3D.UI
{
    /// <summary>
    /// 
    /// </summary>
    public class LevelSelectOption : MonoBehaviour
    {
        //-- Settings
        public LevelSelect.LevelData LevelData;
        private string _levelName;


        [Header("Objects")]
        [SerializeField] private GameObject openButtonPanel;
        [SerializeField] private Button buttonOpen1;
        [SerializeField] private Button buttonOpen2;
        [SerializeField] private TMP_Text buttonOpen1Text;
        [SerializeField] private TMP_Text buttonOpen2Text;
        [SerializeField] private GameObject VideoIcon;

        [SerializeField] private TMP_Text levelNameText;
        [SerializeField] private TMP_Text starAmountText;
        public Image DifficultyFace;

        [Space]
        [SerializeField] private Slider normalProgressBar;
        [SerializeField] private Slider practiceProgressBar;

        [Space]
        [SerializeField] private TMP_Text normalProgressText;
        [SerializeField] private TMP_Text practiceProgressText;
        [SerializeField] private GameObject isGiveDiamond;
        [SerializeField] private TextMeshProUGUI PointsCountGiveText;

        [SerializeField] private EnableRewards enableRewards;

        [SerializeField] private LevelSelectUI levelSelectUI;
        private void Start()
        {
            _levelName = LevelData.LevelName;            

            starAmountText.text = LevelData.StartAwarded.ToString();
            DifficultyFace.sprite = LevelData.DifficultyFace;

            // Update the progress bars with data from the JSON save file
            PlayerData.LevelSaveData levelSave = null;
            levelSave = Geekplay.Instance.PlayerData.LevelData.FirstOrDefault((levelData) => levelData.Name == _levelName);

            if (levelSave == null)
            {
                normalProgressBar.normalizedValue = 0;
                practiceProgressBar.normalizedValue = 0;

                normalProgressText.text = "0%";
                practiceProgressText.text = "0%";
            }
            else
            {
                normalProgressBar.normalizedValue = levelSave.NormalPercent;
                practiceProgressBar.normalizedValue = levelSave.PracticePercent;

                normalProgressText.text = ProgressBar.ToPercent(levelSave.NormalPercent);
                practiceProgressText.text = ProgressBar.ToPercent(levelSave.PracticePercent);
            }


            if (Geekplay.Instance.language == "en")
            {
                PointsCountGiveText.text = $"1% - {LevelData.LevelBuildIndex  - 3} tournament points";
            }
            else if (Geekplay.Instance.language == "ru")
            {
                PointsCountGiveText.text = $"1% - {LevelData.LevelBuildIndex - 3} турнирных очка";
            }
            else if (Geekplay.Instance.language == "tr")
            {
                PointsCountGiveText.text = $"1% - {LevelData.LevelBuildIndex - 3} turnuva puanları";
            }



            if (LevelData.LevelBuildIndex == 8 || LevelData.LevelBuildIndex == 9)
            {
                if (Geekplay.Instance.language == "en")
                {
                    PointsCountGiveText.text = $"1% - {LevelData.LevelBuildIndex - 3} tournament points";
                }
                else if (Geekplay.Instance.language == "ru")
                {
                    PointsCountGiveText.text = $"1% - {LevelData.LevelBuildIndex - 3} турнирных очков";
                }
                else if (Geekplay.Instance.language == "tr")
                {
                    PointsCountGiveText.text = $"1% - {LevelData.LevelBuildIndex - 3} turnuva puanları";
                }
            }


            if (LevelData.IsOpen || LevelData.LevelBuildIndex == 4 || LevelData.LevelBuildIndex == 5)
            {
                openButtonPanel.gameObject.SetActive(false);
                LevelData.IsOpen = true;
            }
            else if(LevelData.LevelBuildIndex == 6 || LevelData.LevelBuildIndex == 7)
            {
                if (Geekplay.Instance.language == "en")
                {
                    buttonOpen1Text.text = "Open";
                    buttonOpen2Text.text = $"Open by {LevelData.cost} Gold";
                }
                else if (Geekplay.Instance.language == "ru")
                {
                    buttonOpen1Text.text = "Открыть";
                    buttonOpen2Text.text = $"открыть за {LevelData.cost} золота";

                    levelNameText.text = _levelName;
                }
                else if (Geekplay.Instance.language == "tr")
                {
                    buttonOpen1Text.text = "Açin";
                    buttonOpen2Text.text = $"{LevelData.cost} Altınla Açildi";
                }


                VideoIcon.SetActive(true);

                if (LevelData.LevelBuildIndex == 6)
                {
                    buttonOpen1.onClick.AddListener(BeforeOpenLevel3ByShowReward);
                    Geekplay.Instance.SubscribeOnReward("OpenLevel3", OpenLevelByShowReward);
                }

                if (LevelData.LevelBuildIndex == 7)
                {
                    buttonOpen1.onClick.AddListener(BeforeOpenLevel4ByShowReward);
                    Geekplay.Instance.SubscribeOnReward("OpenLevel4", OpenLevelByShowReward);
                }

                enableRewards.addRewardsButton(buttonOpen1.gameObject);

                buttonOpen2.onClick.AddListener(OpenLevelByGold);
            }
            else if (LevelData.LevelBuildIndex == 8 || LevelData.LevelBuildIndex == 9)
            {

                if (Geekplay.Instance.language == "en")
                {
                    buttonOpen1Text.text = "Open by 50 Yan";
                    buttonOpen2Text.text = $"Open by {LevelData.cost} diamond";
                }
                else if (Geekplay.Instance.language == "ru")
                {
                    buttonOpen1Text.text = "Открыть за 50 Ян";
                    buttonOpen2Text.text = $"открыть за {LevelData.cost} алмазов";
                }
                else if (Geekplay.Instance.language == "tr")
                {
                    buttonOpen1Text.text = "Saat 50 Yan kadar açilıyor";
                    buttonOpen2Text.text = $"{LevelData.cost} Diamond tarafindan açildı";
                }

                buttonOpen2.onClick.AddListener(OpenLevelByDiamond);
                
                if(LevelData.LevelBuildIndex == 8)
                {
                    if (Geekplay.Instance.language == "en")
                    {
                        buttonOpen1Text.text = "Open by 50 Yan";
                    }
                    else if (Geekplay.Instance.language == "ru")
                    {
                        buttonOpen1Text.text = "Открыть за 50 Ян";
                    }
                    else if (Geekplay.Instance.language == "tr")
                    {
                        buttonOpen1Text.text = "Saat 50 Yan kadar açiliyor";
                    }
                    buttonOpen1.onClick.AddListener(OpenLevel5ByInApp);
                    Geekplay.Instance.SubscribeOnPurchase("openLevel5",OpenLevelByInApp);
                }
                else
                {
                    if (Geekplay.Instance.language == "en")
                    {
                        buttonOpen1Text.text = "Open by 100 Yan";
                    }
                    else if (Geekplay.Instance.language == "ru")
                    {
                        buttonOpen1Text.text = "Открыть за 100 Ян";
                    }
                    else if (Geekplay.Instance.language == "tr")
                    {
                        buttonOpen1Text.text = "Saat 100 Yan kadar açiliyor";
                    }
                    buttonOpen1.onClick.AddListener(OpenLevel6ByInApp);
                    Geekplay.Instance.SubscribeOnPurchase("openLevel6", OpenLevelByInApp);
                }
            }



            switch (LevelData.LevelBuildIndex)
            {
                case 4:
                    if (Geekplay.Instance.language == "en")
                    {
                        levelNameText.text = _levelName;
                    }
                    else if (Geekplay.Instance.language == "ru")
                    {
                        levelNameText.text = "Стерео безумие";
                    }
                    else if (Geekplay.Instance.language == "tr")
                    {
                        levelNameText.text = "Stereo Çilginliğı";
                    }
                    break;
                case 5:
                    if (Geekplay.Instance.language == "en")
                    {
                        levelNameText.text = _levelName;
                    }
                    else if (Geekplay.Instance.language == "ru")
                    {
                        levelNameText.text = "Снова в путь";
                    }
                    else if (Geekplay.Instance.language == "tr")
                    {
                        levelNameText.text = "Iz arkasinda";
                    }
                    break;
                case 6:
                    if (Geekplay.Instance.language == "en")
                    {
                        levelNameText.text = _levelName;
                    }
                    else if (Geekplay.Instance.language == "ru")
                    {
                        levelNameText.text = "Полярность";
                    }
                    else if (Geekplay.Instance.language == "tr")
                    {
                        levelNameText.text = "Polargeist";
                    }
                    break;
                case 7:
                    if (Geekplay.Instance.language == "en")
                    {
                        levelNameText.text = _levelName;
                    }
                    else if (Geekplay.Instance.language == "ru")
                    {
                        levelNameText.text = "Засуха";
                    }
                    else if (Geekplay.Instance.language == "tr")
                    {
                        levelNameText.text = "Kurutma";
                    }
                    break;
                case 8:
                    if (Geekplay.Instance.language == "en")
                    {
                        levelNameText.text = _levelName;
                    }
                    else if (Geekplay.Instance.language == "ru")
                    {
                        levelNameText.text = "База за базой";
                    }
                    else if (Geekplay.Instance.language == "tr")
                    {
                        levelNameText.text = "Tabandan Sonra Taban";
                    }
                    break;
                case 9:
                    if (Geekplay.Instance.language == "en")
                    {
                        levelNameText.text = _levelName;
                    }
                    else if (Geekplay.Instance.language == "ru")
                    {
                        levelNameText.text = "Не могу отпустить";
                    }
                    else if (Geekplay.Instance.language == "tr")
                    {
                        levelNameText.text = "Birakamiyorum";
                    }
                    break;
            }

            if(LevelData.LevelBuildIndex == 8 || LevelData.LevelBuildIndex == 9)
            {
                isGiveDiamond.SetActive(true);
            }
        }

        private void Update()
        {
        
        }

        /// <summary>
        /// Plays the level.
        /// </summary>
        public void PlayLevel()
        {
            if (Transition.IsTransitioning || !LevelData.IsOpen)
            {
                return;
            }

            if(!Geekplay.Instance.PlayerData.isFirstLevelButtonTapped)
            {
                Geekplay.Instance.PlayerData.isFirstLevelButtonTapped = true;
                Analytics.Instance.SendEvent("First_Level_Button_Clicked");
                Geekplay.Instance.Save();
            }
            if (!Geekplay.Instance.PlayerData.isSecondLevelButtonTapped)
            {
                Geekplay.Instance.PlayerData.isSecondLevelButtonTapped = true;
                Analytics.Instance.SendEvent("Second_Level_Button_Clicked");
                Geekplay.Instance.Save();
            }
            Transition.TransitionToScene(LevelData.LevelBuildIndex);

            // Stop the music
            MainMenuMusic.StopInstance();

            // Play sound effect
            //SoundManager.PlaySound("Play Level", 1);
        }


        public void OpenLevelByGold()
        {
            if (LevelData.cost > Geekplay.Instance.PlayerData.GoldCoinsCollected)
            {
                GoldShop.Instance.LacksMoney.SetActive(true);
                return;
            }

            Geekplay.Instance.PlayerData.GoldCoinsCollected -= LevelData.cost;
            LevelData.IsOpen = true;
            Geekplay.Instance.PlayerData.GetLevelData(LevelData.LevelName).isOpen = true;
            SaveData.Save();

            openButtonPanel.SetActive(false);
            levelSelectUI.ChangeCoin();
        }

        public void OpenLevelByDiamond()
        {
            if (LevelData.cost > Geekplay.Instance.PlayerData.DiamondCoinsCollected)
            {
                GoldShop.Instance.LacksMoney.SetActive(true);
                return;
            }

            Geekplay.Instance.PlayerData.DiamondCoinsCollected -= LevelData.cost;
            LevelData.IsOpen = true;
            Geekplay.Instance.PlayerData.GetLevelData(LevelData.LevelName).isOpen = true;
            SaveData.Save();

            openButtonPanel.SetActive(false);
            levelSelectUI.ChangeCoin();
        }

        public void BeforeOpenLevel3ByShowReward()
        {
            Geekplay.Instance.ShowRewardedAd("OpenLevel3");
        }

        public void BeforeOpenLevel4ByShowReward()
        {
            Geekplay.Instance.ShowRewardedAd("OpenLevel4");
        }

        public void OpenLevelByShowReward()
        {
            openButtonPanel.SetActive(false);
            LevelData.IsOpen = true;
            Geekplay.Instance.PlayerData.GetLevelData(LevelData.LevelName).isOpen = true;
        }

        public void OpenLevel5ByInApp()
        {
            Geekplay.Instance.RealBuyItem("openLevel5");
        }

        public void OpenLevel6ByInApp()
        {
            Geekplay.Instance.RealBuyItem("openLevel6");
        }

        public void OpenLevelByInApp()
        {
            openButtonPanel.SetActive(false);
            LevelData.IsOpen = true;
            Geekplay.Instance.PlayerData.GetLevelData(LevelData.LevelName).isOpen = true;
        }
    }
}
