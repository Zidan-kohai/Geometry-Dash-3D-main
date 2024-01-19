using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using GD3D.Player;
using GD3D.Easing;
using GD3D.CustomInput;
using System.Linq;
using UnityEngine.Events;
using System.Security.Cryptography;
using static Cinemachine.CinemachineTriggerAction.ActionSettings;

namespace GD3D.UI
{
    /// <summary>
    /// The icon kit select screen.
    /// </summary>
    /// 
    [Serializable]
    public class CubeCost
    {
        public enum Unit
        {
            Gold,
            Diamond
        }

        public Unit unit;
        public int cost;
    }
    public class IconKit : MonoBehaviour
    {
        [SerializeField] private Image templateCategoryButton;
        [SerializeField] private IconButtonData[] iconButtonData;


        public List<CubeCost> costList;
        public Sprite GoldIcon;
        public Sprite DiamondIcon;

        private Gamemode _categoryOpen;
        private Dictionary<Gamemode, IconButtonData> _iconButtonDataDictionary = new Dictionary<Gamemode, IconButtonData>();
        private Dictionary<Gamemode, Image> _iconCategoryButtons = new Dictionary<Gamemode, Image>();
        private Dictionary<Gamemode, Transform> _iconButtonParents = new Dictionary<Gamemode, Transform>();
        public List<GameObject> CubeButtons;
        private Dictionary<Gamemode, GameObject> _iconButtonTemplates = new Dictionary<Gamemode, GameObject>();
        private Dictionary<Gamemode, List<GameObject>> _iconButtons = new Dictionary<Gamemode, List<GameObject>>();
        private Dictionary<Gamemode, List<GameObject>> _iconModels = new Dictionary<Gamemode, List<GameObject>>();
        private Dictionary<Gamemode, List<GameObject>> _iconButtonsSelected = new Dictionary<Gamemode, List<GameObject>>();

        [Header("Stats")]
        [SerializeField] private TMP_InputField nameInputField;

        [Space]
        [SerializeField] private int goldCoin;
        [SerializeField] private int diamondCoin;

        [SerializeField] private TMP_Text starsText;
        [SerializeField] private TMP_Text coinsText;
        [SerializeField] private TMP_Text goldCoinText;
        [SerializeField] private TMP_Text diamondCoinText;

        [Header("Colors")]
        [SerializeField] private PlayerColors playerColors;

        [Space]
        [SerializeField] private ColorPicker colorPicker1;
        [SerializeField] private ColorPicker colorPicker2;

        [Header("Model/Icons")]
        [SerializeField] private PlayerIcons playerIcons;

        [Header("Spinning")]
        [SerializeField] private Transform modelParent;
        [SerializeField] private float spinSpeed = -70;
        [SerializeField] private float mouseSpinSpeed = 1;

        [SerializeField] private EaseSettings stopMouseSpinEase = EaseSettings.defaultValue;
        private long? _stopMouseSpinEaseID;
        private long? _spinSpeedEaseID;

        [Space]
        [SerializeField] private LayerMask modelLayer;

        private bool _doMouseRotation;

        private float _startSpinSpeed;

        private Vector3 _targetRot;
        private Vector3 _startRot;

        private PlayerData _savefile;
        private UnityEngine.Camera _cam;

        private Key _quitKey;


        [Space]
        [SerializeField] private Button _colorChoose;
        [SerializeField] private Button _playerChoose;

        [Space]
        [SerializeField] private GameObject PlayerChoosePanel;
        [SerializeField] private GameObject ColorChoosePanel;

        [Space]
        [SerializeField] private Button getGoldByReward;

        [Space]
        [SerializeField] private Button kiti4Button;
        [SerializeField] private GameObject kiti4Buyed;


        [SerializeField] private GameObject iconKitHolder;
        [SerializeField] private GameObject worldCanvasHolder;
        [SerializeField] private GameObject modelHolder;

        private void Awake()
        {
            playerIcons.TryCreateDictionary();
        }

