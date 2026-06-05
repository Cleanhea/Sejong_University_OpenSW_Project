using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerAim))]
public class PlayerWeapon : MonoBehaviour
{
    [SerializeField] private PlayerStat playerStat;   // 플레이어 상태
    [SerializeField] private WeaponStat weaponStat;   // 무기 스탯
    [SerializeField] private Transform muzzle;        // 발사 위치
    [SerializeField] private BulletPool bulletPool;   // 총알 풀
    [SerializeField] private SpriteRenderer weaponSpriteRenderer; // 무기 스프라이트 렌더러

    private PlayerAim playerAim;  // 조준 컴포넌트 캐시
    private float fireCooldown;   // 다음 발사까지 남은 시간

    public WeaponStat CurrentWeapon => weaponStat; // 현재 장착 무기

    void Awake()
    {
        playerAim = GetComponent<PlayerAim>();
        ApplyWeaponVisual();
    }

    void Update()
    {
        // 1. 매 프레임 쿨다운을 감소시킵니다.
        if (fireCooldown > 0f)
            fireCooldown -= Time.deltaTime;

        // 2. 사망 또는 넉백 중에는 발사를 차단합니다.
        if (playerStat.playerdead) return;
        if (playerStat.isKnockedBack) return;
        if (weaponStat == null) return;

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
        switch (weaponStat.weaponKind)
        {
            case WeaponKind.BasicGun:
            case WeaponKind.SniperRifle:
            case WeaponKind.SubmachineGun:
                FireSingleBullet();
                break;
            case WeaponKind.ShortRangeAoe:
            case WeaponKind.LongRangeAoe:
                Debug.LogWarning($"[PlayerWeapon] {weaponStat.weaponKind} fire mode is not implemented yet.");
                break;
            default:
                FireSingleBullet();
                break;
        }
    }

    private void FireSingleBullet()
    {
        // 1. 발사 위치와 회전을 결정합니다.
        Vector3 firePosition = muzzle != null ? muzzle.position : transform.position;
        Quaternion fireRotation = Quaternion.Euler(0f, 0f, playerAim.AimAngleDeg);

        // 2. 풀에서 총알을 꺼내 발사 정보로 초기화합니다.
        Bullet bullet = bulletPool.Get(firePosition, fireRotation);
        bullet.Launch(playerAim.AimDirection, weaponStat.projectileSpeed, weaponStat.damage, weaponStat.projectileLifetime);
    }

    /// <summary>
    /// 현재 장착 무기를 교체하고 무기 스프라이트를 갱신합니다.
    /// </summary>
    /// <param name="newWeaponStat">새로 장착할 무기 스탯입니다.</param>
    public void SetWeapon(WeaponStat newWeaponStat)
    {
        if (newWeaponStat == null)
        {
            Debug.LogWarning("[PlayerWeapon] 교체할 무기 스탯이 비어 있습니다.");
            return;
        }

        // 1. 새 무기 스탯을 장착하고 즉시 발사할 수 있도록 쿨다운을 초기화합니다.
        weaponStat = newWeaponStat;
        fireCooldown = 0f;

        // 2. 장착 무기의 표시 스프라이트를 반영합니다.
        ApplyWeaponVisual();
    }

    // 장착 무기에 지정된 스프라이트를 렌더러에 반영합니다.
    private void ApplyWeaponVisual()
    {
        if (weaponSpriteRenderer == null) return;
        if (weaponStat == null) return;
        if (weaponStat.equippedSprite == null) return;

        weaponSpriteRenderer.sprite = weaponStat.equippedSprite;
    }
}
