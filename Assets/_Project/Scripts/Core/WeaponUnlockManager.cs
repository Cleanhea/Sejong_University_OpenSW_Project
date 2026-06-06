using System;
using UnityEngine;

public class WeaponUnlockManager : MonoBehaviour
{
    [Serializable]
    private class WeaponUnlockRule
    {
        public string label;
        public int requiredKillCount;
        public WeaponStat weaponStat;
        public bool unlocked;
    }

    [SerializeField] private KillCountSO killCountSO;
    [SerializeField] private PlayerWeaponInventory playerWeaponInventory;
    [SerializeField] private PlayerInfoController playerInfoController;

    [SerializeField]
    private WeaponUnlockRule[] unlockRules =
    {
        new WeaponUnlockRule { label = "Sniper Rifle", requiredKillCount = 3 },
        new WeaponUnlockRule { label = "Submachine Gun", requiredKillCount = 6 },
        new WeaponUnlockRule { label = "Short Range AOE", requiredKillCount = 9 },
        new WeaponUnlockRule { label = "Long Range AOE", requiredKillCount = 12 },
    };

    private void Awake()
    {
        if (playerWeaponInventory == null)
            playerWeaponInventory = FindObjectOfType<PlayerWeaponInventory>();

        if (playerInfoController == null)
            playerInfoController = FindObjectOfType<PlayerInfoController>();
    }

    private void OnEnable()
    {
        if (killCountSO == null)
        {
            Debug.LogWarning("[WeaponUnlockManager] KillCountSO is not assigned.");
            return;
        }

        killCountSO.OnKillCountChanged += HandleKillCountChanged;
        HandleKillCountChanged(killCountSO.KillCount);
    }

    private void OnDisable()
    {
        if (killCountSO == null) return;

        killCountSO.OnKillCountChanged -= HandleKillCountChanged;
    }

    private void HandleKillCountChanged(int killCount)
    {
        if (playerWeaponInventory == null)
        {
            Debug.LogWarning("[WeaponUnlockManager] PlayerWeaponInventory is not assigned.");
            return;
        }

        for (int i = 0; i < unlockRules.Length; i++)
        {
            WeaponUnlockRule rule = unlockRules[i];
            if (rule == null) continue;
            if (rule.unlocked) continue;
            if (killCount < rule.requiredKillCount) continue;

            UnlockRule(rule);
        }
    }

    private void UnlockRule(WeaponUnlockRule rule)
    {
        if (rule.weaponStat == null)
        {
            Debug.LogWarning($"[WeaponUnlockManager] WeaponStat is not assigned for {rule.label}.");
            return;
        }

        int slotIndex = playerWeaponInventory.WeaponSlots.Count;
        bool added = playerWeaponInventory.UnlockWeapon(rule.weaponStat);

        rule.unlocked = true;

        if (!added) return;

        if (playerInfoController == null)
            playerInfoController = FindObjectOfType<PlayerInfoController>();

        if (playerInfoController != null)
            playerInfoController.UnlockWeapon(slotIndex, rule.weaponStat.inventoryIcon);
    }
}