        private void Start()
        {
            getGoldByReward.onClick.AddListener(() =>
            {
                Geekplay.Instance.ShowRewardedAd("GetFiveGoldCoin");

            });

            GoldShop.Instance.GetGoldByReward.onClick.AddListener(() =>
            {
                Geekplay.Instance.ShowRewardedAd("GetFiveGoldCoin");

            });

            Geekplay.Instance.SubscribeOnPurchase("kiti4", () =>
            {
                kiti4Button.interactable = false;
                kiti4Button.gameObject.GetComponent<Animator>().enabled = false;
                kiti4Buyed.SetActive(true);
            });

            Geekplay.Instance.SubscribeOnReward("GetFiveGoldCoin", GetGold);
            Geekplay.Instance.SubscribeOnReward("GetFiveGoldCoin", ChangeCoin);

            GoldShop.Instance.GetPurchase.AddListener(ChangeCoin);


            GoldShop.Instance.OpenShop.AddListener(() =>
            {
                Debug.Log("Icon Menu Shop Open");

                iconKitHolder.SetActive(false);
                worldCanvasHolder.SetActive(false);
                modelHolder.SetActive(false);
            });

            GoldShop.Instance.CloseShop.AddListener(() =>
            {
                Debug.Log("Icon Menu Shop Close");

                iconKitHolder.SetActive(true);
                worldCanvasHolder.SetActive(true);
                modelHolder.SetActive(true);
            });


            // Set the last active menu scene index
            MenuData.LastActiveMenuSceneIndex = (int)Transition.SceneIndex.iconKit;


            _playerChoose.onClick.AddListener(ShowPlayerChoosePanel);
            _colorChoose.onClick.AddListener(ShowColorChoose);

            // Get the quit key
            _quitKey = PlayerInput.GetKey("Escape");

            // Get Camera
            _cam = Helpers.Camera;

            // Cache the current save file class
            _savefile = Geekplay.Instance.PlayerData;

            // Set color picker colors to the colors stored in the save file
            colorPicker1.SetColor(_savefile.PlayerColor1);
            colorPicker2.SetColor(_savefile.PlayerColor2);

            playerColors.UpdateMaterialColors(_savefile.PlayerColor1, _savefile.PlayerColor2);

            // Subscribe to color pickers to be notified of when their colors are updated
            colorPicker1.OnUpdateColor.AddListener(UpdatePlayerColor1);
            colorPicker2.OnUpdateColor.AddListener(UpdatePlayerColor2);

            // Set some stats to the data in the save file
            nameInputField.text = _savefile.PlayerName;
            nameInputField.characterLimit = SaveFile.PLAYER_NAME_MAX_LENGTH; // Also setup the name input field
            nameInputField.onEndEdit.AddListener(ChangePlayerName);

            starsText.text = _savefile.StarsCollected.ToString();
            coinsText.text = _savefile.CoinsCollected.ToString();
            diamondCoinText.text = _savefile.DiamondCoinsCollected.ToString();
            // Set start variables
            _startRot = modelParent.localRotation.eulerAngles;
            _targetRot = _startRot;

            _startSpinSpeed = spinSpeed;

            goldCoin = Geekplay.Instance.PlayerData.GoldCoinsCollected;
            goldCoinText.text = goldCoin.ToString();

            //-- Create all the icon buttons for every gamemode

            // Create dictionaries from the iconButtonData
            foreach (IconButtonData buttonData in iconButtonData)
            {
                _iconButtonDataDictionary.Add(buttonData.Gamemode, buttonData);
                _iconButtonParents.Add(buttonData.Gamemode, buttonData.Parent);
                _iconButtonTemplates.Add(buttonData.Gamemode, buttonData.TemplateButton);
                _iconButtons.Add(buttonData.Gamemode, new List<GameObject>());
                _iconModels.Add(buttonData.Gamemode, new List<GameObject>());
                _iconButtonsSelected.Add(buttonData.Gamemode, new List<GameObject>());

                // Create the category button
                GameObject newCategoryButton = Instantiate(templateCategoryButton.gameObject, templateCategoryButton.transform.parent);
                
                newCategoryButton.GetComponent<Button>().onClick.AddListener(() =>
                {
                    _categoryOpen = buttonData.Gamemode;

                    UpdateIconCategory();
                });

                _iconCategoryButtons.Add(buttonData.Gamemode, newCategoryButton.GetComponent<Image>());
            }

            UpdateIconCategory();

            //Loop through every gamemodes icon data
            foreach (PlayerIcons.GamemodeIconData iconData in playerIcons.GetGamemodeIconData)
            {
                GameObject buttonTemplate = _iconButtonTemplates[iconData.Gamemode];
                GameObject templateModel = _iconButtonDataDictionary[iconData.Gamemode].TemplateModel.gameObject;

                // Create an integer which will be used to keep track of our current index in the next foreach loop

                int costIndex = 0;
                // Loop through all mesh data in the icon data
                foreach (PlayerIcons.GamemodeIconData.MeshData meshData in iconData.Meshes)
                {
                    // Create a clone of the template button
                    //GameObject newButton = Instantiate(buttonTemplate, _iconButtonParents[Gamemode.cube]);

                    Debug.Log("SpawnCube");

                    TextMeshProUGUI costText = CubeButtons[costIndex].GetComponentInChildren<TextMeshProUGUI>();

                    Buyable buttonCost = CubeButtons[costIndex].GetComponent<Buyable>();

                    if (iconData.Gamemode == Gamemode.cube)
                    {
                        if (costList[costIndex].cost != 0 && !Geekplay.Instance.PlayerData.IsBuyedIconIndex(Gamemode.cube, costIndex))
                        {
                            if (costText == null) return;
                            
                            if (costList[costIndex].unit == CubeCost.Unit.Gold)
                            {
                                costText.gameObject.GetComponentInChildren<Image>().sprite = GoldIcon;
                            }
                            else
                            {
                                costText.gameObject.GetComponentInChildren<Image>().sprite = DiamondIcon;
                            }
                        }
                        else if(costText != null)
                        {
                            costText.gameObject.SetActive(false);
                        }

                        buttonCost.Cost = costList[costIndex].cost;

                        int thisIndex = costIndex;

                        CubeButtons[costIndex].GetComponent<Button>().onClick.AddListener(() =>
                        {
                            if (!Geekplay.Instance.PlayerData.IsBuyedIconIndex(Gamemode.cube, thisIndex))
                            {
                                if (!buttonCost.TryBuyForGold(goldCoin)) return;

                                Geekplay.Instance.PlayerData.SaveBuyedIconIndex(Gamemode.cube, thisIndex);

                                Geekplay.Instance.Save();

                                goldCoin = buttonCost.buyForFold(goldCoin);
                                goldCoinText.text = goldCoin.ToString();
                                costText.enabled = false;
                            }
                            _savefile.SetEquippedIcon(Gamemode.cube, thisIndex);

                            UpdateIconSelection(Gamemode.cube);

                        });
                    }


                    
                    // Create new model
                    GameObject iconModel = Instantiate(templateModel, templateModel.transform.parent);

                    iconModel.GetComponent<MeshFilter>().mesh = playerIcons.GetGamemodeIconData[(int)iconData.Gamemode].Meshes[costIndex].Mesh;

                    _iconModels[iconData.Gamemode].Add(iconModel);

                    // Get the "Selected" image on the button
                    GameObject selectedObj = CubeButtons[costIndex].transform.Find("Selected").gameObject;

                    // Set the mesh for the button
                    MeshFilter meshFilter = CubeButtons[costIndex].GetComponentInChildren<MeshFilter>();

                    meshFilter.mesh = meshData.Mesh;

                    // Add the newly created button and selected image to the dictionary
                    _iconButtons[iconData.Gamemode].Add(CubeButtons[costIndex]);
                    _iconButtonsSelected[iconData.Gamemode].Add(selectedObj);

                    costIndex++;
                }

                costIndex = 0;
                templateModel.SetActive(false);

                // Destroy template button
                //Destroy(_iconButtonTemplates[Gamemode.cube]);


                

            }

            UpdateIconSelection(Gamemode.cube);

            // Destroy template category button
            Destroy(templateCategoryButton.gameObject);

            // Subscribe to events
            SceneManager.sceneUnloaded += OnSceneUnloaded;
            EasingManager.Instance.OnEaseObjectRemove += OnEaseObjectRemove;

            // Loop through all icon models and add an outline to them since QuickOutline apparently only works if it's added and can't be reset manually
            /* foreach (var pair in _iconModels)
             {
                 foreach (GameObject obj in pair.Value)
                 {
                     Outline outline = obj.AddComponent<Outline>();
                     outline.OutlineWidth = 10;
                     outline.OutlineColor = Color.black;
                 }
             }*/

        }


