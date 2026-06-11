using UnityEngine;

public class Playerstatus : MonoBehaviour
{
    [Header("プレイヤーのマスターデータ")]
    public PlayerData playerData;

    [Header("現在のステータス（ゲーム中に変動する値）")]
    public int level = 1;
    public int currentExp = 0;
    public int currentHp = 0;

    private int bonusHp = 0;
    private int bonusAtk = 0;
    private int bonusDef = 0;

    // プロパティ：基本値 ＋ 成長ボーナス
    public int MaxHp => (playerData != null) ? playerData.maxHp + bonusHp : 0;
    public int Attack => (playerData != null) ? playerData.attack + bonusAtk : 0;
    public int Defense => (playerData != null) ? playerData.defense + bonusDef : 0;

    void Start()
    {
        if (currentHp <= 0) currentHp = MaxHp;
    }

    public int GetNextLevelExp()
    {
        if (playerData == null || playerData.levelTable == null) return 999999;

        if (level < playerData.levelTable.Count)
        {
            return playerData.levelTable[level];
        }

        return 999999;
    }

    public bool IsMaxLevel()
    {
        if (playerData == null || playerData.levelTable == null) return true;
        return level >= playerData.levelTable.Count;
    }

    public void GainExp(int amount)
    {
        
        if (IsMaxLevel()) return;

        currentExp += amount;
        Debug.Log($"{amount} の経験値を獲得！ (Current: {currentExp} / Next: {GetNextLevelExp()})");

        while (currentExp >= GetNextLevelExp())
        {
            LevelUp();

            if (IsMaxLevel())
            {
                break;
            }
        }
    }

    void LevelUp()
    {
        level++;

        if (playerData != null)
        {
            bonusHp += playerData.hpGrowth;
            bonusAtk += playerData.atkGrowth;
            bonusDef += playerData.defGrowth;
        }

        currentHp = MaxHp;

        Debug.Log($"レベルアップ！ Level {level} になった！ (最大HP:{MaxHp} 攻撃力:{Attack} 防御力:{Defense})");
    }
}