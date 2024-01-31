using UnityEngine;
using TMPro;
using System;

namespace GD3D
{
    public class LeaderboardInGame : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI leadersText;
        [SerializeField] private TextMeshProUGUI leadersText2;

        [SerializeField] private TextMeshProUGUI leadersText1Bottom;
        [SerializeField] private TextMeshProUGUI leadersText2Bottom;


        private float timeFlag = 0;
        private void Awake()
        {
            Geekplay.Instance.leaderboardInGame = this;
        }
        void Start()
        {
            int time = Convert.ToInt32(Geekplay.Instance.remainingTimeUntilUpdateLeaderboard);

            if (Geekplay.Instance.language == "en")
            {
                leadersText1Bottom.text = $"Table will be updated in: {time}";
                leadersText2Bottom.text = $"Table will be updated in: {time}";
            }
            else if (Geekplay.Instance.language == "ru")
            {
                leadersText1Bottom.text = $"Таблица обновится через: {time}";
                leadersText2Bottom.text = $"Таблица обновится через: {time}";
            }
            else if (Geekplay.Instance.language == "tr")
            {
                leadersText1Bottom.text = $"Su yolla güncellendi: {time}";
                leadersText2Bottom.text = $"Su yolla güncellendi: {time}";
            }

            if (Geekplay.Instance.remainingTimeUntilUpdateLeaderboard <= 0)
                UpdateLeaderBoard();

            else if (Geekplay.Instance.lastLeaderText != string.Empty)
            {
                leadersText.text = Geekplay.Instance.lastLeaderText;
                leadersText2.text = Geekplay.Instance.lastLeaderText;
            }
        }


        private void Update()
        {
            if (Geekplay.Instance.remainingTimeUntilUpdateLeaderboard <= 0)
            {
                UpdateLeaderBoard();
            }
            timeFlag += Time.deltaTime;

            if (timeFlag < 1f) return;

            timeFlag = 0;   
            int time = Convert.ToInt32(Geekplay.Instance.remainingTimeUntilUpdateLeaderboard);


            if (Geekplay.Instance.language == "en")
            {
                leadersText1Bottom.text = $"Table will be updated in: {time}";
                leadersText2Bottom.text = $"Table will be updated in: {time}";
            }
            else if (Geekplay.Instance.language == "ru")
            {
                leadersText1Bottom.text = $"Таблица обновится через: {time}";
                leadersText2Bottom.text = $"Таблица обновится через: {time}";
            }
            else if (Geekplay.Instance.language == "tr")
            {
                leadersText1Bottom.text = $"Su yolla güncellendi: {time}";
                leadersText2Bottom.text = $"Su yolla güncellendi: {time}";
            }

        }
        public void SetText()
        {
            leadersText.text = "";
            Geekplay.Instance.lastLeaderText = "";
            for (int i = 0; i < Geekplay.Instance.l.Length; i++)
            {
                if (Geekplay.Instance.l[i] != null && Geekplay.Instance.lN[i] != null)
                {
                    string s = $"{i+1}. {Geekplay.Instance.lN[i]} : {Geekplay.Instance.l[i]}\n";
                    if (s == $"{i+1}.  : \n")
                    {
                        s = $"{i+1}.\n";
                    }

                    Geekplay.Instance.lastLeaderText += $"{i + 1}. {Geekplay.Instance.lN[i]} : {Geekplay.Instance.l[i]}\n";
                    leadersText.text = Geekplay.Instance.lastLeaderText;
                    leadersText2.text = Geekplay.Instance.lastLeaderText;
                    //$"{i + 1}. {Geekplay.Instance.lN[i]} : {Geekplay.Instance.l[i]}\n"
                }
            }
        }

        public void UpdateLeaderBoard()
        {
            Geekplay.Instance.remainingTimeUntilUpdateLeaderboard = Geekplay.Instance.timeToUpdateLeaderboard;

            Geekplay.Instance.leaderNumber = 0;
            Geekplay.Instance.leaderNumberN = 0;
            Utils.GetLeaderboard("score", 0);
            Utils.GetLeaderboard("name", 0);

        }
    }
}