        public void kiti(string kitiName)
        {
            Geekplay.Instance.RealBuyItem(kitiName);
        }
        private void GetGold()
        {
            Geekplay.Instance.PlayerData.GoldCoinsCollected += 5000;
        }

        public void ChangeCoin()
        {
            goldCoin = Geekplay.Instance.PlayerData.GoldCoinsCollected;
            goldCoinText.text = goldCoin.ToString();

            diamondCoin = Geekplay.Instance.PlayerData.DiamondCoinsCollected;
            diamondCoinText.text = diamondCoin.ToString();
        }
        private void ShowColorChoose()
        {
            ColorChoosePanel.SetActive(true);
            PlayerChoosePanel.SetActive(false);
        }

        private void ShowPlayerChoosePanel()
        {
            ColorChoosePanel.SetActive(false);
            PlayerChoosePanel.SetActive(true);
        }

        private void OnSceneUnloaded(Scene scene)
        {
            ChangePlayerName(nameInputField.text);
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }

        private void Update()
        {
            // Change the scene to the main menu if the quit key is pressed down
            if (!Transition.IsTransitioning && _quitKey.Pressed(PressMode.down))
            {
                GotoMenu(Transition.SceneIndex.mainMenu);
            }

            // Detect if the mouse was pressed down
            //if (Input.GetMouseButtonDown(0) && !_doMouseRotation)
            //{
            //    // Send a ray from the mouse position
            //    Ray ray = _cam.ScreenPointToRay(Input.mousePosition);

            //    // Check if the ray hit the modelLayer
            //    if (Physics.Raycast(ray, Mathf.Infinity, modelLayer))
            //    {
            //        CancelInvoke(nameof(StopMouseSpin));

            //        // If so, then we begin doing mouse rotation
            //        _doMouseRotation = true;

            //        // Stop the current stopMouseSpinEase (if it exists)
            //        if (EasingManager.TryRemoveEaseObject(_stopMouseSpinEaseID))
            //        {
            //            _stopMouseSpinEaseID = null;
            //        }

            //        // Stop the current spinSpeedEase (if it exists)
            //        if (EasingManager.TryRemoveEaseObject(_spinSpeedEaseID))
            //        {
            //            _spinSpeedEaseID = null;
            //        }
            //    }
            //}
            //// Detect if the mouse button was released
            //else if (Input.GetMouseButtonUp(0) && _doMouseRotation)
            //{
            //    _doMouseRotation = false;

            //    EaseObject ease = new EaseObject(1);

            //    ease.OnUpdate = (obj) =>
            //    {
            //        spinSpeed = obj.GetValue(0, _startSpinSpeed);
            //    };

            //    _spinSpeedEaseID = ease.ID;

            //    // If so, then stop doing mouse rotation
            //    Invoke(nameof(StopMouseSpin), 0.5f);
            //}

            //if (_doMouseRotation)
            //{
            //    _targetRot.z += -Input.GetAxis("Mouse X") * mouseSpinSpeed * Time.deltaTime;
            //    _targetRot.x += Input.GetAxis("Mouse Y") * mouseSpinSpeed * Time.deltaTime;

            //    _targetRot.z %= 360;
            //    _targetRot.x %= 360;
            //}
            //else
            //{
            _targetRot.z += spinSpeed * Time.deltaTime;
            _targetRot.z %= 360;
            //}

            modelParent.localRotation = Quaternion.Euler(_targetRot);
        }

