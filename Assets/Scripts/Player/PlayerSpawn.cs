using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GD3D.ObjectPooling;
using GD3D.Easing;
using GD3D.Level;
using GD3D.UI;
using UnityEngine.SceneManagement;

namespace GD3D.Player
{
    /// <summary>
    /// Controls the player spawning and showing the respawn menu/new best popup.
    /// </summary>
    public class PlayerSpawn : PlayerScript
    {
        [SerializeField] private int poolSize = 4;
        private ObjectPool<PoolObject> _pool;

        [SerializeField] private PoolObject respawnRing;

        [SerializeField] private float respawnTime;

        [SerializeField] private TMP_Text attemptText;
        private int _currentAttempt = 1;
        public int CurrentAttemp => _currentAttempt;
            
        [Header("Lose Menu")]
        [SerializeField] private GameObject loseMenu;
        [SerializeField] private EaseSettings respawnMenuEaseSettings;

        [Space]
        [SerializeField] private TMP_Text loseMenuLevelName;
        [SerializeField] private TMP_Text loseMenuAttemptText;
        [SerializeField] private Slider loseMenuProgressBar;
        [SerializeField] private TMP_Text loseMenuProgressPercent;
        [SerializeField] private TMP_Text loseMenuJumpText;
        [SerializeField] private TMP_Text loseMenuTimeText;
        [SerializeField] private TMP_Text loseGoldText;
        [SerializeField] private TMP_Text loseDiamondText;
        [SerializeField] private Button loseReviveButton;

        [Space]
        [SerializeField] private Button loseRestartButton;
        [SerializeField] private Button loseQuitButton;

        [Header("Win Menu")]
        [SerializeField] private GameObject winMenu;

        [Space]
        [SerializeField] private TMP_Text winMenuLevelName;
        [SerializeField] private TMP_Text winMenuJumpText;
        [SerializeField] private TMP_Text winMenuTimeText;
        [SerializeField] private TMP_Text winGoldText;
        [SerializeField] private TMP_Text winDiamondText;

        [Space]
        [SerializeField] private Button winRestartButton;
        [SerializeField] private Button winQuitButton;
        [SerializeField] private Button doubleAwardButton;

        private int goldReward = 0;
        private int diamondReward = 0;

        private UIClickable[] _respawnMenuUIClickables;

        private long? _respawnSizeEaseID = null;
        private Vector3 _respawnMenuStartSize;
        private Transform _respawnMenuTransform;

        [Header("New Best Popup")]
        [SerializeField] private GameObject newBestPopup;
        [SerializeField] private TMP_Text newBestPercentText;

        [Space]
        [SerializeField] private EaseSettings newBestShowEaseSettings;
        [SerializeField] private EaseSettings newBestHideEaseSettings;

        private long? _newBestSizeEaseID = null;
        private Vector3 _newBestStartSize;
        private Transform _newBestTransform;

        //-- Other
        private Coroutine _currentRespawnCoroutine;
        private PlayerData _saveFile;
        private PlayerPracticeMode _practiceMode;
        private int gameCount;

