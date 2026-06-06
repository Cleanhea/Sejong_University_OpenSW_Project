using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PlayerMove))]
[RequireComponent(typeof(PlayerDash))]
[RequireComponent(typeof(PlayerHit))]
public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private PlayerStat playerStat;
    private string EndingSceneName = "";
    void Start()
    {
        playerStat.currentHp = playerStat.maxHp;
        playerStat.playerdead = false;
        playerStat.isInvincible = false;
        playerStat.invincibilityTimer = 0f;
    }

    // PlayerHit에서 호출 - 방어력을 % 감소율로 적용 (곱연산)
    // 예) defensePower=20 → rawDamage * 0.8f (20% 피해 감소)
    public void ApplyDamage(float rawDamage)
    {
        if (playerStat.playerdead) return;

        float damageMultiplier = Mathf.Max(0.1f, 1f - (playerStat.defensePower / 100f));
        float actualDamage = rawDamage * damageMultiplier;
        playerStat.currentHp -= actualDamage;
        playerStat.currentHp = Mathf.Max(0f, playerStat.currentHp);

        if (playerStat.currentHp <= 0f)
            Die();
    }

    public void Heal(float amount)
    {
        if (playerStat.playerdead) return;
        playerStat.currentHp = Mathf.Min(playerStat.maxHp, playerStat.currentHp + amount);
    }

    public bool IsDead() => playerStat.playerdead;

    private void Die()
    {
        playerStat.playerdead = true;
        GetComponent<PlayerMove>().enabled = false;
        GetComponent<PlayerDash>().enabled = false;
        // TODO: 사망 애니메이션, 게임오버 UI
        Debug.Log("Player Dead");
        playerStat.isKnockedBack = false;
        SceneManager.LoadScene("EndingScene");
    }
}
