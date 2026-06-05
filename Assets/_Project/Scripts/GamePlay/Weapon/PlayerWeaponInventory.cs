using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 해금된 무기 슬롯을 관리하고 숫자키 입력으로 현재 무기를 교체합니다.
/// </summary>
[RequireComponent(typeof(PlayerWeapon))]
public class PlayerWeaponInventory : MonoBehaviour
{
    [SerializeField] private List<WeaponStat> weaponSlots = new List<WeaponStat>(); // 해금된 무기 슬롯
    [SerializeField] private int maxSlotCount = 5;                                  // 최대 슬롯 수
    [SerializeField] private bool equipFirstWeaponOnAwake = true;                   // 시작 시 첫 무기 장착

    private PlayerWeapon playerWeapon; // 무기 발사 컴포넌트 캐시

    /// <summary>
    /// 현재 장착 중인 무기 슬롯 번호입니다.
    /// </summary>
    public int CurrentSlotIndex { get; private set; } = -1;

    /// <summary>
    /// 해금된 무기 슬롯 목록을 읽기 전용으로 반환합니다.
    /// </summary>
    public IReadOnlyList<WeaponStat> WeaponSlots => weaponSlots;

    void Awake()
    {
        playerWeapon = GetComponent<PlayerWeapon>();
        RegisterDefaultWeapon();

        if (equipFirstWeaponOnAwake)
            EquipFirstAvailableWeapon();
    }

    void Update()
    {
        if (Keyboard.current == null) return;

        // 1. 숫자키 입력에 맞는 슬롯 무기를 장착합니다.
        if (Keyboard.current.digit1Key.wasPressedThisFrame) EquipSlot(0);
        if (Keyboard.current.digit2Key.wasPressedThisFrame) EquipSlot(1);
        if (Keyboard.current.digit3Key.wasPressedThisFrame) EquipSlot(2);
        if (Keyboard.current.digit4Key.wasPressedThisFrame) EquipSlot(3);
        if (Keyboard.current.digit5Key.wasPressedThisFrame) EquipSlot(4);
    }

    /// <summary>
    /// 지정한 무기를 해금된 무기 슬롯에 추가합니다.
    /// </summary>
    /// <param name="weaponStat">새로 해금할 무기 스탯입니다.</param>
    /// <returns>새 무기가 슬롯에 추가되면 true를 반환합니다.</returns>
    public bool UnlockWeapon(WeaponStat weaponStat)
    {
        if (weaponStat == null)
        {
            Debug.LogWarning("[PlayerWeaponInventory] 해금할 무기 스탯이 비어 있습니다.");
            return false;
        }

        if (weaponSlots.Contains(weaponStat))
            return false;

        if (weaponSlots.Count >= maxSlotCount)
        {
            Debug.LogWarning("[PlayerWeaponInventory] 무기 슬롯이 모두 찼습니다.");
            return false;
        }

        weaponSlots.Add(weaponStat);
        return true;
    }

    /// <summary>
    /// 지정한 슬롯의 무기를 현재 무기로 장착합니다.
    /// </summary>
    /// <param name="slotIndex">0부터 시작하는 무기 슬롯 번호입니다.</param>
    /// <returns>무기 장착에 성공하면 true를 반환합니다.</returns>
    public bool EquipSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= weaponSlots.Count)
            return false;

        WeaponStat weaponStat = weaponSlots[slotIndex];
        if (weaponStat == null)
            return false;

        playerWeapon.SetWeapon(weaponStat);
        CurrentSlotIndex = slotIndex;
        return true;
    }

    // 현재 PlayerWeapon에 지정된 기본 무기를 첫 슬롯에 등록합니다.
    private void RegisterDefaultWeapon()
    {
        WeaponStat currentWeapon = playerWeapon.CurrentWeapon;
        if (currentWeapon == null) return;
        if (!currentWeapon.unlockedByDefault) return;
        if (weaponSlots.Contains(currentWeapon)) return;
        if (weaponSlots.Count >= maxSlotCount) return;

        weaponSlots.Insert(0, currentWeapon);
    }

    // 슬롯에 등록된 첫 무기를 장착합니다.
    private void EquipFirstAvailableWeapon()
    {
        for (int i = 0; i < weaponSlots.Count; i++)
        {
            if (weaponSlots[i] == null) continue;

            EquipSlot(i);
            return;
        }
    }
}
