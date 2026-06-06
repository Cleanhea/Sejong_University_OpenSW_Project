using UnityEngine;

[CreateAssetMenu(fileName = "WeaponStat", menuName = "Stats/WeaponStat")]
public class WeaponStat : ScriptableObject
{
    public int damage = 10;                 // 발당 데미지
    public float fireInterval = 0.25f;      // 발사 간격(초)
    public float projectileSpeed = 15f;     // 투사체 속도
    public float projectileLifetime = 1.5f; // 투사체 수명(초)
}
