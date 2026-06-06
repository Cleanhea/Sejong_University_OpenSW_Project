using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class MonsterHPController : MonoBehaviour
{
    [Header("HP Settings")]
    [SerializeField] private float maxHP = 100f;
    private float currentHP;

    [Header("UI Components")]
    [SerializeField] private Slider hpSlider;
    [SerializeField] private Image fillImage;

    [Header("Flash Settings")]
    [SerializeField] private Color originalColor = Color.blue;
    [SerializeField] private Color flashColor = Color.white;
    [SerializeField] private float flashDuration = 0.2f;

    private Coroutine flashCoroutine;

    void Start()
    {
        currentHP = maxHP;
        
        if(hpSlider != null)
        {
            hpSlider.maxValue = maxHP;
            hpSlider.value = currentHP;
        }

        if(fillImage != null)
        {
            fillImage.color = originalColor;
        }
    }

    public void TakeDamage(float damage)
    {
        currentHP -= damage;
        currentHP = Mathf.Clamp(currentHP, 0f, maxHP);

        if(hpSlider != null)
        {
            hpSlider.value = currentHP;
        }

        if(fillImage != null && gameObject.activeInHierarchy)
        {
            if(flashCoroutine != null)
            {
                StopCoroutine(flashCoroutine);
            }
            flashCoroutine = StartCoroutine(FlashHpBar());
        }

        if(currentHP <= 0f)
        {
            Die();
        }
    }

    private IEnumerator FlashHpBar()
    {
        fillImage.color = flashColor;
        yield return new WaitForSeconds(flashDuration);
        fillImage.color = originalColor;
        flashCoroutine = null;
    }

    private void Die()
    {
        Debug.Log("Monster has died.");

        if(flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
            flashCoroutine = null;
        }

        gameObject.SetActive(false);
    }

}
