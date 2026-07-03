using UnityEngine;
using System.Collections.Generic;

// --- モンスターのベースデータ ---
[CreateAssetMenu(fileName = "NewUnitData", menuName = "Battle/Enemy Data")]
public class UnitData : ScriptableObject
{
    public string unitName;
    public Sprite visual;
    public int maxHp;
    public int attack;
    public int defense;
    public int exp;
    public Sprite unitSprite;
}

// --- プレイヤーのデータ（UnitDataを継承） ---
[CreateAssetMenu(fileName = "NewPlayerData", menuName = "Battle/Player Data")]
public class PlayerData : UnitData
{
    [Header("成長の設定（レベルごとの必要【累計】経験値リスト）")]
    public List<int> levelTable;

    [Header("1レベルあたりの成長量")]
    public int hpGrowth = 3;
    public int atkGrowth = 1;
    public int defGrowth = 1;
}
