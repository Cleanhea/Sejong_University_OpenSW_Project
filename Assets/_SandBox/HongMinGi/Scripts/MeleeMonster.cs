using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class MeleeMonster : MonoBehaviour, IMonster
{
    [SerializeField] MonsterDataSO monsterDataSO;
    [SerializeField] private float rotationOffset;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRadius = 0.7f;
    [SerializeField] private float attackDelay = 0.5f;
    private Rigidbody2D _rigidbody;
    private bool _isAttacking;

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
    }

    void Start()
    {
        currentHealth = _maxHealth;
    }

    void FixedUpdate()
    {
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
    }

    public void Move()
    {
        if (_target == null)
        {
            State = MonsterState.Idle;
            return;
        }

        Vector2 targetPosition = _target.transform.position;
        Vector2 direction = targetPosition - _rigidbody.position;

        if (direction.magnitude <= _AttackRange)
        {
            State = MonsterState.Attack;
            return;
        }

        Vector2 moveDirection = direction.normalized;
        Vector2 nextPosition = Vector2.MoveTowards(
            _rigidbody.position,
            targetPosition,
            _MoveSpeed * Time.fixedDeltaTime
        );
        _rigidbody.MovePosition(nextPosition);

        if (moveDirection != Vector2.zero)
        {
            float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
            _rigidbody.MoveRotation(angle + rotationOffset);
        }
    }

    public void Attack()
    {
        if (_target == null)
        {
            State = MonsterState.Idle;
            return;
        }

        float distance = Vector2.Distance(_rigidbody.position, _target.transform.position);
        if (distance > _AttackRange)
        {
            State = MonsterState.Move;
            return;
        }

        if (!_isAttacking)
        {
            StartCoroutine(AttackRoutine());
        }
    }

    public void Die()
    {
    }

    private IEnumerator AttackRoutine()
    {
        _isAttacking = true;

        yield return new WaitForSeconds(attackDelay);

        Vector2 center = attackPoint != null ? attackPoint.position : transform.position;
        Collider2D[] hits = Physics2D.OverlapCircleAll(center, attackRadius);

        foreach (Collider2D hit in hits)
        {
            Dummy dummy = hit.GetComponent<Dummy>();
            if (dummy != null)
            {
                dummy.TakeDamage((int)_AttackDamage);
            }
        }

        _isAttacking = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 center = attackPoint != null ? attackPoint.position : transform.position;
        Gizmos.DrawWireSphere(center, attackRadius);
    }
}