        //private void StopMouseSpin()
        //{
        //    EaseObject ease = stopMouseSpinEase.CreateEase();

        //    float startX = _targetRot.x;
        //    float startY = _targetRot.y;

        //    // Set on update
        //    ease.OnUpdate = (obj) =>
        //    {
        //        _targetRot.x = obj.GetValue(startX, _startRot.x);
        //        _targetRot.y = obj.GetValue(startY, _startRot.y);
        //    };

        //    // Set ID
        //    _stopMouseSpinEaseID = ease.ID;
        //}

        private void OnEaseObjectRemove(long id)
        {
            // Set the mouse spin ease ID to null if that ease ID just got removed
            if (_stopMouseSpinEaseID.HasValue && id == _stopMouseSpinEaseID)
            {
                _stopMouseSpinEaseID = null;
            }
            else if (_spinSpeedEaseID.HasValue && id == _spinSpeedEaseID)
            {
                _spinSpeedEaseID = null;
            }
        }

        private void UpdateIconCategory()
        {
            // Loop through every gamemodes icon data
            foreach (PlayerIcons.GamemodeIconData iconData in playerIcons.GetGamemodeIconData)
            {
                IconButtonData buttonData = _iconButtonDataDictionary[iconData.Gamemode];

                bool isSelectedCategory = _categoryOpen == iconData.Gamemode;

                _iconCategoryButtons[iconData.Gamemode].sprite = isSelectedCategory ? buttonData.OnSprite : buttonData.OffSprite;
                _iconButtonParents[iconData.Gamemode].gameObject.SetActive(isSelectedCategory); 

                int index = 0;

                foreach (GameObject obj in _iconModels[iconData.Gamemode])
                {
                    obj.SetActive(isSelectedCategory && PlayerIcons.GetIconIndex(iconData.Gamemode) == index);

                    index++;
                }
            }
        }

