using UnityEngine;

// 점수 계산 및 최고 기록(PlayerPrefs) 저장을 담당하는 싱글톤 매니저
// 현재 점수를 씬 전환(GameScene -> EndingScene) 후에도 유지하기 위해 DontDestroyOnLoad 사용
public class ScoreManager : MonoBehaviour
{
    // PlayerPrefs 저장 키 (매직 스트링 방지)
    private const string HIGH_SCORE_KEY = "HighScore";

    // 점수 초기값
    private const int INITIAL_SCORE = 0;

    // 싱글톤 인스턴스
    public static ScoreManager Instance { get; private set; }

    // 씬에 존재할 경우 점수 변동 시 갱신할 UI (없으면 무시)
    [SerializeField] private UIManager uiManager;

    // 현재 점수
    public int CurrentScore { get; private set; }

    // 최고 기록
    public int HighScore { get; private set; }

    private void Awake()
    {
        // 중복 인스턴스 제거
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // 저장된 최고 기록 불러오기
        HighScore = PlayerPrefs.GetInt(HIGH_SCORE_KEY, INITIAL_SCORE);
    }

    // 점수 추가 (적 처치, 생존 보너스 등에서 호출)
    public void AddScore(int amount)
    {
        CurrentScore += amount;

        // 진행 중에도 최고 기록을 실시간으로 갱신
        if (CurrentScore > HighScore)
            HighScore = CurrentScore;

        RefreshUI();
    }

    // 새 게임 시작 시 현재 점수 초기화 (최고 기록은 유지)
    public void ResetScore()
    {
        CurrentScore = INITIAL_SCORE;
        RefreshUI();
    }

    // 최고 기록을 PlayerPrefs에 영구 저장 (게임 종료 시 호출)
    public void SaveHighScore()
    {
        PlayerPrefs.SetInt(HIGH_SCORE_KEY, HighScore);
        PlayerPrefs.Save();
    }

    // 연결된 UI가 있으면 점수/최고 기록 텍스트 갱신
    private void RefreshUI()
    {
        if (uiManager == null) return;
        uiManager.UpdateScore(CurrentScore);
        uiManager.UpdateHighScore(HighScore);
    }

    // 씬 전환 후 새 UIManager를 연결할 때 사용 (예: EndingScene 진입 시)
    public void BindUI(UIManager manager)
    {
        uiManager = manager;
        RefreshUI();
    }
}
