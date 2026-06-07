using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(PlayerAim))]
public class PlayerWeapon : MonoBehaviour
{
    [SerializeField] private PlayerStat playerStat;
    [SerializeField] private WeaponStat weaponStat;
    [SerializeField] private Transform muzzle;
    [SerializeField] private BulletPool bulletPool;
    [SerializeField] private SpriteRenderer weaponSpriteRenderer;
    [SerializeField] private SpriteRenderer aoeRangeIndicator;
    [SerializeField] private float rangeIndicatorBaseDiameter = 5.12f;
    [SerializeField] private float minChargeIndicatorAlpha = 0.15f;
    [SerializeField] private float maxChargeIndicatorAlpha = 0.75f;
    [SerializeField] private float aoeReleasePulseScale = 1.2f;
    [SerializeField] private float aoeReleasePulseDuration = 0.18f;

    private PlayerAim playerAim;
    private Camera mainCam;
    private float fireCooldown;
    private bool isChargingAoe;
    private float aoeChargeTime;
    private Coroutine aoeIndicatorPulseRoutine;
    private readonly HashSet<GameObject> damagedAoeTargets = new HashSet<GameObject>();

    public WeaponStat CurrentWeapon => weaponStat;

    void Awake()
    {
        playerAim = GetComponent<PlayerAim>();
        mainCam = Camera.main;
        HideAoeIndicator();
        ApplyWeaponVisual();
    }

    void Update()
    {
        if (fireCooldown > 0f)
            fireCooldown -= Time.deltaTime;

        if (playerStat.playerdead || playerStat.isKnockedBack)
        {
            CancelAoeCharge();
            return;
        }

        if (weaponStat == null) return;
        if (Mouse.current == null) return;

        if (IsAoeWeapon())
        {
            HandleAoeInput();
            return;
        }

        HandleGunInput();
    }

    private void HandleGunInput()
    {
        if (Mouse.current.leftButton.isPressed && fireCooldown <= 0f)
        {
            Fire();
            fireCooldown = weaponStat.fireInterval;
        }
    }

    private void HandleAoeInput()
    {
        if (!isChargingAoe && Mouse.current.leftButton.wasPressedThisFrame && fireCooldown <= 0f)
            StartAoeCharge();

        if (!isChargingAoe) return;

        if (Mouse.current.leftButton.isPressed)
            UpdateAoeCharge();

        if (Mouse.current.leftButton.wasReleasedThisFrame)
            ReleaseAoeCharge();
    }

    private void StartAoeCharge()
    {
        StopAoeIndicatorPulse();
        isChargingAoe = true;
        aoeChargeTime = 0f;
        ShowAoeIndicator();
        UpdateAoeIndicator();
    }

    private void UpdateAoeCharge()
    {
        aoeChargeTime = Mathf.Min(aoeChargeTime + Time.deltaTime, weaponStat.maxChargeTime);
        UpdateAoeIndicator();
    }

    private void ReleaseAoeCharge()
    {
        UpdateAoeIndicator();
        Fire();
        fireCooldown = weaponStat.fireInterval;
        FinishAoeCharge();
    }

    private void CancelAoeCharge()
    {
        isChargingAoe = false;
        aoeChargeTime = 0f;
        HideAoeIndicator();
    }

    private void FinishAoeCharge()
    {
        isChargingAoe = false;
        aoeChargeTime = 0f;
        PlayAoeReleasePulse();
    }

    private void ShowAoeIndicator()
    {
        if (aoeRangeIndicator == null) return;

        aoeRangeIndicator.gameObject.SetActive(true);
    }

    private void HideAoeIndicator()
    {
        if (aoeRangeIndicator == null) return;

        StopAoeIndicatorPulse();
        aoeRangeIndicator.gameObject.SetActive(false);
    }

    private void UpdateAoeIndicator()
    {
        if (aoeRangeIndicator == null) return;
        if (weaponStat == null) return;

        aoeRangeIndicator.transform.position = GetAoeCenter();

        float targetDiameter = weaponStat.aoeRadius * 2f;
        float scale = rangeIndicatorBaseDiameter > 0f ? targetDiameter / rangeIndicatorBaseDiameter : targetDiameter;
        aoeRangeIndicator.transform.localScale = new Vector3(scale, scale, 1f);

        float chargeRatio = GetChargeRatio();
        Color color = aoeRangeIndicator.color;
        color.a = Mathf.Lerp(minChargeIndicatorAlpha, maxChargeIndicatorAlpha, chargeRatio);
        aoeRangeIndicator.color = color;
    }

    private void PlayAoeReleasePulse()
    {
        if (aoeRangeIndicator == null) return;

        StopAoeIndicatorPulse();
        aoeIndicatorPulseRoutine = StartCoroutine(AoeReleasePulseRoutine());
    }

