using UnityEngine;

public class TreasureChest : MonoBehaviour
{
    [Header("•ó” ‚©‚ç“üè‚·‚é‘•”õ")]
    public EquipmentData equipment;

    [Header("‘•”õ‚Ìí—Ş")]
    public bool isWeapon = true;

    private bool opened = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (opened) return;

        if (!other.CompareTag("Player")) return;

        Playerstatus player = other.GetComponent<Playerstatus>();

        if (player == null || equipment == null) return;

        if (isWeapon)
        {
            player.EquipWeapon(equipment);
        }
        else
        {
            player.EquipArmor(equipment);
        }

        opened = true;

        Debug.Log(equipment.equipmentName + " ‚ğ“üè‚µ‚½I");
    }
}