        /// <summary>
        /// Start is called before the first frame update.
        /// </summary>
        public override void Start()
        {
            base.Start();
            loseDiamondText.text = $"{0}";
            _currentAttempt = Geekplay.Instance.CurrentLevelData.TotalAttempts + 1;
            // Set the save file
            _saveFile = Geekplay.Instance.PlayerData;

            attemptText.text = $"Attempt  {_currentAttempt}";
            loseMenuAttemptText.text = $"Attempt  {_currentAttempt}";

            // Setup respawn menu
            _respawnMenuTransform = loseMenu.transform;
            _respawnMenuStartSize = _respawnMenuTransform.localScale;

            loseMenuLevelName.text = LevelData.Instance.LevelName;

            loseRestartButton.onClick.AddListener(Respawn);
            loseQuitButton.onClick.AddListener(QuitToMenu);
            loseReviveButton.onClick.AddListener(BeforeRevive);

            Geekplay.Instance.SubscribeOnReward("Revive", Revive);

            winRestartButton.onClick.AddListener(Respawn);
            winQuitButton.onClick.AddListener(QuitToMenu);
            doubleAwardButton.onClick.AddListener(() => {
                Geekplay.Instance.ShowRewardedAd("doubleAward");
            });

            Geekplay.Instance.SubscribeOnReward("doubleAward", OnDoubleAward);

            loseMenu.SetActive(false);
            winMenu.SetActive(false);

            _respawnMenuUIClickables = loseMenu.GetComponentsInChildren<UIClickable>();
            //winMenu.GetComponentsInChildren<UIClickable>();

            // Setup new best popup
            _newBestTransform = newBestPopup.transform;
            _newBestStartSize = _newBestTransform.localScale;

            newBestPopup.SetActive(false);

            // Get the player practice mode script
            _practiceMode = player.PracticeMode;

            // Subscribe to the OnDeath event
            //player.OnDeath += OnDeath;

            EasingManager.Instance.OnEaseObjectRemove += OnEaseObjectRemove;

            // Setup the respawnRing obj by creating a copy and setting the copy
            GameObject obj = Instantiate(respawnRing.gameObject, transform.position, Quaternion.identity, transform);
            obj.transform.position = _transform.position;

            // Change the line renderers color
            LineRenderer lr = obj.GetComponent<LineRenderer>();
            lr.startColor = PlayerColor1;
            lr.endColor = PlayerColor1;

            // Create pool
            _pool = new ObjectPool<PoolObject>(obj, poolSize,
                (poolObj) =>
                {
                    poolObj.transform.SetParent(_transform);
                    poolObj.transform.localPosition = Vector3.zero;
                }
            );

            PlayerMain.Instance.OnWin += ShowWinMenu;
            PlayerMain.Instance.OnDeath += ShowLoseMenu;
            // Destroy the newly created object because we have no use out of it anymore
            Destroy(obj);

            gameCount = 0;
        }

        private void OnDoubleAward()
        {
            Geekplay.Instance.PlayerData.GoldCoinsCollected += goldReward;
            Geekplay.Instance.PlayerData.DiamondCoinsCollected += diamondReward;


            goldReward *= 2;
            diamondReward *= 2;

            winGoldText.text = goldReward.ToString();
            winDiamondText.text = diamondReward.ToString();

            doubleAwardButton.gameObject.SetActive(false);
        }

        private void OnEaseObjectRemove(long id)
        {
            // Set respawn size ease ID to null if the respawn size ease got removed
            if (_respawnSizeEaseID.HasValue && id == _respawnSizeEaseID.Value)
            {
                _respawnSizeEaseID = null;
            }

            // Do the same thing for the new best size easing
            if (_newBestSizeEaseID.HasValue && id == _newBestSizeEaseID.Value)
            {
                _newBestSizeEaseID = null;
            }
        }

        #region Respawn Menu

