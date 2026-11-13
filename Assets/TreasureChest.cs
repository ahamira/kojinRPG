
using UnityEngine;

public class TreasureChest : MonoBehaviour
{
    [Header("•ó” ‚©‚ç“üè‚·‚é‘•”õ")]
    public EquipmentData equipment;

    [Header("‘•”õ‚Ìí—Ş")]
    public bool isWeapon = true;

    [Header("•ó” ‚Ì‰æ‘œ")]
    public Sprite closedSprite;
    public Sprite openedSprite;

    private bool opened = false;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null && closedSprite != null)
        {
            spriteRenderer.sprite = closedSprite;
        }
    }

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

        if (spriteRenderer != null && openedSprite != null)
        {
            spriteRenderer.sprite = openedSprite;
        }

        Debug.Log(equipment.equipmentName + " ‚ğ“üè‚µ‚½I");
    }
}


