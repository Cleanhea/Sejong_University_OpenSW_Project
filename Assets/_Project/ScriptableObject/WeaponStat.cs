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
    public WeaponKind weaponKind = WeaponKind.BasicGun;
    public string displayName = "\uae30\ubcf8\ucd1d";
    public Sprite inventoryIcon;
    public Sprite equippedSprite;
    public bool unlockedByDefault = true;

    [Header("Projectile Stat")]
    public int damage = 10;
    public float fireInterval = 0.25f;
    public float projectileSpeed = 15f;
    public float projectileLifetime = 1.5f;

    [Header("AOE Stat")]
    public float aoeRadius = 2f;
    public float maxChargeTime = 1.5f;
    public float maxChargeMultiplier = 2f;
}