        /// <summary>
        /// Makes the respawn menu appear with a scale easing.
        /// </summary>
        private void ShowLoseMenu()
        {

            SoundManager.Instance.StopMainAudio();

            // Disable the pause menu so you can't pause
            PauseMenu.CanPause = false;

            // Set objects on the respawn menu
            loseMenuAttemptText.text = attemptText.text;

            loseMenuJumpText.text = $"Jumps: {PlayerMain.TimesJumped}";

            // Use TimeSpan here to format the text to look nice :)
            TimeSpan time = TimeSpan.FromSeconds(PlayerMain.TimeSpentPlaying);
            loseMenuTimeText.text = $"Time: {time.ToString("mm':'ss")}";

            switch (SceneManager.GetActiveScene().buildIndex)
            {
                case 4:
                    loseGoldText.text = (Convert.ToInt32(ProgressBar.Percent * 100) * 100).ToString();
                    break;
                case 5:
                    loseGoldText.text = (Convert.ToInt32(ProgressBar.Percent * 100) * 150).ToString();
                    break;
                case 6:
                    loseGoldText.text = (Convert.ToInt32(ProgressBar.Percent * 100) * 200).ToString();
                    break;
                case 7:
                    loseGoldText.text = (Convert.ToInt32(ProgressBar.Percent * 100) * 250).ToString();
                    break;
                case 8:
                    loseGoldText.text = (Convert.ToInt32(ProgressBar.Percent * 100) * 300).ToString();
                    loseDiamondText.text = Convert.ToInt32(ProgressBar.Percent * 10).ToString();
                    break;
                case 9:
                    loseDiamondText.text = Convert.ToInt32(ProgressBar.Percent * 10).ToString();
                    loseGoldText.text = (Convert.ToInt32(ProgressBar.Percent * 100) * 350).ToString();
                    break;
            }

            goldReward = Convert.ToInt32(loseGoldText.text);
            diamondReward = Convert.ToInt32(loseDiamondText.text);

            loseMenuProgressBar.normalizedValue = ProgressBar.Percent;
            loseMenuProgressPercent.text = ProgressBar.PercentString;

            // Enable respawn menu
            loseMenu.SetActive(true);

            // Try remove ease object
            EasingManager.TryRemoveEaseObject(_respawnSizeEaseID);

            // Set the respawn menu scale to 0
            _respawnMenuTransform.localScale = Vector3.zero;

            // Create new scale easing
            EaseObject ease = _respawnMenuTransform.EaseScale(_respawnMenuStartSize, 1);


            // Set ease settings
            ease.SetSettings(respawnMenuEaseSettings);

            // Set ease ID
            _respawnSizeEaseID = ease.ID;

            StartCoroutine(ShowADV());


            if (gameCount >= 3)
            {
                Geekplay.Instance.RateGameFunc();
            }
        }

        private void ShowWinMenu()
        {
            switch (SceneManager.GetActiveScene().buildIndex)
            {
                case 4:
                    Geekplay.Instance.PlayerData.LeaderboardPointds += Convert.ToInt32(ProgressBar.Percent * 100);
                    break;
                case 5:
                    Geekplay.Instance.PlayerData.LeaderboardPointds += Convert.ToInt32(ProgressBar.Percent * 200);
                    break;
                case 6:
                    Geekplay.Instance.PlayerData.LeaderboardPointds += Convert.ToInt32(ProgressBar.Percent * 300);
                    break;
                case 7:
                    Geekplay.Instance.PlayerData.LeaderboardPointds += Convert.ToInt32(ProgressBar.Percent * 400);
                    break;
                case 8:
                    Geekplay.Instance.PlayerData.LeaderboardPointds += Convert.ToInt32(ProgressBar.Percent * 500);
                    break;
                case 9:
                    Geekplay.Instance.PlayerData.LeaderboardPointds += Convert.ToInt32(ProgressBar.Percent * 600);
                    break;
            }

            Geekplay.Instance.Leaderboard("Points", Geekplay.Instance.PlayerData.LeaderboardPointds);

            // Disable the pause menu so you can't pause
            PauseMenu.CanPause = false;

            // Set objects on the respawn menu

            winMenuJumpText.text = $"Jumps: {PlayerMain.TimesJumped}";

            // Use TimeSpan here to format the text to look nice :)
            TimeSpan time = TimeSpan.FromSeconds(PlayerMain.TimeSpentPlaying);
            winMenuTimeText.text = $"Time: {time.ToString("mm':'ss")}";

            
            switch (SceneManager.GetActiveScene().buildIndex)
            {
                case 4:
                    winGoldText.text = $"{12500}";
                    winDiamondText.text = $"0";
                    Geekplay.Instance.PlayerData.GoldCoinsCollected += 12500;
                    Geekplay.Instance.PlayerData.DiamondCoinsCollected += 0;
                    break;
                case 5:
                    winGoldText.text = $"{18000}";
                    winDiamondText.text = $"0";
                    Geekplay.Instance.PlayerData.GoldCoinsCollected += 18000;
                    Geekplay.Instance.PlayerData.DiamondCoinsCollected += 0;
                    break;
                case 6:
                    winGoldText.text = $"{24000}";
                    winDiamondText.text = $"0";
                    Geekplay.Instance.PlayerData.GoldCoinsCollected += 24000;
                    Geekplay.Instance.PlayerData.DiamondCoinsCollected += 0;
                    break;
                case 7:
                    winGoldText.text = $"{30000}";
                    winDiamondText.text = $"0";
                    Geekplay.Instance.PlayerData.GoldCoinsCollected += 30000;
                    Geekplay.Instance.PlayerData.DiamondCoinsCollected += 0;
                    break;
                case 8:
                    winGoldText.text = $"{40000}";
                    winDiamondText.text = $"0";
                    Geekplay.Instance.PlayerData.GoldCoinsCollected += 40000;
                    Geekplay.Instance.PlayerData.DiamondCoinsCollected += (int)ProgressBar.Percent * 10;
                    break;
                case 9:
                    winGoldText.text = $"{50000}";
                    winDiamondText.text = $"0";
                    Geekplay.Instance.PlayerData.GoldCoinsCollected += 50000;
                    Geekplay.Instance.PlayerData.DiamondCoinsCollected += (int)ProgressBar.Percent * 10;
                    break;
            }

            goldReward = Convert.ToInt32(winGoldText.text);
            diamondReward = Convert.ToInt32(winDiamondText.text);

            SaveData.Save();

            // Enable win menu
            winMenu.SetActive(true);

            // Try remove ease object
            EasingManager.TryRemoveEaseObject(_respawnSizeEaseID);

            // Set the respawn menu scale to 0
            _respawnMenuTransform.localScale = Vector3.zero;

            // Create new scale easing
            EaseObject ease = _respawnMenuTransform.EaseScale(_respawnMenuStartSize, 1);


            // Set ease settings
            ease.SetSettings(respawnMenuEaseSettings);

            // Set ease ID
            _respawnSizeEaseID = ease.ID;

            StartCoroutine(ShowADV());


            if (gameCount >= 3)
            {
                Geekplay.Instance.RateGameFunc();
            }
        }

