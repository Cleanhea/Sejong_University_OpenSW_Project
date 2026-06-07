using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMove : MonoBehaviour
{
    [SerializeField] private PlayerStat playerStat;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
    }

    void Update()
    {
        if (playerStat.playerdead) return;
        if (playerStat.isKnockedBack) return;

        float x = Keyboard.current.aKey.isPressed ? -1 :
                  Keyboard.current.dKey.isPressed ?  1 : 0;
        float y = Keyboard.current.wKey.isPressed ?  1 :
                  Keyboard.current.sKey.isPressed ? -1 : 0;
        rb.linearVelocity = new Vector2(x, y) * playerStat.speed;
    }
}
