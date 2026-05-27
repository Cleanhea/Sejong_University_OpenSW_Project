using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Bullet : MonoBehaviour
{
    private Rigidbody2D rb;                // 물리 컴포넌트
    private float damage;                  // 데미지 값
    private Action<Bullet> releaseToPool;  // 풀 반납 콜백
    private Coroutine lifeRoutine;         // 수명 코루틴 핸들
    private bool released;                 // 반납 중복 방지 플래그

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
    }

    /// <summary>풀이 인스턴스를 생성한 직후 1회 호출하여 반납 콜백을 주입합니다.</summary>
    /// <param name="release">자신을 풀로 반납할 때 호출할 콜백입니다.</param>
    public void Bind(Action<Bullet> release)
    {
        releaseToPool = release;
    }

    /// <summary>총알의 이동 방향, 속도, 데미지, 수명을 설정하고 발사합니다.</summary>
    /// <param name="direction">정규화된 이동 방향 벡터입니다.</param>
    /// <param name="speed">이동 속도입니다.</param>
    /// <param name="damageAmount">충돌 시 입힐 데미지 값입니다.</param>
    /// <param name="lifetime">자동 반납까지의 수명(초)입니다.</param>
    public void Launch(Vector2 direction, float speed, float damageAmount, float lifetime)
    {
        // 1. 재사용 시점이므로 이전 상태를 초기화합니다.
        released = false;
        damage = damageAmount;
        rb.linearVelocity = direction * speed;

        // 2. 잔존 수명 코루틴이 있다면 정지하고 새로 시작합니다.
        if (lifeRoutine != null)
            StopCoroutine(lifeRoutine);
        lifeRoutine = StartCoroutine(LifeRoutine(lifetime));
    }

    // 수명만큼 대기한 뒤 자신을 풀로 반납합니다.
    private IEnumerator LifeRoutine(float lifetime)
    {
        yield return new WaitForSeconds(lifetime);
        Release();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // 1. 이미 반납 처리되었다면 중복 발화를 무시합니다.
        if (released) return;

        // 2. 데미지를 받을 수 있는 대상이면 데미지를 입히고 풀로 반납합니다.
        IDamageable damageable = other.GetComponentInParent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(damage);
            Release();
        }
    }

    // 수명 코루틴을 정리하고 풀로 반납하며 중복 반납을 차단합니다.
    private void Release()
    {
        if (released) return;
        released = true;

        if (lifeRoutine != null)
        {
            StopCoroutine(lifeRoutine);
            lifeRoutine = null;
        }

        // 풀 바인딩이 없는 경우(샌드박스 직접 배치 등)는 안전하게 파괴합니다.
        if (releaseToPool != null)
            releaseToPool(this);
        else
            Destroy(gameObject);
    }
}
