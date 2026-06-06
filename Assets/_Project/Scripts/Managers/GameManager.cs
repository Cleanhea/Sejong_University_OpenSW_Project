using UnityEngine;
using UnityEngine.SceneManagement;

// 게임 전체 흐름을 관리하는 싱글톤 매니저
// 씬 전환을 담당하며 DontDestroyOnLoad로 씬이 바뀌어도 유지된다
public class GameManager : MonoBehaviour
{
    // 씬 이름 상수 (매직 스트링 방지)
    private const string TITLE_SCENE = "TitleScene";
    private const string GAME_SCENE = "GameScene";
    private const string ENDING_SCENE = "EndingScene";

    // 싱글톤 인스턴스
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        // 이미 인스턴스가 존재하면 중복 생성된 자신을 제거
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // 타이틀 씬으로 이동
    public void LoadTitle()
    {
        SceneManager.LoadScene(TITLE_SCENE);
    }

    // 게임 씬으로 이동 (게임 시작)
    public void LoadGame()
    {
        SceneManager.LoadScene(GAME_SCENE);
    }

    // 엔딩 씬으로 이동 (게임 종료)
    public void LoadEnding()
    {
        SceneManager.LoadScene(ENDING_SCENE);
    }
}
