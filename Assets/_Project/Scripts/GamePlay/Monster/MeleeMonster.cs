using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class MeleeMonster : MonoBehaviour, IMonster
{
    [Header("Monster Status")]
    [SerializeField] MonsterDataSO monsterDataSO;


    [Header("Attack Settings")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRadius = 0.7f;
    [SerializeField] private float attackWindup = 1f;
    [SerializeField] private float attackNextDelay = 0.3f;


    private Rigidbody2D _rigidbody;
    private SpriteRenderer _spriteRenderer;
    private Sprite _originalSprite;
    private Coroutine _attackCoroutine;
    private Coroutine _stateAnimCoroutine;
    private MonsterState? _lastAnimState;
    private bool isStateNotChangeable = false;

    public MonsterState State { get; set; }

    public float _maxHealth => monsterDataSO.maxHealth;
    public float _MoveSpeed => monsterDataSO.MoveSpeed;
    public float _AttackRange => monsterDataSO.AttackRange;
    public float _AttackDamage => monsterDataSO.AttackDamage;

    public float currentHealth;

    public GameObject _target { get; set; }

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rigidbody.freezeRotation = true;
    }

    void Start()
    {
        currentHealth = _maxHealth;
    }

    void FixedUpdate()
    {
        if (_attackCoroutine != null) return;
        switch (State)
        {
            case MonsterState.Idle:
                Idle();
                break;
            case MonsterState.Move:
                Move();
                break;
            case MonsterState.Attack:
                Attack();
                break;
            case MonsterState.Die:
                Die();
                break;
        }
    }

    public void Idle()
    {
        PlayStateAnimation(MonsterState.Idle, monsterDataSO.idleSprites, monsterDataSO.idleFrameDuration, loop: true);
    }

    public void Move()
    {
        if (_target == null)
        {
            ChangeState(MonsterState.Idle);
            return;
        }

        Vector2 targetPosition = _target.transform.position;
        Vector2 direction = targetPosition - _rigidbody.position;

        if (direction.magnitude <= _AttackRange)
        {
            ChangeState(MonsterState.Attack);
            return;
        }

        Vector2 moveDirection = direction.normalized;
        Vector2 nextPosition = Vector2.MoveTowards(
            _rigidbody.position,
            targetPosition,
            _MoveSpeed * Time.fixedDeltaTime
        );
        PlayStateAnimation(MonsterState.Move, monsterDataSO.moveSprites, monsterDataSO.moveFrameDuration, loop: true);
        _rigidbody.MovePosition(nextPosition);

        if (moveDirection.x != 0)
            _spriteRenderer.flipX = moveDirection.x < 0;
    }

    public void Attack()
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

    public void Die()
    {
        ChangeState(MonsterState.Die);
        isStateNotChangeable = true;
        if(_attackCoroutine != null)
            StopCoroutine(_attackCoroutine);
        StartCoroutine(DieRoutine());
    }

    private IEnumerator DieRoutine()
    {
        PlayStateAnimation(MonsterState.Die, monsterDataSO.dieSprites, monsterDataSO.dieFrameDuration, loop: false);
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
    private IEnumerator AttackRoutine()
    {
        _spriteRenderer.sprite = monsterDataSO.attackSprites[0];
        // 상태 애니메이션 중단 후 공격 애니메이션 재생
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

    private void PlayStateAnimation(MonsterState state, Sprite[] sprites, float frameDur, bool loop)
    {
        if (_lastAnimState == state) return;
        if (sprites == null || sprites.Length == 0) return;

        if (_stateAnimCoroutine != null)
            StopCoroutine(_stateAnimCoroutine);

        _lastAnimState = state;
        _stateAnimCoroutine = StartCoroutine(PlaySpriteAnimation(sprites, frameDur, loop));
    }

    private IEnumerator PlaySpriteAnimation(Sprite[] sprites, float frameDur, bool loop)
    {
        do
        {
            foreach (Sprite frame in sprites)
            {
                _spriteRenderer.sprite = frame;
                yield return new WaitForSeconds(frameDur);
            }
        } while (loop);

        _stateAnimCoroutine = null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 center = attackPoint != null ? attackPoint.position : transform.position;
        Gizmos.DrawWireSphere(center, attackRadius);
    }

    public void ChangeState(MonsterState newState)
    {
        if(isStateNotChangeable) return;
        State = newState;
    }
}
