using UnityEngine;

// GameScene 전용 5분 카운트다운 타이머
// 시간이 0이 되면 최고 기록을 저장하고 엔딩 씬으로 전환한다
public class GameTimer : MonoBehaviour
{
    // 제한 시간 (5분 = 300초)
    private const float GAME_DURATION = 300f;

    // 표시 갱신 비교용 초기값
    private const int INVALID_SECOND = -1;

    // 점수 변동 등으로 UI를 연결할 매니저
    [SerializeField] private UIManager uiManager;

    // 남은 시간(초)
    private float remainingTime;

    // 타이머 동작 여부
    private bool isRunning;

    // 마지막으로 UI에 표시한 초 (값이 바뀔 때만 갱신해 GC 할당 최소화)
    private int lastDisplayedSecond = INVALID_SECOND;

    private void Start()
    {
        StartTimer();
    }

    // 타이머 시작 (시간 초기화)
    public void StartTimer()
    {
        remainingTime = GAME_DURATION;
        lastDisplayedSecond = INVALID_SECOND;
        isRunning = true;

        UpdateTimerUI();
    }

    private void Update()
    {
        if (!isRunning) return;

        remainingTime -= Time.deltaTime;

        if (remainingTime <= 0f)
        {
            remainingTime = 0f;
            UpdateTimerUI();
            TimeOver();
            return;
        }

        UpdateTimerUI();
    }

    // 표시할 초가 바뀐 경우에만 UI 갱신
    private void UpdateTimerUI()
    {
        if (uiManager == null) return;

        int currentSecond = Mathf.CeilToInt(remainingTime);
        if (currentSecond == lastDisplayedSecond) return;

        lastDisplayedSecond = currentSecond;
        uiManager.UpdateTimer(remainingTime);
    }

    // 시간 종료 처리
    private void TimeOver()
    {
        isRunning = false;

        // 최고 기록 영구 저장 후 엔딩 씬으로 전환
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.SaveHighScore();

        if (GameManager.Instance != null)
            GameManager.Instance.LoadEnding();
    }
}
