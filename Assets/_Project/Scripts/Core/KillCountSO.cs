using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "KillCountSO", menuName = "ScriptableObjects/KillCountSO", order = 1)]
public class KillCountSO : ScriptableObject
{
    public Action<int> OnKillCountChanged;
    public int KillCount { get; private set; }
    public void UpdateKillCount(int amount)
    {
        KillCount += amount;
        OnKillCountChanged?.Invoke(KillCount);
    }
    public void ResetKillCount()
    {
        KillCount = 0;
        OnKillCountChanged?.Invoke(KillCount);
    }
}
