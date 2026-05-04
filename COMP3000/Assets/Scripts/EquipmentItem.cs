using UnityEngine;

public enum EquipmentSlot
{
    Helmet,
    Body,
    Legs,
    Hands,
    Feet,
    Cape,
    Ring,
    Necklace,
    MainHand,
    OffHand
}

[System.Serializable]
public class EquipmentItem
{
    [SerializeField] private string itemName;
    [SerializeField] private EquipmentSlot slot;
    [SerializeField] private EquipmentStats stats;
    [SerializeField] private string description;
    [SerializeField] private Sprite icon;

    public string ItemName => itemName;
    public EquipmentSlot Slot => slot;
    public EquipmentStats Stats => stats;
    public string Description => description;
    public Sprite Icon => icon;

    public EquipmentItem(string name, EquipmentSlot equipmentSlot, EquipmentStats itemStats, string desc = "", Sprite itemIcon = null)
    {
        itemName = name;
        slot = equipmentSlot;
        stats = itemStats;
        description = desc;
        icon = itemIcon;
    }

    public string GetDetailedInfo()
    {
        return $"<b>{itemName}</b>\nSlot: {slot}\n{stats}\n{description}";
    }

    public EquipmentItem Clone()
    {
        return new EquipmentItem(itemName, slot, stats, description, icon);
    }
}
