using UnityEngine;
using System.Collections;

public class MeleeMonster : DefaultMonster
{
    [Header("Attack Settings")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRadius = 0.7f;
    [SerializeField] private float attackWindup = 1f;
    [SerializeField] private float attackNextDelay = 0.3f;

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
        // 상태 애니메이션 중단 후 공격 애니메이션 준비
        if (_stateAnimCoroutine != null)
        {
            StopCoroutine(_stateAnimCoroutine);
            _stateAnimCoroutine = null;
        }
        _lastAnimState = null;

        // 공격 시작 시점의 플레이어 위치를 고정 — 이후 회피 판정 기준
        Vector2 telegraphPosition = _target != null ? (Vector2)_target.transform.position : transform.position;
        Coroutine animCoroutine = null;

        // 회피 가능 대기 시간
        yield return new WaitForSeconds(attackWindup);

        if (monsterDataSO.attackSprites != null && monsterDataSO.attackSprites.Length > 0 && _spriteRenderer != null)
        {
            _originalSprite = _spriteRenderer.sprite;
            animCoroutine = StartCoroutine(PlayAttackAnimation());
        }

        if (monsterDataSO.attackEffectPrefab != null)
        {
            GameObject effect = Instantiate(monsterDataSO.attackEffectPrefab, telegraphPosition, Quaternion.identity);
            Destroy(effect, monsterDataSO.effectDuration);
        }

        yield return new WaitForSeconds(0.05f);

        // 캡처된 위치 기준으로 판정 — 플레이어가 벗어났으면 회피 성공
        Collider2D[] hits = Physics2D.OverlapCircleAll(telegraphPosition, attackRadius);
        foreach (Collider2D hit in hits)
        {
            Dummy dummy = hit.GetComponent<Dummy>();
            if (dummy != null)
                dummy.TakeDamage((int)_AttackDamage);
        }

        if (animCoroutine != null)
            yield return animCoroutine;

        yield return new WaitForSeconds(attackNextDelay);
        _attackCoroutine = null;
    }

    private IEnumerator PlayAttackAnimation()
    {
        foreach (Sprite frame in monsterDataSO.attackSprites)
        {
            _spriteRenderer.sprite = frame;
            yield return new WaitForSeconds(monsterDataSO.attackFrameDuration);
        }
        _spriteRenderer.sprite = _originalSprite;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 center = attackPoint != null ? attackPoint.position : transform.position;
        Gizmos.DrawWireSphere(center, attackRadius);
    }
}
