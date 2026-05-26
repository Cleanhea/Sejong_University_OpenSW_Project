using UnityEngine;
using System.Collections;

public class RangeMonster : DefaultMonster
{
    [Header("Range Attack Settings")]
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float attackWindup = 0.8f;    // 전조 시간 (플레이어 회피 가능 구간)
    [SerializeField] private float attackNextDelay = 1.5f; // 다음 공격까지 대기 시간

    public override void Attack()
    {
        if (_target == null)
        {
            ChangeState(MonsterState.Idle);
            return;
        }

        float distance = Vector2.Distance(_rigidbody.position, _target.transform.position);
        if (distance > _AttackRange)
        {
            ChangeState(MonsterState.Move);
            return;
        }

        if (_attackCoroutine == null)
        {
            _attackCoroutine = StartCoroutine(AttackRoutine());
        }
    }

    private IEnumerator AttackRoutine()
    {
        // 상태 애니메이션 중단
        if (_stateAnimCoroutine != null)
        {
            StopCoroutine(_stateAnimCoroutine);
            _stateAnimCoroutine = null;
        }
        _lastAnimState = null;

        // 전조 시작 시점에 플레이어 방향을 고정 — 이후 플레이어가 움직여서 회피 가능
        Vector2 aimDirection = _target != null
            ? ((Vector2)_target.transform.position - (Vector2)transform.position).normalized
            : Vector2.right;

        // 조준 방향으로 스프라이트 flip
        if (aimDirection.x != 0)
            _spriteRenderer.flipX = aimDirection.x < 0;

        // 와인드업 애니메이션 재생 (활 당기기 등)
        if (monsterDataSO.attackSprites != null && monsterDataSO.attackSprites.Length > 0)
        {
            _originalSprite = _spriteRenderer.sprite;
            StartCoroutine(PlayWindupAnimation());
        }

        // 전조 대기 — 이 시간 안에 피하면 회피 성공
        yield return new WaitForSeconds(attackWindup);

        // 투사체 발사
        FireArrow(aimDirection);

        // 다음 공격 쿨다운
        yield return new WaitForSeconds(attackNextDelay);
        _attackCoroutine = null;
    }

    private void FireArrow(Vector2 direction)
    {
        if (arrowPrefab == null)
        {
            Debug.LogWarning($"[RangeMonster] arrowPrefab이 비어있습니다. ({gameObject.name})");
            return;
        }

        Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position;
        GameObject arrowObj = Instantiate(arrowPrefab, spawnPos, Quaternion.identity);

        if (arrowObj.TryGetComponent(out Arrow arrow))
        {
            arrow.Init(direction, _AttackDamage);
        }
        else
        {
            Debug.LogWarning("[RangeMonster] arrowPrefab에 Arrow 컴포넌트가 없습니다.");
        }
    }

    private IEnumerator PlayWindupAnimation()
    {
        foreach (Sprite frame in monsterDataSO.attackSprites)
        {
            if (_spriteRenderer != null)
                _spriteRenderer.sprite = frame;
            yield return new WaitForSeconds(monsterDataSO.attackFrameDuration);
        }

        // 와인드업 끝나면 원래 스프라이트로 복귀
        if (_spriteRenderer != null && _originalSprite != null)
            _spriteRenderer.sprite = _originalSprite;
    }
}