        IEnumerator ShowADV()
        {
            yield return new WaitForSeconds(2);

            Geekplay.Instance.ShowInterstitialAd();
        }

        /// <summary>
        /// Transitions to the main menu.
        /// </summary>
        public void QuitToMenu()
        {
            _currentAttempt++;
            Geekplay.Instance.CurrentLevelData.TotalAttempts++;
            Geekplay.Instance.PlayerData.GoldCoinsCollected += goldReward;
            Geekplay.Instance.PlayerData.DiamondCoinsCollected += diamondReward;


            switch (SceneManager.GetActiveScene().buildIndex)
            {
                case 4:
                    Geekplay.Instance.PlayerData.LeaderboardPointds += Convert.ToInt32(ProgressBar.Percent * 100);
                    break;
                case 5:
                    Geekplay.Instance.PlayerData.LeaderboardPointds += Convert.ToInt32(ProgressBar.Percent * 200);
                    break;
                case 6:
                    Geekplay.Instance.PlayerData.LeaderboardPointds += Convert.ToInt32(ProgressBar.Percent * 300);
                    break;
                case 7:
                    Geekplay.Instance.PlayerData.LeaderboardPointds += Convert.ToInt32(ProgressBar.Percent * 400);
                    break;
                case 8:
                    Geekplay.Instance.PlayerData.LeaderboardPointds += Convert.ToInt32(ProgressBar.Percent * 500);
                    break;
                case 9:
                    Geekplay.Instance.PlayerData.LeaderboardPointds += Convert.ToInt32(ProgressBar.Percent * 600);
                    break;
            }


            SaveData.Save();



            AudioListener.volume = 1;

            Geekplay.Instance.Leaderboard("Points", Geekplay.Instance.PlayerData.LeaderboardPointds);

            Transition.TransitionToLastActiveMenu();

        }
        #endregion

