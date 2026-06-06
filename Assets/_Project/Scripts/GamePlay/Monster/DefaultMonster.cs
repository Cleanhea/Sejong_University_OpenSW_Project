using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

    [Header("Pathfinding")]
    [SerializeField] protected float pathRefreshInterval = 0.5f;



    [SerializeField] private MonsterHPController hpController;
    [SerializeField] private KillCountSO killCountSO;
    protected List<Vector2> _path;
    protected int           _pathIndex;
    protected float         _pathTimer;

    private const float WaypointRadius = 0.4f;

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

        if ((_rigidbody.position - targetPosition).magnitude <= _AttackRange)
        {
            ChangeState(MonsterState.Attack);
            return;
        }

        // 경로 갱신 조건: 타이머 만료, 경로 없음, 웨이포인트 모두 소진
        _pathTimer -= Time.fixedDeltaTime;
        bool pathExhausted = _path == null || _pathIndex >= _path.Count;
        if (_pathTimer <= 0f || pathExhausted)
        {
            _path      = PathfindingManager.Instance?.FindPath(_rigidbody.position, targetPosition);
            _pathIndex = 1; // index 0은 현재 위치 셀이므로 건너뜀
            _pathTimer = pathRefreshInterval;
        }

        // 다음 웨이포인트 결정
        Vector2 waypoint;
        if (_path != null && _pathIndex < _path.Count)
        {
            waypoint = _path[_pathIndex];
            if (Vector2.Distance(_rigidbody.position, waypoint) < WaypointRadius)
            {
                _pathIndex++;
                if (_pathIndex >= _path.Count) return;
                waypoint = _path[_pathIndex];
            }
        }
        else
        {
            waypoint = targetPosition; // A* 실패 시 직선 이동 폴백
        }

        Vector2 moveDir  = (waypoint - _rigidbody.position).normalized;
        Vector2 nextPos  = Vector2.MoveTowards(_rigidbody.position, waypoint, _MoveSpeed * Time.fixedDeltaTime);

        PlayStateAnimation(MonsterState.Move, monsterDataSO.moveSprites, monsterDataSO.moveFrameDuration, loop: true);
        _rigidbody.MovePosition(nextPos);

        if (moveDir.x != 0)
            _spriteRenderer.flipX = moveDir.x < 0;
    }

    // 각 몬스터 타입마다 공격 방식이 달라서 반드시 구현 필요
    public abstract void Attack();

    [ContextMenu("Test Die")]
    public virtual void Die()
    {
        if(State == MonsterState.Die) return;
        ChangeState(MonsterState.Die);
        isStateNotChangeable = true;
        if (_attackCoroutine != null)
            StopCoroutine(_attackCoroutine);
        killCountSO.UpdateKillCount(1);
        StartCoroutine(DieRoutine());
    }

    public virtual void TakeDamage(int damage)
    {
        hpController.TakeDamage(damage);
        currentHealth -= damage;
        if (currentHealth <= 0)
            Die();
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
