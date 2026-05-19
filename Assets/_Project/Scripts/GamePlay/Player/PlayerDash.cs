using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDash : MonoBehaviour
{
    [SerializeField] private PlayerStat playerStat;

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
        playerStat.isInvincible = true;
        playerStat.dashTimer = playerStat.maxDashTime;

        float x = Keyboard.current.aKey.isPressed ? -1f :
                  Keyboard.current.dKey.isPressed ?  1f : 0;
        float y = Keyboard.current.wKey.isPressed ?  1f :
                  Keyboard.current.sKey.isPressed ? -1f : 0;

        while (playerStat.dashTimer > 0)
        {
            transform.Translate(new Vector3(x, y, 0) * (playerStat.dashSpeed * Time.deltaTime));
            playerStat.dashTimer -= Time.deltaTime;
            yield return null;
        }

        playerStat.currentDashCount--;
        playerStat.isInvincible = false;
    }
}
