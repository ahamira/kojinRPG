using UnityEngine;
using System.Collections.Generic;

// --- モンスター用の基本データ ---
[CreateAssetMenu(fileName = "NewUnitData", menuName = "Battle/Enemy Data")]
public class UnitData : ScriptableObject
{
    public string unitName;
    public Sprite visual;     
    public int maxHp;
    public int attack;
    public int defense;
    public int dropExp;        
}

// --- プレイヤー用の拡張データ ---
[CreateAssetMenu(fileName = "NewPlayerData", menuName = "Battle/Player Data")]
public class PlayerData : UnitData
{
    [Header("成長の設定")]
    public int currentLevel = 1;
    public int currentExp = 0;
    public List<int> levelTable;
    public int hpGrowth = 3;     
    public int atkGrowth = 1;  

    public bool CanLevelUp()
    {
        if (currentLevel >= levelTable.Count) return false;
        return currentExp >= levelTable[currentLevel];
    }
}