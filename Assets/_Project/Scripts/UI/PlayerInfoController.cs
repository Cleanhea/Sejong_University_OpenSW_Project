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
}
