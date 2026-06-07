using UnityEngine;
using System;

public class GameTimeManager : MonoBehaviour
{
    public static string GetPlayTime()
    {
        float totalSeconds = Time.timeSinceLevelLoad;
        TimeSpan timeSpan = TimeSpan.FromSeconds(totalSeconds);
        return string.Format("{0:D2}:{1:D2}:{2:D2}", 
                timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
    }
}
