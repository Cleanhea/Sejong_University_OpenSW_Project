using UnityEngine;

// 적, 총알, 레이저가 이 인터페이스를 구현하면 데미지 값을 자동으로 가져옴
public interface IDamageDealer      //민기님 이거 구현해주세영
{
    float GetDamage();
}



[RequireComponent(typeof(PlayerHit))]
public class RecieveAttack : MonoBehaviour
{
    private PlayerHit playerHit;

    void Start()
    {
        playerHit = GetComponent<PlayerHit>();
    }

    // 총알: 충돌 순간 1회 히트
    // 레이저: Enter 시 히트 (Stay는 무적 시간이 반복 피격 방어)
    // 적 몬스터: Stay로 지속 접촉 히트
    // ※ 플레이어 Collider2D의 Is Trigger를 false로 설정해야 물리 충돌이 작동합니다
    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"Hit by: {collision.gameObject.name}, Tag: {collision.gameObject.tag}");
        if (collision.collider.CompareTag("EnemyBullet") || collision.collider.CompareTag("EnemyLaser"))
        {
            CollisionHit(collision);
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Enemy") || collision.collider.CompareTag("EnemyLaser"))
        {
            CollisionHit(collision);
        }
    }

    private void CollisionHit(Collision2D collision)
    {
        IDamageDealer dealer = collision.collider.GetComponent<IDamageDealer>();
        float damage = dealer != null ? dealer.GetDamage() : 1f;
        playerHit.Hit(damage, collision.transform.position);
    }
}