    private IEnumerator AoeReleasePulseRoutine()
    {
        Vector3 baseScale = aoeRangeIndicator.transform.localScale;
        Vector3 peakScale = baseScale * aoeReleasePulseScale;
        float halfDuration = Mathf.Max(aoeReleasePulseDuration * 0.5f, 0.01f);

        yield return AnimateAoeIndicatorScale(baseScale, peakScale, halfDuration);
        yield return AnimateAoeIndicatorScale(peakScale, baseScale, halfDuration);

        aoeRangeIndicator.transform.localScale = baseScale;
        aoeRangeIndicator.gameObject.SetActive(false);
        aoeIndicatorPulseRoutine = null;
    }

    private IEnumerator AnimateAoeIndicatorScale(Vector3 from, Vector3 to, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float ratio = Mathf.Clamp01(elapsed / duration);
            aoeRangeIndicator.transform.localScale = Vector3.Lerp(from, to, ratio);
            yield return null;
        }
    }

    private void StopAoeIndicatorPulse()
    {
        if (aoeIndicatorPulseRoutine == null) return;

        StopCoroutine(aoeIndicatorPulseRoutine);
        aoeIndicatorPulseRoutine = null;
    }

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
                FireAoe();
                break;
            default:
                FireSingleBullet();
                break;
        }
    }

    private void FireSingleBullet()
    {
        Vector3 firePosition = muzzle != null ? muzzle.position : transform.position;
        Quaternion fireRotation = Quaternion.Euler(0f, 0f, playerAim.AimAngleDeg);

        Bullet bullet = bulletPool.Get(firePosition, fireRotation);
        bullet.Launch(playerAim.AimDirection, weaponStat.projectileSpeed, weaponStat.damage, weaponStat.projectileLifetime);
    }

    private void FireAoe()
    {
        Vector3 center = GetAoeCenter();
        int damage = GetChargedDamage();
        Collider2D[] hits = Physics2D.OverlapCircleAll(center, weaponStat.aoeRadius);

        damagedAoeTargets.Clear();

        foreach (Collider2D hit in hits)
        {
            if (hit == null) continue;

            IDamageable damageable = hit.GetComponent<IDamageable>();
            if (damageable != null)
            {
                GameObject target = GetDamageTargetObject(damageable) ?? hit.gameObject;
                if (!damagedAoeTargets.Add(target)) continue;

                damageable.TakeDamage(damage);
                continue;
            }

            DefaultMonster monster = hit.GetComponent<DefaultMonster>();
            if (monster == null) continue;
            if (!damagedAoeTargets.Add(monster.gameObject)) continue;

            monster.TakeDamage(damage);
        }
    }

    private bool IsAoeWeapon()
    {
        return weaponStat.weaponKind == WeaponKind.ShortRangeAoe ||
               weaponStat.weaponKind == WeaponKind.LongRangeAoe;
    }

    private Vector3 GetAoeCenter()
    {
        if (weaponStat.weaponKind == WeaponKind.LongRangeAoe)
            return GetMouseWorldPosition();

        return transform.position;
    }

    private Vector3 GetMouseWorldPosition()
    {
        if (mainCam == null)
            mainCam = Camera.main;

        if (mainCam == null || Mouse.current == null)
            return transform.position;

        Vector2 mouseScreen = Mouse.current.position.ReadValue();
        Vector3 mouseWorld = mainCam.ScreenToWorldPoint(mouseScreen);
        mouseWorld.z = transform.position.z;
        return mouseWorld;
    }

    private float GetChargeRatio()
    {
        if (weaponStat == null) return 0f;
        if (weaponStat.maxChargeTime <= 0f) return 1f;

        return Mathf.Clamp01(aoeChargeTime / weaponStat.maxChargeTime);
    }

    private int GetChargedDamage()
    {
        float multiplier = Mathf.Lerp(1f, weaponStat.maxChargeMultiplier, GetChargeRatio());
        return Mathf.RoundToInt(weaponStat.damage * multiplier);
    }

    private GameObject GetDamageTargetObject(IDamageable damageable)
    {
        if (damageable is Component component)
            return component.gameObject;

        return null;
    }

    public void SetWeapon(WeaponStat newWeaponStat)
    {
        if (newWeaponStat == null)
        {
            Debug.LogWarning("[PlayerWeapon] Weapon stat is empty.");
            return;
        }

        CancelAoeCharge();
        weaponStat = newWeaponStat;
        fireCooldown = 0f;
        ApplyWeaponVisual();
    }

    private void ApplyWeaponVisual()
    {
        if (weaponSpriteRenderer == null) return;
        if (weaponStat == null) return;
        if (weaponStat.equippedSprite == null) return;

        weaponSpriteRenderer.sprite = weaponStat.equippedSprite;
    }
}