        #region New Best Popup
        /// <summary>
        /// Makes the new best popup appear with a scale easing. <para/>
        /// Is called in <see cref="LevelData"/>.
        /// </summary>
        public void ShowNewBest()
        {
            // Set the percent text on the new best popup
            newBestPercentText.text = ProgressBar.PercentString;

            // Enable new best popuo
            newBestPopup.SetActive(true);

            // Try remove ease object
            EasingManager.TryRemoveEaseObject(_newBestSizeEaseID);

            // Set the new best popup scale to 0
            _newBestTransform.localScale = Vector3.zero;

            // Create new scale easing
            EaseObject ease = _newBestTransform.EaseScale(_newBestStartSize, 1);

            // Set ease settings
            ease.SetSettings(newBestShowEaseSettings);

            // Set ease ID
            _newBestSizeEaseID = ease.ID;

            // Make the easing dissapear when it's complete
            ease.SetOnComplete((obj) =>
            {
                // Create new scale easing
                EaseObject ease = _newBestTransform.EaseScale(Vector3.zero, 1);

                // Set ease settings
                ease.SetSettings(newBestHideEaseSettings);

                // Set ease ID
                _newBestSizeEaseID = ease.ID;

            });
        }
        #endregion


        ///<summary>
        ///is called on win for next scene
        ///</summary>
        
