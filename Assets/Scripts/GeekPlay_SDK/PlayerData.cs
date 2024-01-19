using System;
using System.Collections.Generic;
using UnityEngine;
using GD3D.Player;

[Serializable]
public class PlayerData
{
    /////InApps//////
    public string lastBuy;

    public const string DEFAULT_PLAYER_NAME = "Name";
    public const int PLAYER_NAME_MAX_LENGTH = 30;

    public string PlayerName = DEFAULT_PLAYER_NAME;
    public int StarsCollected = 0;
    public int CoinsCollected = 0;
    public int GoldCoinsCollected = 0;
    public int DiamondCoinsCollected = 0;
    public int LeaderboardPointds = 0;
    public bool IsbyedKiti1 = false;
    public bool IsbyedKiti2 = false;
    public bool IsbyedKiti3 = false;
    public bool IsbyedKiti4 = false;

    public float MusicVolume = 1;
    public float SFXVolume = 1;

    public bool AutoRetryEnabled = true;
    public bool AutoCheckpointsEnabled = true;
    public bool ProgressBarEnabled = true;
    public bool ShowPercentEnabled = true;

    public ColorNoAlpha PlayerColor1 = PlayerColors.DefaultColor1;
    public ColorNoAlpha PlayerColor2 = PlayerColors.DefaultColor2;

    #region Saving Icons
    private Dictionary<Gamemode, int> _equippedIconIndex = null;

    public MyDictionary myDictionary = new MyDictionary()
    {
        gamemode = new List<Gamemode>() { Gamemode.cube, Gamemode.ship },
        cubeIndex = new List<int>(),
        shipIndex = new List<int>()
    };

    [Serializable]
    public class MyDictionary
    {
        public List<Gamemode> gamemode;
        public List<int> cubeIndex;
        public List<int> shipIndex;
    }

    public List<ColorNoAlpha> BuyedColor1 = new List<ColorNoAlpha>();
    public List<ColorNoAlpha> BuyedColor2 = new List<ColorNoAlpha>();
    /// <summary>
    /// Will return the index of the currently equipped icon of the given <paramref name="gamemode"/>.
    /// </summary>
    public int GetEquippedIconIndex(Gamemode gamemode)
    {
        TryCreateDictionary(gamemode);

        // Return the index in the dictionary
        return _equippedIconIndex[gamemode];
    }

    public void SetEquippedIcon(Gamemode gamemode, int index)
    {
        IconData[(int)gamemode].Index = index;
        _equippedIconIndex[gamemode] = index;
    }
    public bool IsBuyedIconIndex(Gamemode gamemode, int index)
    {
        if (gamemode == Gamemode.cube)
        {
            return myDictionary.cubeIndex.Contains(index);
        }
        else
        {
            return myDictionary.shipIndex.Contains(index);
        }
    }

    public void SaveBuyedIconIndex(Gamemode gamemode, int index)
    {
        if (gamemode == Gamemode.cube)
        {
            myDictionary.cubeIndex.Add(index);
        }
        else
        {
            myDictionary.shipIndex.Add(index);
        }

    }

    private void TryCreateDictionary(Gamemode gamemode)
    {
        // Check if our dictionary is null
        if (_equippedIconIndex == null)
        {
            // If so, then we will create a new dictionary for getting the equipped icon
            _equippedIconIndex = new Dictionary<Gamemode, int>();

            // We will now populate the dictionary by looping through our icon data list
            foreach (IconSaveData icon in IconData)
            {
                _equippedIconIndex.Add(icon.Gamemode, icon.Index);
            }
        }

        // Check if the gamemode doesn't exist in the dictionary
        if (!_equippedIconIndex.ContainsKey(gamemode))
        {
            // If it doesn't exist, then we will create a new entry in the dictionary with the default value
            _equippedIconIndex.Add(gamemode, 0);
            IconData.Add(new IconSaveData(gamemode, 0));
        }
    }

    public List<IconSaveData> IconData = new List<IconSaveData>();

    /// <summary>
    /// Class that contains data for a single icon.
    /// </summary>
    [Serializable]
    public class IconSaveData
    {
        public Gamemode Gamemode = Gamemode.cube;
        public int Index = 0;

        public IconSaveData(Gamemode gamemode, int index)
        {
            Gamemode = gamemode;
            Index = index;
        }
    }
    #endregion

    #region Saving Level Progress
    private Dictionary<string, LevelSaveData> _levelDataDictionary = null;

    /// <summary>
    /// Will return the LevelData that matches with the given <paramref name="name"/>. If none is found, then a new empty LevelData is created and returned.
    /// </summary>
    public LevelSaveData GetLevelData(string name)
    {
        // Check if our dictionary is null
        if (_levelDataDictionary == null)
        {
            // If so, then we will create a new dictionary for getting the level data
            _levelDataDictionary = new Dictionary<string, LevelSaveData>();

            // We will now populate the dictionary by looping through our icon data list
            foreach (LevelSaveData levelData in LevelData)
            {
                _levelDataDictionary.Add(levelData.Name, levelData);
            }
        }

        // Check if the level doesn't exist in the dictionary
        if (!_levelDataDictionary.ContainsKey(name))
        {
            // If it doesn't exist, then we will create a new entry in the dictionary with the given name
            LevelSaveData newData = new LevelSaveData(name);

            _levelDataDictionary.Add(name, newData);
            LevelData.Add(newData);
        }

        // Return the LevelData in the dictionary
        return _levelDataDictionary[name];
    }

    public List<LevelSaveData> LevelData = new List<LevelSaveData>();

    /// <summary>
    /// Class that contains data for a single level.
    /// </summary>
    [Serializable]
    public class LevelSaveData
    {
        public string Name = "null";

        public bool isOpen;
        public bool CompletedLevel => NormalPercent >= 1;
        public bool CompletedLevelPractice => PracticePercent >= 1;

        public float NormalPercent = 0;
        public float PracticePercent = 0;

        public bool[] GottenCoins = new bool[] { false, false, false };

        public int TotalAttempts = 0;
        public int TotalJumps = 0;

        public LevelSaveData(string name)
        {
            Name = name;
        }
    }
    #endregion

    /// <summary>
    /// Simply a color struct without any alpha
    /// </summary>
    [Serializable]
    public struct ColorNoAlpha
    {
        public float r, g, b;

        public static implicit operator Color(ColorNoAlpha colorNoAlpha)
        {
            return new Color(colorNoAlpha.r, colorNoAlpha.g, colorNoAlpha.b, 1);
        }

        public static implicit operator ColorNoAlpha(Color col)
        {
            return new ColorNoAlpha(col.r, col.g, col.b);
        }

        public ColorNoAlpha(float r, float g, float b)
        {
            this.r = r;
            this.g = g;
            this.b = b;
        }
    }
}