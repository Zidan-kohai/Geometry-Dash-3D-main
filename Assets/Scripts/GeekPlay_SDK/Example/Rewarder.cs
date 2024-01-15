using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rewarder : MonoBehaviour
{
	void Start()
	{
		Geekplay.Instance.GameReady();
	}

    public void PlusCoinsReward()
    {
    	Geekplay.Instance.PlayerData.GoldCoinsCollected += 100;
    }

    public void BuyCoins(int coinsCount)
    {
    	Geekplay.Instance.PlayerData.GoldCoinsCollected += coinsCount;
    }
}
