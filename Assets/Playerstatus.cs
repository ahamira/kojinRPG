using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    public int level = 1;
    public int currentExp = 0;
    public int nextLevelExp = 10; 

    public void GainExp(int amount)
    {
        currentExp += amount;
        Debug.Log($"{amount} の経験値を獲得！");

        while (currentExp >= nextLevelExp)
        {
            LevelUp();
        }
    }

    void LevelUp()
    {
        currentExp -= nextLevelExp;
        level++;
        nextLevelExp = Mathf.RoundToInt(nextLevelExp * 1.5f);

        BattleUnit unit = GetComponent<BattleUnit>();
        unit.data.maxHp += 10;
        unit.currentHp = unit.data.maxHp; 
        unit.data.attack += 2;

        Debug.Log($"レベルアップ！ Level {level} になった！");
    }
}
