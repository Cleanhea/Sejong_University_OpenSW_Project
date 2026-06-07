using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class MonsterHPController : MonoBehaviour
{
    [Header("HP Settings")]
    [SerializeField] private float maxHP = 100f;
    [SerializeField] private DefaultMonster monster; // 몬스터 스크립트 참조
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
        // 몬스터의 최대 체력(MonsterDataSO.maxHealth)에 맞춰 maxHP 설정
        // monster 참조가 연결돼 있으면 그 값을 사용(아니면 인스펙터 maxHP 유지)
        if(monster != null)
        {
            maxHP = monster._maxHealth;
        }

        currentHP = maxHP;

        if(fillImage != null)
        {
            fillImage.color = originalColor;
        }

        UpdateSlider();
    }

    // DefaultMonster가 자신의 최대 체력을 HP바로 직접 전달(push)할 때 사용.
    // monster 참조가 비어 있어도, 실행 순서와 무관하게 maxHP를 올바르게 세팅한다.
    public void SetMaxHP(float value)
    {
        maxHP = value;
        currentHP = maxHP;
        UpdateSlider();
    }

    private void UpdateSlider()
    {
        if(hpSlider != null)
        {
            hpSlider.maxValue = maxHP;
            hpSlider.value = currentHP;
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
