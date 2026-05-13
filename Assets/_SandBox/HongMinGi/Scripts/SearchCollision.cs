using UnityEngine;

public class SearchCollision : MonoBehaviour
{
    private IMonster meleeMonster;

    void Awake()
    {
        meleeMonster = GetComponentInParent<IMonster>();
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            meleeMonster._target = other.gameObject;
            meleeMonster.State = MonsterState.Move;
            Debug.Log("탐지");
        }
    }
}
