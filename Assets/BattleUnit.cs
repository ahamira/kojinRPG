using UnityEngine;

public class BattleUnit : MonoBehaviour
{
    public UnitData data;
    private SpriteRenderer spriteRenderer;
    [HideInInspector] public int currentHp;
    [HideInInspector] public bool isDead = false;
    public bool isDefending = false;
    public void Setup()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && data != null)
        {
            spriteRenderer.sprite = data.visual;
        }
        currentHp = data.maxHp;
        isDead = false;
    }

    public void TakeDamage(int attackerAtk)
    {
        int damage = (attackerAtk / 2) - (data.defense / 4);
        if (isDefending)
        {
            damage /= 2;
            isDefending = false;
        }
            if (damage <= 0) damage = 1;

        currentHp -= damage;
        if (currentHp <= 0)
        {
            currentHp = 0;
            isDead = true;
        }
    }

}