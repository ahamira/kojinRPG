using UnityEngine;
using System.Collections.Generic;

public class Playerstatus : MonoBehaviour
{
    [Header("プレイヤーのマスターデータ")]
    public PlayerData playerData;

    [Header("現在のステータス")]
    public int level = 1;
    public int currentExp = 0;
    public int currentHp = 0;

    private int bonusHp = 0;
    private int bonusAtk = 0;
    private int bonusDef = 0;

    [Header("装備")]
    public EquipmentData equippedWeapon;
    public EquipmentData equippedArmor;

    [Header("所持している装備")]
    public List<EquipmentData> ownedWeapons = new List<EquipmentData>();
    public List<EquipmentData> ownedArmors = new List<EquipmentData>();

    public int MaxHp =>
        (playerData != null ? playerData.maxHp + bonusHp : 0)
        + (equippedWeapon != null ? equippedWeapon.hpBonus : 0)
        + (equippedArmor != null ? equippedArmor.hpBonus : 0);

    public int Attack =>
        (playerData != null ? playerData.attack + bonusAtk : 0)
        + (equippedWeapon != null ? equippedWeapon.attackBonus : 0)
        + (equippedArmor != null ? equippedArmor.attackBonus : 0);

    public int Defense =>
        (playerData != null ? playerData.defense + bonusDef : 0)
        + (equippedWeapon != null ? equippedWeapon.defenseBonus : 0)
        + (equippedArmor != null ? equippedArmor.defenseBonus : 0);

    void Start()
    {
        if (currentHp <= 0)
        {
            currentHp = MaxHp;
        }
    }

    public int GetNextLevelExp()
    {
        if (playerData == null || playerData.levelTable == null)
            return 999999;

        if (level < playerData.levelTable.Count)
            return playerData.levelTable[level];

        return 999999;
    }

    public bool IsMaxLevel()
    {
        if (playerData == null || playerData.levelTable == null)
            return true;

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
                break;
        }
    }

    public void HealFull()
    {
        currentHp = MaxHp;
    }

    public void TakeDamage(int damage)
    {
        currentHp = Mathf.Max(0, currentHp - damage);
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

    public void EquipWeapon(EquipmentData weapon)
    {
        equippedWeapon = weapon;
        Debug.Log("武器を装備：" + weapon.equipmentName);
    }

    public void EquipArmor(EquipmentData armor)
    {
        equippedArmor = armor;
        Debug.Log("防具を装備：" + armor.equipmentName);
    }

    public void AddEquipment(EquipmentData equipment, bool isWeapon)
    {
        if (equipment == null) return;

        if (isWeapon)
        {
            if (!ownedWeapons.Contains(equipment))
                ownedWeapons.Add(equipment);
        }
        else
        {
            if (!ownedArmors.Contains(equipment))
                ownedArmors.Add(equipment);
        }

        Debug.Log(equipment.equipmentName + " を入手した！");
    }
}
