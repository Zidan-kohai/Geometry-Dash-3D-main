using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
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
        [Space]
        [SerializeField] private GameObject lacksMoneyPopup;


        [SerializeField] private TMP_Text levelNameText;
        [SerializeField] private TMP_Text starAmountText;
        public Image DifficultyFace;

        [Space]
        [SerializeField] private Slider normalProgressBar;
        [SerializeField] private Slider practiceProgressBar;

        [Space]
        [SerializeField] private TMP_Text normalProgressText;
        [SerializeField] private TMP_Text practiceProgressText;

        private void Start()
        {
            _levelName = LevelData.LevelName;

            levelNameText.text = _levelName;
            starAmountText.text = LevelData.StartAwarded.ToString();
            DifficultyFace.sprite = LevelData.DifficultyFace;

            // Update the progress bars with data from the JSON save file
            PlayerData.LevelSaveData levelSave = null;
            levelSave = SaveData.PlayerData.LevelData.FirstOrDefault((levelData) => levelData.Name == _levelName);

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
                buttonOpen1Text.text = "Open by viewing ads";
                buttonOpen2Text.text = $"Open by {LevelData.cost} Gold";

                buttonOpen1.onClick.AddListener(OpenLevelByShowReward);
                buttonOpen2.onClick.AddListener(OpenLevelByGold);
            }
            else if (LevelData.LevelBuildIndex == 8 || LevelData.LevelBuildIndex == 9)
            {
                buttonOpen1Text.text = "Open by In app";
                buttonOpen2Text.text = $"Open by {LevelData.cost} Diamond";

                buttonOpen1.onClick.AddListener(OpenLevelByInApp);
                buttonOpen2.onClick.AddListener(OpenLevelByDiamond);
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
            SoundManager.PlaySound("Play Level", 1);
        }


        public void OpenLevelByGold()
        {
            if (LevelData.cost > SaveData.PlayerData.GoldCoinsCollected)
            {
                lacksMoneyPopup.SetActive(true);
                return;
            }

            SaveData.PlayerData.GoldCoinsCollected -= LevelData.cost;
            LevelData.IsOpen = true;
            SaveData.PlayerData.GetLevelData(LevelData.LevelName).isOpen = true;
            SaveData.Save();

            openButtonPanel.SetActive(false);
        }

        public void OpenLevelByDiamond()
        {
            if (LevelData.cost > SaveData.PlayerData.DiamondCoinsCollected)
            {
                lacksMoneyPopup.SetActive(true);
                return;
            }

            SaveData.PlayerData.DiamondCoinsCollected -= LevelData.cost;
            LevelData.IsOpen = true;
            SaveData.PlayerData.GetLevelData(LevelData.LevelName).isOpen = true;
            SaveData.Save();

            openButtonPanel.SetActive(false);
        }

        public void OpenLevelByShowReward()
        {
            openButtonPanel.SetActive(false);
            LevelData.IsOpen = true;
            SaveData.PlayerData.GetLevelData(LevelData.LevelName).isOpen = true;
        }

        public void OpenLevelByInApp()
        {
            openButtonPanel.SetActive(false);
            LevelData.IsOpen = true;
            SaveData.PlayerData.GetLevelData(LevelData.LevelName).isOpen = true;
        }
    }
}
