using TMPro;
using UnityEngine;

/// <summary>
/// RangeMonster가 발사하는 투사체.
/// Prefab에 부착하고, Tag를 "EnemyBullet"으로 설정해야 RecieveAttack이 감지함.
/// Physics 2D Layer Collision Matrix에서 몬스터 레이어끼리 충돌 무시 설정 권장.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Arrow : MonoBehaviour, IDamageDealer
{
    [SerializeField] private float speed = 8f;
    [SerializeField] private float lifetime = 5f;
    private string[] ArrowText = { "Null", "0", "1" };
    private TextMeshPro arrowTextMesh;

    private float _damage;
    private Vector2 _direction;
    private Rigidbody2D _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.bodyType = RigidbodyType2D.Kinematic; // 외부 물리력 영향 X, 직접 위치 제어
        _rb.gravityScale = 0f;
        _rb.freezeRotation = true;
        arrowTextMesh = GetComponent<TextMeshPro>();
    }
    private void Start()
    {
        if (arrowTextMesh != null)
        {
            arrowTextMesh.text = ArrowText[Random.Range(0, ArrowText.Length)];
        }
    }
    /// <summary>
    /// RangeMonster가 발사 직후 호출 — 날아갈 방향과 데미지를 주입한다.
    /// </summary>
    public void Init(Vector2 direction, float damage)
    {
        _direction = direction.normalized;
        _damage = damage;

        // 날아가는 방향으로 스프라이트 회전 (오른쪽이 0도 기준)
        float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        Destroy(gameObject, lifetime);
    }

    private void FixedUpdate()
    {
        // Kinematic body는 MovePosition으로 이동 (linearVelocity 무시됨)
        _rb.MovePosition(_rb.position + _direction * (speed * Time.fixedDeltaTime));
    }

    // RecieveAttack.CollisionHit()에서 이 값을 가져가 플레이어에게 데미지 적용
    public float GetDamage() => _damage;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        Destroy(gameObject);
    }
}
