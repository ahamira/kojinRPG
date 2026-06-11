using UnityEngine;
using UnityEngine.UI;
public class BattleUnit : MonoBehaviour
{
    public GameObject damageTextPrefab;
    public UnitData data;
    private Image unitImage;
    [HideInInspector] public int currentHp;
    [HideInInspector] public bool isDead = false;
    public bool isDefending = false;
    public void Setup()
    {
        if (unitImage == null) unitImage = GetComponent<Image>();
        if (unitImage != null && data != null)
        {
            unitImage.sprite = data.visual;
            unitImage.SetNativeSize();
        }

        isDead = false;

        Playerstatus status = GetComponent<Playerstatus>();
        if (status != null)
        {
            currentHp = Mathf.Clamp(status.currentHp, 0, status.MaxHp);
        }
        else if (data != null)
        {
            currentHp = data.maxHp;
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