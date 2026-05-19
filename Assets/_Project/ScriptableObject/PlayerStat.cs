using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStat", menuName = "Stats/PlayerStat")]
public class PlayerStat : ScriptableObject
{
    public float maxHp = 100f;// 최대 체력
    public float currentHp = 100f;// 체력
    public float attackPower = 10f;// 공격력
    public float defensePower = 5f;// 방어력
    public float speed = 5f;// 이동 속도
    public int maxDashCount = 3; // 최대 대시 횟수
    public int currentDashCount = 3;// 현재 대시 횟수
    public float dashCooldown = 2f;// 대시 쿨타임
    public float dashCooldownTimer = 0f; // 대시 쿨타임 타이머
    public float dashSpeed = 50f;   // 대시 속도
    public float maxDashTime = 0.1f; // 대시 지속 시간
    public float dashTimer = 0.5f;   // 현재 대시 시간
    public bool isInvincible = false; //무적 상태 여부
    public float invincibilityDuration = 2f; // 무적 지속 시간
    public float invincibilityTimer = 0f; // 무적 타이머
    public float knockbackForce = 20f; // 넉백 힘
    public float knockbackDuration = 0.3f; // 넉백 지속 시간 (초)
    public bool isKnockedBack = false; // 넉백 중 여부
    public bool playerdead = false; //사망 판정
}
