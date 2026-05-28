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

    public int MaxHp
    {
        get { return (playerData != null) ? playerData.maxHp + bonusHp : 0; }
    }

    public int Attack
    {
        get { return (playerData != null) ? playerData.attack + bonusAtk : 0; }
    }

    public int Defense
    {
        get { return (playerData != null) ? playerData.defense + bonusDef : 0; }
    }

    private BattleUnit battleUnit;

    void Awake()
    {
        battleUnit = GetComponent<BattleUnit>();
    }

    void Start()
    {
        if (currentHp <= 0) currentHp = MaxHp;
        if (battleUnit != null) battleUnit.currentHp = currentHp;
        if (battleUnit != null)
        {
            battleUnit.currentHp = MaxHp;
        }
    }

    // 次のレベルまでに必要な「トータル経験値」をPlayerDataのテーブルから取得
    public int GetNextLevelExp()
    {
        if (playerData == null || playerData.levelTable == null) return 9999;

        // 次のレベルのインデックスがテーブルの範囲内ならその値を返す
        if (level < playerData.levelTable.Count)
        {
            return playerData.levelTable[level];
        }
        return 999999; // カンスト
    }

    public void GainExp(int amount)
    {
        currentExp += amount;
        Debug.Log($"{amount} の経験値を獲得！ (Current: {currentExp} / Next: {GetNextLevelExp()})");

        // PlayerDataのテーブルを参照してレベルアップ判定
        while (currentExp >= GetNextLevelExp())
        {
            LevelUp();
        }
    }

    void LevelUp()
    {
        level++;

        // PlayerDataに設定された成長量に基づいてボーナスを加算
        if (playerData != null)
        {
            bonusHp += playerData.hpGrowth;
            bonusAtk += playerData.atkGrowth;
            bonusDef += playerData.defGrowth;
        }

        currentHp = MaxHp;
        // 増えた最大HPに合わせて、現在のHPも全回復
        if (battleUnit != null)
        {
            battleUnit.currentHp = MaxHp;
        }

        Debug.Log($"レベルアップ！ Level {level} になった！ (最大HP:{MaxHp} 攻撃力:{Attack} 防御力:{Defense})");
    }
}