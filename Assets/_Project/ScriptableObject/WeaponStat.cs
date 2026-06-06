using UnityEngine;

public enum WeaponKind
{
    BasicGun,
    SniperRifle,
    SubmachineGun,
    ShortRangeAoe,
    LongRangeAoe
}

[CreateAssetMenu(fileName = "WeaponStat", menuName = "Stats/WeaponStat")]
public class WeaponStat : ScriptableObject
{
    [Header("Weapon Info")]
    public WeaponKind weaponKind = WeaponKind.BasicGun; // 무기 종류
    public string displayName = "기본총";               // 표시 이름
    public Sprite inventoryIcon;                        // 인벤토리 아이콘
    public Sprite equippedSprite;                       // 장착 스프라이트
    public bool unlockedByDefault = true;               // 기본 해금 여부

    [Header("Projectile Stat")]
    public int damage = 10;                             // 발당 데미지
    public float fireInterval = 0.25f;                  // 발사 간격(초)
    public float projectileSpeed = 15f;                 // 투사체 속도
    public float projectileLifetime = 1.5f;             // 투사체 수명(초)
}
