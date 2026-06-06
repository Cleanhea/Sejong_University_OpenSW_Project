using UnityEngine;
using UnityEngine.UI;

public class MonsterKillCountManager : MonoBehaviour
{
    [SerializeField] private KillCountSO killCountSO;
    [SerializeField] private Text killCountText;

    private void Start()
    {
        killCountSO.ResetKillCount();
        UpdateKillCountText(killCountSO.KillCount);
    }
    private void OnEnable()
    {
        killCountSO.OnKillCountChanged+=  UpdateKillCountText;
    }
    private void OnDisable()
    {
        killCountSO.OnKillCountChanged-= UpdateKillCountText;
    }

    private void UpdateKillCountText(int newCount)
    {
        killCountText.text = $"Kill Score: {newCount}";
    }
}
