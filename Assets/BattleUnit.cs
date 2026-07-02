using UnityEngine;
using UnityEngine.UI;
public class BattleUnit : MonoBehaviour
{
    public GameObject damageTextPrefab;
    public UnitData data;
    private Image unitImage;
    [HideInInspector] public int maxHp;
    [HideInInspector] public int attack;
    [HideInInspector] public int defense;
    [HideInInspector] public int currentHp;
    [HideInInspector] public bool isDead = false;
    public bool isDefending = false;
    public bool isPlayer;

    public void Setup()
    {
        if (unitImage == null) unitImage = GetComponent<Image>();

        if (unitImage != null && data != null)
        {
            unitImage.sprite = data.visual;
            unitImage.SetNativeSize();
        }

        isDead = false;

        if (!isPlayer && data != null)
        {
            maxHp = data.maxHp;
            attack = data.attack;
            defense = data.defense;
            currentHp = maxHp;
        }
    }

    public void TakeDamage(int attackerAtk)
    {
        int targetDef = (data != null) ? data.defense : 0;

        Playerstatus status = GetComponent<Playerstatus>();
        if (status != null)
        {
            targetDef = status.Defense; 
        }
        int damage = (attackerAtk / 2) - (data.defense / 4);
        if (isDefending)
        {
            damage /= 2;
        }
        if (damage <= 0) damage = 1;

        currentHp -= damage;

        if (damageTextPrefab != null)
        {
            GameObject popup = Instantiate(damageTextPrefab, transform.position, Quaternion.identity, transform.parent.parent);
            if (popup.TryGetComponent<DamagePopup>(out var popupScript))
            {
                popupScript.Setup(damage);
            }
        }
        if (currentHp <= 0)
        {
            currentHp = 0;
            isDead = true;
        }

    }
}