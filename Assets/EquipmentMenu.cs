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

        weaponText.text = "Buki：" +
            (player.equippedWeapon != null
            ? player.equippedWeapon.equipmentName : "no");

        armorText.text = "Bougu：" +
            (player.equippedArmor != null
            ? player.equippedArmor.equipmentName : "no");

        statusText.text =
            "HP：" + player.currentHp + " / " + player.MaxHp +
            "\nAttack：" + player.Attack +
            "\nDefense：" + player.Defense;
    }
}
