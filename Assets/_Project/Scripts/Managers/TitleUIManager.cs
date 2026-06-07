using UnityEngine;
using UnityEngine.UI;

// TitleScene 전용 UI 매니저
// New Game / Load Game / Settings / Exit 버튼과 설정 패널을 제어한다
public class TitleUIManager : MonoBehaviour
{
    // 저장 데이터 존재 여부를 확인할 PlayerPrefs 키 (저장 시스템 구현 시 사용)
    private const string SAVE_DATA_KEY = "SaveData";

    [Header("버튼")]
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button loadGameButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button exitButton;

    [Header("패널")]
    [SerializeField] private GameObject settingsPanel;   // 설정 창 (기본 비활성)

    private void Awake()
    {
        // 버튼 클릭 이벤트 등록 (인스펙터 연결 누락 대비 null 가드)
        if (newGameButton != null) newGameButton.onClick.AddListener(OnNewGame);
        if (loadGameButton != null) loadGameButton.onClick.AddListener(OnLoadGame);
        if (settingsButton != null) settingsButton.onClick.AddListener(OnSettings);
        if (exitButton != null) exitButton.onClick.AddListener(OnExit);
    }

    private void Start()
    {
        // 설정 패널은 시작 시 닫아둔다
        if (settingsPanel != null) settingsPanel.SetActive(false);

        // 저장 데이터가 없으면 Load Game 버튼 비활성화
        if (loadGameButton != null) loadGameButton.interactable = HasSaveData();
    }

    private void OnDestroy()
    {
        // 등록한 리스너 해제 (메모리 누수 방지)
        if (newGameButton != null) newGameButton.onClick.RemoveListener(OnNewGame);
        if (loadGameButton != null) loadGameButton.onClick.RemoveListener(OnLoadGame);
        if (settingsButton != null) settingsButton.onClick.RemoveListener(OnSettings);
        if (exitButton != null) exitButton.onClick.RemoveListener(OnExit);
    }

    // 새 게임 시작: 점수 초기화 후 게임 씬으로 이동
    public void OnNewGame()
    {
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.ResetScore();

        if (GameManager.Instance != null)
            GameManager.Instance.LoadGame();
    }

    // 이어하기: 저장 데이터 불러오기
    private void OnLoadGame()
    {
        // TODO: 저장 시스템 구현 후 저장된 진행 상황을 불러와 게임 씬으로 이동
        if (!HasSaveData()) return;

        if (GameManager.Instance != null)
            GameManager.Instance.LoadGame();
    }

    // 설정 창 토글
    public void OnSettings()
    {
        if (settingsPanel == null) return;
        settingsPanel.SetActive(!settingsPanel.activeSelf);
    }

    // 게임 종료
    public void OnExit()
    {
#if UNITY_EDITOR
        // 에디터에서는 플레이 모드 종료
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // 저장 데이터 존재 여부 확인
    private bool HasSaveData()
    {
        // TODO: 실제 저장 시스템에 맞게 판정 로직 수정
        return PlayerPrefs.HasKey(SAVE_DATA_KEY);
    }
}
