using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public abstract class DefaultMonster : MonoBehaviour
{
    [Header("Monster Status")]
    [SerializeField] protected MonsterDataSO monsterDataSO;

    protected Rigidbody2D _rigidbody;
    protected SpriteRenderer _spriteRenderer;
    protected Sprite _originalSprite;
    protected Coroutine _attackCoroutine;
    protected Coroutine _stateAnimCoroutine;
    protected MonsterState? _lastAnimState;
    protected bool isStateNotChangeable = false;

    public MonsterState State { get; set; }
    public GameObject _target { get; set; }

    public float _maxHealth => monsterDataSO.maxHealth;
    public float _MoveSpeed => monsterDataSO.MoveSpeed;
    public float _AttackRange => monsterDataSO.AttackRange;
    public float _AttackDamage => monsterDataSO.AttackDamage;

    public float currentHealth;

    protected virtual void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rigidbody.bodyType = RigidbodyType2D.Kinematic; // 외부 물리력·중력 완전 차단, MovePosition으로만 이동
        _rigidbody.gravityScale = 0f;
        _rigidbody.freezeRotation = true;
    }

    protected virtual void Start()
    {
        currentHealth = _maxHealth;
    }

    protected virtual void FixedUpdate()
    {
        if (_attackCoroutine != null) return;
        switch (State)
        {
            case MonsterState.Idle:   Idle();   break;
            case MonsterState.Move:   Move();   break;
            case MonsterState.Attack: Attack(); break;
            case MonsterState.Die:    Die();    break;
        }
    }

    public virtual void Idle()
    {
        PlayStateAnimation(MonsterState.Idle, monsterDataSO.idleSprites, monsterDataSO.idleFrameDuration, loop: true);
    }

    public virtual void Move()
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

    // 각 몬스터 타입마다 공격 방식이 달라서 반드시 구현 필요
    public abstract void Attack();

    public virtual void Die()
    {
        ChangeState(MonsterState.Die);
        isStateNotChangeable = true;
        if (_attackCoroutine != null)
            StopCoroutine(_attackCoroutine);
        StartCoroutine(DieRoutine());
    }

    protected virtual IEnumerator DieRoutine()
    {
        PlayStateAnimation(MonsterState.Die, monsterDataSO.dieSprites, monsterDataSO.dieFrameDuration, loop: false);
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }

    public void ChangeState(MonsterState newState)
    {
        if (isStateNotChangeable) return;
        State = newState;
    }

    protected void PlayStateAnimation(MonsterState state, Sprite[] sprites, float frameDur, bool loop)
    {
        if (_lastAnimState == state) return;
        if (sprites == null || sprites.Length == 0) return;

        if (_stateAnimCoroutine != null)
            StopCoroutine(_stateAnimCoroutine);

        _lastAnimState = state;
        _stateAnimCoroutine = StartCoroutine(PlaySpriteAnimation(sprites, frameDur, loop));
    }

    protected IEnumerator PlaySpriteAnimation(Sprite[] sprites, float frameDur, bool loop)
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
}
