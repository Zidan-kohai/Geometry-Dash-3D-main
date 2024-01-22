using System.Linq;
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

            if(LevelData.IsOpen || LevelData.LevelBuildIndex == 4 || LevelData.LevelBuildIndex == 5)
            {
                openButtonPanel.gameObject.SetActive(false);
                LevelData.IsOpen = true;
            }
            else if(LevelData.LevelBuildIndex == 6 || LevelData.LevelBuildIndex == 7)
            {
                if (Geekplay.Instance.language == "en")
                {
                    buttonOpen1Text.text = "Open by viewing ads";
                    buttonOpen2Text.text = $"Open by {LevelData.cost} Gold";

                }
                else if (Geekplay.Instance.language == "ru")
                {
                    buttonOpen1Text.text = "открыть за рекламу";
                    buttonOpen2Text.text = $"открыть за {LevelData.cost} золото";

                    levelNameText.text = _levelName;
                }
                else if (Geekplay.Instance.language == "tu")
                {
                    buttonOpen1Text.text = "Reklamlari görüntüleyerek açin";
                    buttonOpen2Text.text = $"{LevelData.cost} Altınla Açıldı";
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

                buttonOpen2.onClick.AddListener(OpenLevelByGold);
            }
            else if (LevelData.LevelBuildIndex == 8 || LevelData.LevelBuildIndex == 9)
            {

                if (Geekplay.Instance.language == "en")
                {
                    buttonOpen1Text.text = "Open by 20 yan";
                    buttonOpen2Text.text = $"Open by {LevelData.cost} Diamond";
                }
                else if (Geekplay.Instance.language == "ru")
                {
                    buttonOpen1Text.text = "купить уровень 20 ян";
                    buttonOpen2Text.text = $"открыть за {LevelData.cost} алмаз";
                }
                else if (Geekplay.Instance.language == "tu")
                {
                    buttonOpen1Text.text = "Saat 20'ye kadar açılıyor";
                    buttonOpen2Text.text = $"{LevelData.cost} Diamond tarafından açıldı";
                }

                buttonOpen2.onClick.AddListener(OpenLevelByDiamond);
                
                if(LevelData.LevelBuildIndex == 8)
                {
                    buttonOpen1.onClick.AddListener(OpenLevel5ByInApp);
                    Geekplay.Instance.SubscribeOnPurchase("openLevel5",OpenLevelByInApp);
                }
                else
                {
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
                    else if (Geekplay.Instance.language == "tu")
                    {
                        levelNameText.text = "Stereo Çılgınlığı";
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
                    else if (Geekplay.Instance.language == "tu")
                    {
                        levelNameText.text = "İz arkasında";
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
                    else if (Geekplay.Instance.language == "tu")
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
                    else if (Geekplay.Instance.language == "tu")
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
                    else if (Geekplay.Instance.language == "tu")
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
                    else if (Geekplay.Instance.language == "tu")
                    {
                        levelNameText.text = "Bırakamıyorum";
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
