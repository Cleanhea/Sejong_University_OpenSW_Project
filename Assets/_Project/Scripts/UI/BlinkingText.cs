using UnityEngine;
using TMPro;
using System.Collections;

[DefaultExecutionOrder(101)]
public class BlinkingText : MonoBehaviour
{
    [Header("UI Settings")]
    [SerializeField] private TextMeshProUGUI targetText;
    [SerializeField] private GameObject controlsPanel;

    [Header("Blinking Settings")]
    [SerializeField] private float blinkInterval = 0.5f;

    private bool isStarted = false;

    void Start()
    {
        if (targetText == null)
        {
            Debug.LogError("Target Text is not assigned.");
            return;
        }
        if (targetText != null)
        {
            Time.timeScale = 0f;
            StartCoroutine(BlinkText());
        }
    }

    void Update()
    {
        if (isStarted) return;
        if (Input.GetKeyDown(KeyCode.E))
        {
            StartGame();
        }
    }

    private void StartGame()
    {
        Time.timeScale = 1f;
        isStarted = true;
        StopAllCoroutines();
        targetText.alpha = 1f;

        if (controlsPanel != null)
        {
            Debug.Log("Hiding controls panel.");
            controlsPanel.SetActive(false);
        }
        
    }  

    private IEnumerator BlinkText()
    {
        while (true)
        {
            // Fade Out
            float time = 0f;
            while (time < blinkInterval)
            {
                time += Time.unscaledDeltaTime;
                float percentage = time / blinkInterval;
                targetText.alpha = Mathf.Lerp(1f, 0f, percentage);
                yield return null;
            }
            targetText.alpha = 0f;

            // Fade In
            time = 0f;
            while (time < blinkInterval)
            {
                time += Time.unscaledDeltaTime;
                float percentage = time / blinkInterval;
                targetText.alpha = Mathf.Lerp(0f, 1f, percentage);
                yield return null;
            }
            targetText.alpha = 1f;
        }
    }
}
