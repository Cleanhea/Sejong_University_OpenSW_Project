using UnityEngine;

public class SearchCollision : MonoBehaviour
{
    private DefaultMonster monster;

    void Awake()
    {
        monster = GetComponentInParent<DefaultMonster>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            monster._target = other.gameObject;
            monster.ChangeState(MonsterState.Move);
            Debug.Log("탐지");
        }
    }
}
