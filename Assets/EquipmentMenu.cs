using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class EquipmentMenu : MonoBehaviour
{
    public Playerstatus player;

    public TMP_Text weaponText;
    public TMP_Text armorText;
    public TMP_Text statusText;

    public GameObject equipmentPanel;

    private bool isOpen = false;

    void Start()
    {
        equipmentPanel.SetActive(false);
    }

    void Update()
    {
        if (Keyboard.current != null &&
            Keyboard.current.iKey.wasPressedThisFrame)
        {
            ToggleMenu();
        }

        if (isOpen)
        {
            UpdateMenu();
        }
    }

    public void ToggleMenu()
    {
        isOpen = !isOpen;
        equipmentPanel.SetActive(isOpen);

        if (isOpen)
        {
            UpdateMenu();
        }
    }

    public void UpdateMenu()
    {
        if (player == null) return;

        weaponText.text = "ïêäÌÅF" +
            (player.equippedWeapon != null
            ? player.equippedWeapon.equipmentName : "Ç»Çµ");

        armorText.text = "ñhãÔÅF" +
            (player.equippedArmor != null
            ? player.equippedArmor.equipmentName : "Ç»Çµ");

        statusText.text =
            "HPÅF" + player.currentHp + " / " + player.MaxHp +
            "\nçUåÇÅF" + player.Attack +
            "\nñhå‰ÅF" + player.Defense;
    }
}
