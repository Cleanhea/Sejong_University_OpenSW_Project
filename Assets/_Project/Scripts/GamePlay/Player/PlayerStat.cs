using UnityEngine;

public class PlayerStat : MonoBehaviour
{
    public float maxHp = 100f;// 최대 체력
    public float currentHp = 100f;// 체력
    public float attackPower = 10f;// 공격력
    public float defensePower = 5f;// 방어력
    public float speed = 5f;// 이동 속도
    public int maxDashCount = 1; // 최대 대시 횟수
    public int currentDashCount = 1;// 현재 대시 횟수
    public float dashCooldown = 2f;// 대시 쿨타임
    public float dashSpeed = 10f;   // 대시 속도
    public bool isInvincible = false; //무적 상태 여부
    public float invincibilityDuration = 2f; // 무적 지속 시간
    public float invincibilityTimer = 0f; // 무적 타이머
    public float knockbackForce = 5f; // 넉백 힘
    
}
