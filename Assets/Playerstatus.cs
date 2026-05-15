using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    public int level = 1;
    public int currentExp = 0;
    public int nextLevelExp = 10;

    public int bonusHp = 0;
    public int bonusAtk = 0;
    public void GainExp(int amount)
    {
        currentExp += amount;
        Debug.Log($"{amount} の経験値を獲得！");

        while (currentExp >= nextLevelExp)
        {
            Debug.Log("GainExpが呼ばれました。加算量: " + amount);
            LevelUp();
        }
    }

    void LevelUp()
    {
        currentExp -= nextLevelExp;
        level++;
        nextLevelExp = Mathf.RoundToInt(nextLevelExp * 1.5f); 

        bonusHp += 10;
        bonusAtk += 2;

        BattleUnit unit = GetComponent<BattleUnit>();
        unit.currentHp = unit.data.maxHp; 

        Debug.Log($"レベルアップ！ Level {level} になった！");
    }
}
