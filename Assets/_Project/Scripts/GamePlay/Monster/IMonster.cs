using UnityEngine;

public interface IMonster
{
    public MonsterState State { get; set; }
    public GameObject _target { get; set; }
    public void ChangeState(MonsterState newState);
    void Idle();
    void Move();
    void Attack();
    void Die();
}