        private void nextScene()
        {
            _currentAttempt++;
            Geekplay.Instance.CurrentLevelData.TotalAttempts++;

            SaveData.Save();

            Transition.TransitionToNextScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        /// <summary>
        /// Is called when the player dies.
        /// </summary>
        public void Spawn()
        {
            // Increase attempt count
            _currentAttempt++;
            Geekplay.Instance.CurrentLevelData.TotalAttempts++;

            // Disable the mesh
            player.Mesh.ToggleCurrentMesh(false);

            Coroutine coroutine;

            // Check if auto retry is enabled
            // But also auto retry if we are in practice mode since there is no respawn menu in practice mode
            if (PlayerPracticeMode.InPracticeMode || _saveFile.AutoRetryEnabled)
            {
                // Respawn after 1 second if auto retry is enabled or if we are in practice mode
                coroutine = Helpers.TimerSeconds(this, 1, Respawn);
            }
            else
            {
                // Bring up the respawn menu after 1 second if auto retry is disabled
                coroutine = Helpers.TimerSeconds(this, 1, ShowLoseMenu);
            }

            StartRespawnCouroutine(coroutine);
        }

        private void StartRespawnCouroutine(Coroutine coroutine)
        {
            // Stop the currently active respawn coroutine
            if (_currentRespawnCoroutine != null)
            {
                StopCoroutine(_currentRespawnCoroutine);
            }

            // Set the new respawn coroutine to the given coroutine
            _currentRespawnCoroutine = coroutine;
        }

        /// <summary>
        /// Respawns the player.
        /// </summary>
        /// 

        #region Respawn
        public void Respawn()
        {
            loseReviveButton.gameObject.SetActive(true);
            _practiceMode.Checkpoints.Clear();

            switch (SceneManager.GetActiveScene().buildIndex)
            {
                case 4:
                    Geekplay.Instance.PlayerData.LeaderboardPointds += Convert.ToInt32(ProgressBar.Percent * 100);
                    break;
                case 5:
                    Geekplay.Instance.PlayerData.LeaderboardPointds += Convert.ToInt32(ProgressBar.Percent * 200);
                    break;
                case 6:
                    Geekplay.Instance.PlayerData.LeaderboardPointds += Convert.ToInt32(ProgressBar.Percent * 300);
                    break;
                case 7:
                    Geekplay.Instance.PlayerData.LeaderboardPointds += Convert.ToInt32(ProgressBar.Percent * 400);
                    break;
                case 8:
                    Geekplay.Instance.PlayerData.LeaderboardPointds += Convert.ToInt32(ProgressBar.Percent * 500);
                    break;
                case 9:
                    Geekplay.Instance.PlayerData.LeaderboardPointds += Convert.ToInt32(ProgressBar.Percent * 600);
                    break;
            }

            Geekplay.Instance.Leaderboard("Points", Geekplay.Instance.PlayerData.LeaderboardPointds);

            // Enable the pause menu so you can pause again
            PauseMenu.CanPause = true;

            _currentAttempt++;
            Geekplay.Instance.CurrentLevelData.TotalAttempts++;

            Geekplay.Instance.PlayerData.GoldCoinsCollected += goldReward;
            Geekplay.Instance.PlayerData.DiamondCoinsCollected += diamondReward;

            SaveData.Save();
            // Disable the respawn menu and new best popups
            loseMenu.SetActive(false);
            winMenu.SetActive(false);   
            newBestPopup.SetActive(false);

            // Stop the scaling of all the UI Clickables on the respawn menu
            foreach (UIClickable clickable in _respawnMenuUIClickables)
            {
                clickable.StopScaling();
            }

            bool inPracticeMode = PlayerPracticeMode.InPracticeMode;
            Checkpoint checkpoint = null;

            if (inPracticeMode)
            {
                checkpoint = _practiceMode.LatestCheckpoint;
                checkpoint?.OnLoaded();
            }

            // Invoke respawn event
            player.InvokeRespawnEvent(inPracticeMode && checkpoint != null, checkpoint);

            // Set attempt text
            attemptText.text = $"Attempt  {_currentAttempt}";
            // Reset jumps and time because they are static

            PlayerMain.TimesJumped = 0;
            PlayerMain.TimeSpentPlaying = 0;

            // Start the respawn coroutine
            Coroutine coroutine = StartCoroutine(RespawnCouroutine());
            StartRespawnCouroutine(coroutine);

            // Ignore input for this moment so the player won't instantly jump when respawning
            player.IgnoreInput();

            gameCount++;


            SoundManager.Instance.PlayMainAudio();
        }

        public void BeforeRevive()
        {
            Geekplay.Instance.ShowRewardedAd("Revive");
        }

        public void Revive()
        {

            loseReviveButton.gameObject.SetActive(false);

            // Enable the pause menu so you can pause again
            PauseMenu.CanPause = true;

            // Disable the respawn menu and new best popups
            loseMenu.SetActive(false);
            winMenu.SetActive(false);
            newBestPopup.SetActive(false);

            // Stop the scaling of all the UI Clickables on the respawn menu
            foreach (UIClickable clickable in _respawnMenuUIClickables)
            {
                clickable.StopScaling();
            }

            Checkpoint checkpoint = _practiceMode.LatestCheckpoint;
            checkpoint?.OnLoaded();


            // Invoke respawn event
            player.InvokeRespawnEvent(true, checkpoint);


            // Start the respawn coroutine
            Coroutine coroutine = StartCoroutine(RespawnCouroutine());
            StartRespawnCouroutine(coroutine);

            // Ignore input for this moment so the player won't instantly jump when respawning
            player.IgnoreInput();


            SoundManager.Instance.PlayMainAudio();
        }

        #endregion

        /// <summary>
        /// Makes the player flash on/off and spawn respawn rings 3 times.
        /// </summary>
        
        private IEnumerator RespawnCouroutine()
        {
            // Make the player flash on/off and spawn respawn rings every time the player is turned on
            // Do this 3 times total over the course of 0.6 seconds
            SpawnRespawnRing();

            PlayerMesh mesh = player.Mesh;

            mesh.ToggleCurrentMesh(true);

            for (int i = 0; i < 3; i++)
            {
                yield return Helpers.GetWaitForSeconds(0.05f);

                mesh.ToggleCurrentMesh(false);

                yield return Helpers.GetWaitForSeconds(0.05f);

                SpawnRespawnRing();
                mesh.ToggleCurrentMesh(true);
            }
        }

        /// <summary>
        /// Spawns a respawn ring.
        /// </summary>
        private void SpawnRespawnRing()
        {
            // Spawn the ring
            PoolObject obj = _pool.SpawnFromPool(_transform.position);
            obj?.RemoveAfterTime(0.5f);
        }
    }
}
