using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class GameResultPopup : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private GameObject GameResultPanel;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI totalScoreText;

    [Header("Button")]
    [SerializeField] private Button restartButton;
    [SerializeField] private Button menuButton;

    [Header("Connected Data")]
    [SerializeField] private PlayerStat playerStat;

    [Header("Scene Names")]
    [SerializeField] private string inGameSceneName = "MainScene";
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    
    private int currentScore = 0; // Score
    private bool isGameOver = false; // Game over flag
    private const float SUCCESS_TIME = 300f; // 5 minutes

    void Start()
    {
        if (GameResultPanel != null)
            GameResultPanel.SetActive(false);
        
        if(restartButton != null)
            restartButton.onClick.AddListener(OnRestartButtonClicked);

        if(menuButton != null)
            menuButton.onClick.AddListener(OnMenuButtonClicked);

        currentScore = 0;
        UpdateScoreUI();
    }

    void Update()
    {
        if (isGameOver) return;

        float totalSeconds = Time.timeSinceLevelLoad;

        if(totalSeconds >= SUCCESS_TIME)
        {
            ShowGameResult(true, totalSeconds);
        }
        else if(playerStat != null && playerStat.playerdead)
        {
            ShowGameResult(false, totalSeconds);
        }
    }

    private void ShowGameResult(bool isSuccess, float totalSeconds)
    {
        isGameOver = true;

        Time.timeScale = 0f; // Pause the game

        if(GameResultPanel != null)
            GameResultPanel.SetActive(true);

        if (isSuccess)
        {
            titleText.text = "CLEAR";
        }
        else
        {
            titleText.text = "GAME OVER";
        }

        TimeSpan timeSpan = TimeSpan.FromSeconds(totalSeconds);
        timeText.text = string.Format("{0:D2}:{1:D2}:{2:D2}",
                playerStat.playTime.Hours, playerStat.playTime.Minutes, playerStat.playTime.Seconds);

        if (totalScoreText != null)
            totalScoreText.text = playerStat.killCountSO.KillCount.ToString("D3");
    }

    // Method to add score
    public void AddScore(int amount)
    {
        if (isGameOver) return;

        currentScore += amount;
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if(scoreText != null)
            scoreText.text = "Score: ";
        if (totalScoreText != null)
            totalScoreText.text = string.Format("{0:D2}:{1:D2}:{2:D2}",
                playerStat.playTime.Hours, playerStat.playTime.Minutes, playerStat.playTime.Seconds);
    }

    private void OnRestartButtonClicked()
    {
        Time.timeScale = 1f; // Resume the game
        SceneManager.LoadScene(inGameSceneName);
    }

    private void OnMenuButtonClicked()
    {
        Time.timeScale = 1f; // Resume the game
        SceneManager.LoadScene(mainMenuSceneName);
    }

}
