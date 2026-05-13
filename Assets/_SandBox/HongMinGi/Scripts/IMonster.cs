using UnityEngine;

public interface IMonster
{
    public MonsterState State { get; set; }
    public GameObject _target { get; set; }
    void Idle();
    void Move();
    void Attack();
    void Die();
}
