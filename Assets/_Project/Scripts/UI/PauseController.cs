using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PauseController : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private GameObject optionsPanel;

    [Header("Blinking Settings")]
    [SerializeField] private float blinkInterval = 1f;

    [Header("Scene Settings")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private bool isPaused = false;
    
    void Start()
    {
        if(pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
        if(optionsPanel != null)
        {
            optionsPanel.SetActive(false);
        }

        Time.timeScale = 1f;
    }

    
    void Update()
    {
        if (UnityEngine.InputSystem.Keyboard.current != null && 
            UnityEngine.InputSystem.Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if(optionsPanel != null && optionsPanel.activeSelf)
            {
                CloseOptions();
            }
            else
            {
                if(isPaused) ResumeGame();
                else PauseGame();
            }
        }

        if(isPaused && timeText != null)
        {
            timeText.text = GameTimeManager.GetPlayTime();

            float alpha = Mathf.PingPong(Time.unscaledTime * blinkInterval, 1f);
            timeText.color = new Color(timeText.color.r, timeText.color.g, timeText.color.b, alpha);
        }
    }

    public void PauseGame()
    {
        isPaused = true;

        if(pausePanel != null)
        {
            pausePanel.SetActive(true);
        }

        Time.timeScale = 0f; // Stop the game time
    }

    // Reume Button
    public void ResumeGame()
    {
        isPaused = false;

        if(pausePanel != null) pausePanel.SetActive(false);
        if(optionsPanel != null) optionsPanel.SetActive(false);

        if(timeText != null)
        {
            timeText.color = new Color(timeText.color.r, timeText.color.g, timeText.color.b, 1f);
        }

        Time.timeScale = 1f; // Resume the game time
    }

    // Options Button
    public void OpenOptions()
    {
        if(optionsPanel != null)
        {
            optionsPanel.SetActive(true);
        }
    }


    public void CloseOptions()
    {
        if(optionsPanel != null)
        {
            optionsPanel.SetActive(false);
        }
    }

    // Main Menu Button
    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;

        Debug.Log("Returning to Main Menu...");
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
