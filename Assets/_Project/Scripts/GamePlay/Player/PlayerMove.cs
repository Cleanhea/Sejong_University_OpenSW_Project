using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private PlayerStat playerStat;
    void Start()
    {
        playerStat = GetComponent<PlayerStat>();
    }
    // Update is called once per frame
    void Update()
    {
        float x =   Keyboard.current.aKey.isPressed ? -1 : 
                    Keyboard.current.dKey.isPressed ? 1 : 0;
        float y =   Keyboard.current.wKey.isPressed ? 1 : 
                    Keyboard.current.sKey.isPressed ? -1 : 0;
        transform.Translate(new Vector3(x, y, 0) * playerStat.speed * Time.deltaTime);

    }
}
