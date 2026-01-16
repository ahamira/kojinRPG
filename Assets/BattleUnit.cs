using UnityEngine;

public class BattleUnit : MonoBehaviour
{
    public UnitData data; 
    [HideInInspector] public int currentHp;
    [HideInInspector] public bool isDead = false;

    public void Setup()
    {
        currentHp = data.maxHp;
        isDead = false;
    }

    public void TakeDamage(int attackerAtk)
    {
        int damage = (attackerAtk / 2) - (data.defense / 4);
        if (damage <= 0) damage = 1;

        currentHp -= damage;
        if (currentHp <= 0)
        {
            currentHp = 0;
            isDead = true;
        }
    }
}