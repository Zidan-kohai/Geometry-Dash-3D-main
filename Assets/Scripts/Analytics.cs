using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameAnalyticsSDK;

public class Analytics : MonoBehaviour
{
    public static Analytics Instance;

    private void Awake()
    {
        Instance = this;

        DontDestroyOnLoad(this);
    }

    void Start()
    {
        GameAnalytics.Initialize();  
    }

    public void SendEvent(string eventStr)
    {
        try
        {
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, eventStr);  
            //AppMetrica.Instance.ReportEvent(eventStr);
        }
        catch (Exception e)
        {

        }
    }
}

