using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerDash : MonoBehaviour
{
    [SerializeField] private PlayerStat playerStat;
    private Rigidbody2D rb;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        if (playerStat.playerdead) return;

        bool canDash = playerStat.currentDashCount > 0;
        if (Keyboard.current.spaceKey.wasPressedThisFrame && canDash)
        {
            StartCoroutine(Dash());
        }

        // 대시 쿨타임 처리
        if (playerStat.currentDashCount < playerStat.maxDashCount)
        {
            playerStat.dashCooldownTimer -= Time.deltaTime;
            if (playerStat.dashCooldownTimer <= 0)
            {
                playerStat.currentDashCount += 1;
                playerStat.dashCooldownTimer = playerStat.dashCooldown;
            }
        }
    }

    IEnumerator Dash()
    {
        playerStat.dashInvincible = true;
        playerStat.dashTimer = playerStat.maxDashTime;

        float x = Keyboard.current.aKey.isPressed ? -1f :
                  Keyboard.current.dKey.isPressed ?  1f : 0;
        float y = Keyboard.current.wKey.isPressed ?  1f :
                  Keyboard.current.sKey.isPressed ? -1f : 0;

        Vector2 dashDir = new Vector2(x,y).normalized;

        while (playerStat.dashTimer > 0)
        {
            rb.linearVelocity = dashDir * playerStat.dashSpeed;
            playerStat.dashTimer -= Time.deltaTime;
            yield return null;
        }

        rb.linearVelocity = Vector2.zero;
        playerStat.currentDashCount--;
        playerStat.dashInvincible = false;
    }
}
