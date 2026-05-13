using UnityEngine;

[CreateAssetMenu(fileName = "MonsterDataSO", menuName = "ScriptableObjects/MonsterDataSO", order = 1)]
public class MonsterDataSO : ScriptableObject
{
    [SerializeField] public float maxHealth;
    [SerializeField] public float MoveSpeed;
    [SerializeField] public float AttackRange;
    [SerializeField] public float AttackDamage;
}