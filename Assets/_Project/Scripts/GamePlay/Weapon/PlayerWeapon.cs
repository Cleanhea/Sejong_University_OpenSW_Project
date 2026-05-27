using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerAim))]
public class PlayerWeapon : MonoBehaviour
{
    [SerializeField] private PlayerStat playerStat;   // 플레이어 상태
    [SerializeField] private WeaponStat weaponStat;   // 무기 스탯
    [SerializeField] private Transform muzzle;        // 발사 위치
    [SerializeField] private BulletPool bulletPool;   // 총알 풀

    private PlayerAim playerAim;  // 조준 컴포넌트 캐시
    private float fireCooldown;   // 다음 발사까지 남은 시간

    void Awake()
    {
        playerAim = GetComponent<PlayerAim>();
    }

    void Update()
    {
        // 1. 매 프레임 쿨다운을 감소시킵니다.
        if (fireCooldown > 0f)
            fireCooldown -= Time.deltaTime;

        // 2. 사망 또는 넉백 중에는 발사를 차단합니다.
        if (playerStat.playerdead) return;
        if (playerStat.isKnockedBack) return;

        // 3. 좌클릭이 눌려 있고 쿨다운이 끝났다면 발사합니다.
        if (Mouse.current.leftButton.isPressed && fireCooldown <= 0f)
        {
            Fire();
            fireCooldown = weaponStat.fireInterval;
        }
    }

    // 현재 조준 방향으로 풀에서 총알을 꺼내 발사합니다.
    private void Fire()
    {
        // 1. 발사 위치와 회전을 결정합니다.
        Vector3 firePosition = muzzle != null ? muzzle.position : transform.position;
        Quaternion fireRotation = Quaternion.Euler(0f, 0f, playerAim.AimAngleDeg);

        // 2. 풀에서 총알을 꺼내 발사 정보로 초기화합니다.
        Bullet bullet = bulletPool.Get(firePosition, fireRotation);
        bullet.Launch(playerAim.AimDirection, weaponStat.projectileSpeed, weaponStat.damage, weaponStat.projectileLifetime);
    }
}