        private void UpdateIconSelection()
        {
            foreach (PlayerIcons.GamemodeIconData iconData in playerIcons.GetGamemodeIconData)
            {
                int index = 0;

                foreach (PlayerIcons.GamemodeIconData.MeshData meshData in iconData.Meshes)
                {
                    bool isEquipped = _savefile.GetEquippedIconIndex(iconData.Gamemode) == index;

                    _iconButtonsSelected[iconData.Gamemode][index].SetActive(isEquipped);

                    index++;
                }
            }

            foreach (var pair in _iconModels)
            {
                foreach (GameObject obj in pair.Value)
                {
                    obj.SetActive(false);
                }
            }

            // Update mesh for the model on our currently open category
            _iconModels[_categoryOpen][PlayerIcons.GetIconIndex(_categoryOpen)].SetActive(true);
        }

        private void UpdateIconSelection(Gamemode gamemode)
        {
            int index = 0;

            foreach (PlayerIcons.GamemodeIconData.MeshData meshData in playerIcons.GetGamemodeIconData[0].Meshes)
            {
                bool isEquipped = _savefile.GetEquippedIconIndex(gamemode) == index;

                _iconButtonsSelected[gamemode][index].SetActive(isEquipped);

                index++;
            }

            foreach (var pair in _iconModels)
            {
                foreach (GameObject obj in pair.Value)
                {
                    obj.SetActive(false);
                }
            }

            // Update mesh for the model on our currently open category
            _iconModels[_categoryOpen][PlayerIcons.GetIconIndex(_categoryOpen)].SetActive(true);
        }


        private void OnApplicationQuit()
        {
            ChangePlayerName(nameInputField.text);
        }

        /// <summary>
        /// Updates the first player color in both the save file and player model.
        /// </summary>
        public void UpdatePlayerColor1(Color newColor)
        {
            _savefile.PlayerColor1 = newColor;

            playerColors.UpdateMaterialColors(newColor, _savefile.PlayerColor2);
        }

        /// <summary>
        /// Updates the second player color in both the save file and player model.
        /// </summary>
        public void UpdatePlayerColor2(Color newColor)
        {
            _savefile.PlayerColor2 = newColor;

            playerColors.UpdateMaterialColors(_savefile.PlayerColor1, newColor);
        }

        /// <summary>
        /// Changes the player name in the save file.
        /// </summary>
        private void ChangePlayerName(string name)
        {
            // Default to the default name when null or empty
            if (string.IsNullOrEmpty(name))
            {
                name = SaveFile.DEFAULT_PLAYER_NAME;
            }

            // Set the name in the save file and update the input field
            _savefile.PlayerName = name;

            nameInputField.text = name;
        }

        /// <summary>
        /// Transitions to the given menu scene <paramref name="index"/>.
        /// </summary>
        public void GotoMenu(Transition.SceneIndex index)
        {
            // Don't transition if we are already transitioning
            if (Transition.IsTransitioning)
            {
                return;
            }

            Transition.TransitionToScene((int)index);
        }

        /// <summary>
        /// Transitions to the given scene <paramref name="index"/>.
        /// </summary>
        public void GotoScene(int index)
        {
            // Don't transition if we are already transitioning
            if (Transition.IsTransitioning)
            {
                return;
            }

            Transition.TransitionToScene(index);

            if(index == 0)
            {
                Geekplay.Instance.ShowInterstitialAd();
            }
        }

        [Serializable]
        public class IconButtonData
        {
            public Gamemode Gamemode;
            public MeshFilter TemplateModel;

            [Space]
            public Sprite OnSprite;
            public Sprite OffSprite;

            [Space]
            public Transform Parent;
            public GameObject TemplateButton;
        }
    }
}
