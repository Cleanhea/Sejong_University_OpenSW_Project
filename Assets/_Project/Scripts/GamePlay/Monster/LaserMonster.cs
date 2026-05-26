using UnityEngine;
using System.Collections;

/// <summary>
/// 히트스캔 레이저를 발사하는 원거리 몬스터.
///
/// [Prefab 설정]
///  - LineRenderer 컴포넌트 필수 (positionCount = 2)
///  - hitLayerMask: 플레이어·벽 레이어 포함, 몬스터 레이어 제외 권장
///
/// [공격 흐름]
///  1) 와인드업  : 가느다란 예고 레이저가 플레이어를 실시간 추적 (회피 창)
///  2) 발사      : 방향 고정 → Physics2D.Raycast → PlayerHit.Hit() 직접 호출
///  3) 쿨다운    : 레이저 소멸 후 다음 공격 대기
/// </summary>
[RequireComponent(typeof(LineRenderer))]
public class LaserMonster : DefaultMonster
{
    [Header("Laser Attack Settings")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private float attackWindup    = 1.5f;  // 예고 레이저 추적 시간 (회피 창)
    [SerializeField] private float laserDuration   = 0.4f;  // 발사 레이저 표시 지속 시간
    [SerializeField] private float attackNextDelay = 2f;    // 쿨다운
    [SerializeField] private float laserMaxRange   = 15f;   // 최대 사거리
    [SerializeField] private LayerMask hitLayerMask = Physics2D.AllLayers; // 레이캐스트 감지 레이어

    [Header("Laser Visual")]
    [SerializeField] private Color previewColor = new Color(1f, 0.2f, 0.2f, 0.35f); // 예고: 반투명 빨강
    [SerializeField] private Color fireColor    = new Color(1f, 0.15f, 0.15f, 1f);  // 발사: 불투명 빨강
    [SerializeField] private float previewWidth = 0.04f;
    [SerializeField] private float fireWidth    = 0.18f;

    private LineRenderer _lineRenderer;

    // ────────────────────────────────────────────────────────────
    // 초기화
    // ────────────────────────────────────────────────────────────

    protected override void Awake()
    {
        base.Awake();
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.positionCount = 2;
        _lineRenderer.useWorldSpace = true;
        _lineRenderer.enabled = false;
    }

    // ────────────────────────────────────────────────────────────
    // 상태 머신
    // ────────────────────────────────────────────────────────────

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
            _attackCoroutine = StartCoroutine(AttackRoutine());
    }

    // ────────────────────────────────────────────────────────────
    // 공격 코루틴
    // ────────────────────────────────────────────────────────────

    private IEnumerator AttackRoutine()
    {
        // 상태 애니메이션 중단
        if (_stateAnimCoroutine != null)
        {
            StopCoroutine(_stateAnimCoroutine);
            _stateAnimCoroutine = null;
        }
        _lastAnimState = null;
        _originalSprite = _spriteRenderer.sprite;

        // ── 1단계: 와인드업 ─ 예고 레이저가 플레이어를 실시간 추적 ──────────────
        float elapsed = 0f;
        while (elapsed < attackWindup)
        {
            if (_target != null)
            {
                Vector2 origin  = GetFireOrigin();
                Vector2 trackDir = ((Vector2)_target.transform.position - origin).normalized;

                DrawLaser(origin, trackDir, laserMaxRange, previewColor, previewWidth);

                if (trackDir.x != 0)
                    _spriteRenderer.flipX = trackDir.x < 0;
            }
            elapsed += Time.deltaTime;
            yield return null; // Update 주기로 매 프레임 갱신
        }

        // ── 2단계: 발사 ─ 방향 고정 후 즉시 히트스캔 ─────────────────────────
        Vector2 fireOrigin = GetFireOrigin();
        Vector2 fireDir = _target != null
            ? ((Vector2)_target.transform.position - fireOrigin).normalized
            : (_spriteRenderer.flipX ? Vector2.left : Vector2.right);

        yield return new WaitForSeconds(laserDuration);
        FireHitscan(fireOrigin, fireDir);
        yield return new WaitForSeconds(laserDuration);

        // ── 3단계: 쿨다운 ──────────────────────────────────────────────────
        _lineRenderer.enabled = false;
        _spriteRenderer.sprite = _originalSprite;

        yield return new WaitForSeconds(attackNextDelay);
        _attackCoroutine = null;
    }

    // ────────────────────────────────────────────────────────────
    // 히트스캔
    // ────────────────────────────────────────────────────────────

    private void FireHitscan(Vector2 origin, Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.Raycast(origin, direction, laserMaxRange, hitLayerMask);

        Vector2 endPoint;
        if (hit.collider != null)
        {
            endPoint = hit.point;

            // 플레이어에게 직접 데미지 적용 (충돌 이벤트 없이 즉시 판정)
            if (hit.collider.TryGetComponent(out PlayerHit playerHit))
                playerHit.Hit(_AttackDamage, origin);
        }
        else
        {
            endPoint = origin + direction * laserMaxRange;
        }

        float actualRange = Vector2.Distance(origin, endPoint);
        DrawLaser(origin, direction, actualRange, fireColor, fireWidth);
    }

    // ────────────────────────────────────────────────────────────
    // LineRenderer 헬퍼
    // ────────────────────────────────────────────────────────────

    private void DrawLaser(Vector2 origin, Vector2 direction, float range, Color color, float width)
    {
        _lineRenderer.enabled = true;
        _lineRenderer.SetPosition(0, origin);
        _lineRenderer.SetPosition(1, origin + direction * range);
        _lineRenderer.startWidth = width;
        _lineRenderer.endWidth   = width * 0.5f; // 끝으로 갈수록 가늘어짐
        _lineRenderer.startColor = color;
        _lineRenderer.endColor   = color;
    }

    private Vector2 GetFireOrigin()
        => firePoint != null ? (Vector2)firePoint.position : _rigidbody.position;

    // ────────────────────────────────────────────────────────────
    // 사망 시 레이저 강제 종료
    // ────────────────────────────────────────────────────────────

    protected override IEnumerator DieRoutine()
    {
        _lineRenderer.enabled = false;
        PlayStateAnimation(MonsterState.Die, monsterDataSO.dieSprites, monsterDataSO.dieFrameDuration, loop: false);
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
}
