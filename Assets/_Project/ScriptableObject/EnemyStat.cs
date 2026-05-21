using UnityEngine;

//테스트파일 추후 삭제 요망

[CreateAssetMenu(fileName = "EnemyStat", menuName = "Stats/EnemyStat")]
public class EnemyStat : ScriptableObject
{
    public float maxHp = 50f;
    public float currentHp = 50f;
    public float attackPower = 10f;
    public float speed = 3f;
}

//여기까지 테스트
