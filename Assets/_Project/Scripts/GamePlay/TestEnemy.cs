using UnityEngine;

// IDamageDealer 구현 — RecieveAttack이 데미지 값을 이 메서드로 가져감
[RequireComponent(typeof(Rigidbody2D))]
public class TestEnemy : MonoBehaviour, IDamageDealer
{
    [SerializeField] private float damage = 10f;

    void Awake()
    {
        // Kinematic: 플레이어를 물리력으로 밀지 않으면서 충돌 감지는 유지
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
    }

    public float GetDamage() => damage;
}
