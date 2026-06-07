using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoController : MonoBehaviour
{
    [System.Serializable]
    public class WeaponSlot
    {
        public Image slotImage;
        public Image borderImage;
        public Sprite emptySprite;
        public bool isUnlocked = false;
    }    

    [Header("HP Bar")]
    [SerializeField] private Slider hpSlider;

    [Header("Weapon Slots")]
    [SerializeField] private WeaponSlot[] weaponSlots = new WeaponSlot[5];

    [Header("Dash Cooldowns")]
    [SerializeField] private Image[] dashImages = new Image[3];

    [Header("Player Stat Data Connector ")]
    [SerializeField] private PlayerStat playerStat;
    [SerializeField] private PlayerWeaponInventory playerWeaponInventory;  // 무기 인벤토리

    private void OnEnable()
    {
        BindPlayerWeaponInventory();
    }

    private void OnDisable()
    {
        if (playerWeaponInventory == null) return;

        playerWeaponInventory.OnWeaponEquipped -= ChangeActiveWeaponBorder;
    }

    void Start()
    {
        for(int i = 0; i < weaponSlots.Length; i++)
        {
            if (weaponSlots[i].slotImage == null) continue;

            if(weaponSlots[i].emptySprite != null)
            {
                weaponSlots[i].slotImage.sprite = weaponSlots[i].emptySprite;
            }

            weaponSlots[i].isUnlocked = (i == 0); // Unlock the first slot by default
        }
        ChangeActiveWeaponBorder(0);

        for(int i = 0; i < dashImages.Length; i++)
        {
            if(dashImages[i] != null) dashImages[i].fillAmount = 0f;
        }

        SyncWeaponSlotsWithInventory();
        SyncActiveWeaponBorder();
    }

    void Update()
    {
        if (playerStat == null) return;

        // HP Bar Update
        if(hpSlider != null)
        {
            hpSlider.maxValue = playerStat.maxHp;
            hpSlider.value = playerStat.currentHp;
        }

        // Dash UI Update
        for (int i = 0; i < dashImages.Length; i++)
        {
            if (dashImages[i] == null) continue;

            if(i < playerStat.currentDashCount)
            {
                dashImages[i].fillAmount = 1f; // Dash available
            }
            else if(i == playerStat.currentDashCount && playerStat.currentDashCount < playerStat.maxDashCount)
            {
                dashImages[i].fillAmount = 1f - (playerStat.dashCooldownTimer / playerStat.dashCooldown);
            }
            else
            {
                dashImages[i].fillAmount = 0f; // Dash on cooldown))ㄱㄱ
            }
        }
    }

    public void UnlockWeapon(int slotIndex, Sprite newWeaponSprite = null)
    {
        if(slotIndex >= 0 && slotIndex < weaponSlots.Length)
        {
            weaponSlots[slotIndex].isUnlocked = true;

            if (newWeaponSprite != null && weaponSlots[slotIndex].slotImage != null)
            {
                weaponSlots[slotIndex].slotImage.sprite = newWeaponSprite;
            }

        }
    }

    public void ChangeActiveWeaponBorder(int activeSlotIndex)
    {
        for (int i = 0; i < weaponSlots.Length; i++)
        {
            if (weaponSlots[i].borderImage == null) continue;
            weaponSlots[i].borderImage.gameObject.SetActive(i == activeSlotIndex);
        }
    }

    // 인벤토리에 등록된 무기 아이콘을 슬롯 UI에 반영합니다.
    private void SyncWeaponSlotsWithInventory()
    {
        BindPlayerWeaponInventory();

        if (playerWeaponInventory == null) return;

        for (int i = 0; i < playerWeaponInventory.WeaponSlots.Count && i < weaponSlots.Length; i++)
        {
            WeaponStat weaponStat = playerWeaponInventory.WeaponSlots[i];
            if (weaponStat == null) continue;

            UnlockWeapon(i, weaponStat.inventoryIcon);
        }
    }

    // 현재 장착된 무기 슬롯의 테두리 상태를 맞춥니다.
    private void SyncActiveWeaponBorder()
    {
        BindPlayerWeaponInventory();

        int activeSlotIndex = 0;
        if (playerWeaponInventory != null && playerWeaponInventory.CurrentSlotIndex >= 0)
            activeSlotIndex = playerWeaponInventory.CurrentSlotIndex;

        ChangeActiveWeaponBorder(activeSlotIndex);
    }

    // 무기 장착 이벤트를 UI 테두리 갱신에 연결합니다.
    private void BindPlayerWeaponInventory()
    {
        if (playerWeaponInventory == null)
            playerWeaponInventory = FindObjectOfType<PlayerWeaponInventory>();

        if (playerWeaponInventory == null) return;

        playerWeaponInventory.OnWeaponEquipped -= ChangeActiveWeaponBorder;
        playerWeaponInventory.OnWeaponEquipped += ChangeActiveWeaponBorder;
    }
}
