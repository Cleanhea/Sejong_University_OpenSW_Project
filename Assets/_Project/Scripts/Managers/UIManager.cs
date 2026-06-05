using UnityEngine;
using TMPro;

// 씬별 UI 갱신을 담당하는 매니저
// 하나의 씬에 하나만 배치하며, 해당 씬에 존재하는 UI 텍스트만 인스펙터에서 연결한다
// (연결되지 않은 필드는 null 가드로 무시되므로 GameScene / EndingScene에 공용으로 사용 가능)
public class UIManager : MonoBehaviour
{
    // 점수 표시 포맷 상수 (매직 스트링 방지)
    private const string SCORE_FORMAT = "Score : {0}";
    private const string HIGH_SCORE_FORMAT = "Best : {0}";
    private const string FINAL_SCORE_FORMAT = "Final Score : {0}";

    // 타이머 분/초 변환 상수
    private const int SECONDS_PER_MINUTE = 60;
    private const string TIMER_FORMAT = "{0:00}:{1:00}";

    [Header("게임 씬 UI")]
    [SerializeField] private TMP_Text scoreText;       // 현재 점수
    [SerializeField] private TMP_Text timerText;       // 남은 시간

    [Header("공용 UI")]
    [SerializeField] private TMP_Text highScoreText;   // 최고 기록

    [Header("엔딩 씬 UI")]
    [SerializeField] private TMP_Text finalScoreText;  // 최종 점수

    // 현재 점수 갱신
    public void UpdateScore(int score)
    {
        if (scoreText == null) return;
        scoreText.text = string.Format(SCORE_FORMAT, score);
    }

    // 최고 기록 갱신
    public void UpdateHighScore(int highScore)
    {
        if (highScoreText == null) return;
        highScoreText.text = string.Format(HIGH_SCORE_FORMAT, highScore);
    }

    // 최종 점수 갱신 (엔딩 씬)
    public void UpdateFinalScore(int finalScore)
    {
        if (finalScoreText == null) return;
        finalScoreText.text = string.Format(FINAL_SCORE_FORMAT, finalScore);
    }

    // 남은 시간 갱신 (초 단위를 분:초 형태로 변환)
    public void UpdateTimer(float remainingSeconds)
    {
        if (timerText == null) return;

        // 음수 방지
        if (remainingSeconds < 0f) remainingSeconds = 0f;

        int totalSeconds = Mathf.CeilToInt(remainingSeconds);
        int minutes = totalSeconds / SECONDS_PER_MINUTE;
        int seconds = totalSeconds % SECONDS_PER_MINUTE;

        timerText.text = string.Format(TIMER_FORMAT, minutes, seconds);
    }
}
