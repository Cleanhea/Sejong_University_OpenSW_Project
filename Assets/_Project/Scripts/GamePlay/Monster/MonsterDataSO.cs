using UnityEngine;

[CreateAssetMenu(fileName = "MonsterDataSO", menuName = "ScriptableObjects/MonsterDataSO", order = 1)]
public class MonsterDataSO : ScriptableObject
{
    [Header("Status")]
    public float maxHealth;
    public float MoveSpeed;
    public float AttackRange;
    public float AttackDamage;

    [Header("Idle Animation")]
    public Sprite[] idleSprites;
    public float idleFrameDuration = 0.2f;

    [Header("Move Animation")]
    public Sprite[] moveSprites;
    public float moveFrameDuration = 0.2f;

    [Header("Attack Animation")]
    public Sprite[] attackSprites;
    public float attackFrameDuration = 0.1f;

    [Header("Die Animation")]
    public Sprite[] dieSprites;
    public float dieFrameDuration = 0.15f;

    [Header("Attack Effect")]
    public GameObject attackEffectPrefab;
    public float effectDuration = 0.7f;
}
