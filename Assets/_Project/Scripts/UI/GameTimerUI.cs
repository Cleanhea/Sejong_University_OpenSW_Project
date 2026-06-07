using UnityEngine;
using TMPro;
using System;

public class GameTimerUI : MonoBehaviour
{
    [Header("Timer Text UI")]
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private PlayerStat playerStat;
    void Update()
    {
        if(timerText != null)
        {
            float totalSeconds = Time.timeSinceLevelLoad;
            TimeSpan timeSpan = TimeSpan.FromSeconds(totalSeconds);
            playerStat.playTime = timeSpan;
            timerText.text = string.Format("{0:D2}:{1:D2}:{2:D2}", 
                timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
        }
    }
}
