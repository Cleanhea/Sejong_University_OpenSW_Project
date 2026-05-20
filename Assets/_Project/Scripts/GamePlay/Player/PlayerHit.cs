using UnityEngine;
using System.Collections;

public class PlayerHit : MonoBehaviour
{
    [SerializeField] private PlayerStat playerStat;
    private PlayerHealth playerHealth;
    [SerializeField] private EnemyStat enemyStat;
    private Rigidbody2D rb;

    void Start()
    {
        playerHealth = GetComponent<PlayerHealth>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    public void Hit(float damage, Vector2 enemyPosition)
    {
        if(playerStat.isInvincible) return; //무적상태면 취소
        if(playerStat.dashInvincible) return; //대시 무적 상태면 취소
        playerHealth.ApplyDamage(damage);
        StartCoroutine(KnockBack(enemyPosition));
        StartCoroutine(HitInvincible());
    }

    // 지수 감소 계수: 높을수록 초반에 더 빠르게 감속
    private const float KnockbackDecay = 5f;

    IEnumerator KnockBack(Vector2 enemyPosition)
    {
        playerStat.isKnockedBack = true;

        if (rb != null)
        {
            Vector2 knockbackDir = ((Vector2)transform.position - enemyPosition).normalized;
            float elapsed = 0f;

            // v(t) = F * e^(-k * t/T) : 빠른 초반 → 서서히 감속 (변위 곡선이 로그 형태)
            while (elapsed < playerStat.knockbackDuration)
            {
                float t = elapsed / playerStat.knockbackDuration;
                rb.linearVelocity = playerStat.knockbackForce * Mathf.Exp(-KnockbackDecay * t) * knockbackDir;
                elapsed += Time.deltaTime;
                yield return null;
            }

            rb.linearVelocity = Vector2.zero;
        }

        playerStat.isKnockedBack = false;
    }

    IEnumerator HitInvincible()
    {
        // 무적 시작
        playerStat.isInvincible = true;
        playerStat.invincibilityTimer = playerStat.invincibilityDuration;
                // 무적 시간만큼 대기
        while (playerStat.invincibilityTimer > 0f)
        {
            playerStat.invincibilityTimer -= Time.deltaTime;
            yield return null;
        }

        playerStat.invincibilityTimer = 0f;
        playerStat.isInvincible = false;
    }
}
