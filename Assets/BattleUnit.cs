using UnityEngine;
using UnityEngine.UI;

public class BattleUnit : MonoBehaviour
{
    public GameObject damageTextPrefab;
    public UnitData data;
    private Image unitImage;

    [HideInInspector] public int currentHp;
    [HideInInspector] public int maxHp;
    [HideInInspector] public int attack;
    [HideInInspector] public int defense;

    [HideInInspector] public bool isDead = false;
    public bool isDefending = false;

    public void Setup(Playerstatus status)
    {
        if (unitImage == null)
            unitImage = GetComponent<Image>();

        if (unitImage != null && data != null)
        {
            unitImage.sprite = data.visual;
            unitImage.SetNativeSize();
        }

        if (status != null)
        {
            maxHp = status.MaxHp;
            attack = status.Attack;
            defense = status.Defense;
            currentHp = status.currentHp;
        }

        isDead = false;
    }

    public void TakeDamage(int attackerAtk)
    {
        int damage = (attackerAtk / 2) - (defense / 4);

        if (isDefending)
            damage /= 2;

        damage = Mathf.Max(1, damage);

        currentHp -= damage;

        if (currentHp <= 0)
        {
            currentHp = 0;
            isDead = true;
        }
    }
    public void SetupEnemy(UnitData enemyData)
    {
        data = enemyData;

        if (unitImage == null)
            unitImage = GetComponent<Image>();

        if (unitImage != null)
        {
            unitImage.sprite = data.visual;
            unitImage.SetNativeSize();
        }

        maxHp = data.maxHp;
        attack = data.attack;
        defense = data.defense;
        currentHp = maxHp;
        isDead = false;
    }
}