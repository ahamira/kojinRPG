using UnityEngine;

[CreateAssetMenu(fileName = "NewEquipment", menuName = "RPG/Equipment")]
public class EquipmentData : ScriptableObject
{
    public string equipmentName;

    public Sprite icon;

    public int attackBonus;
    public int defenseBonus;
    public int hpBonus;
}