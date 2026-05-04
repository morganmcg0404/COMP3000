using UnityEngine;

[System.Serializable]
public class Item
{
    [SerializeField] private string itemId;
    [SerializeField] private string itemName;
    [SerializeField] private string description;
    [SerializeField] private Sprite icon;
    [SerializeField] private bool isStackable;
    [SerializeField] private int maxStackSize;

    public string ItemId => itemId;
    public string ItemName => itemName;
    public string Description => description;
    public Sprite Icon => icon;
    public bool IsStackable => isStackable;
    public int MaxStackSize => maxStackSize;

    public Item(string id, string name, string desc = "", Sprite itemIcon = null, bool stackable = true, int maxStack = 999)
    {
        itemId = id;
        itemName = name;
        description = desc;
        icon = itemIcon;
        isStackable = stackable;
        maxStackSize = stackable ? maxStack : 1;
    }

    public Item Clone()
    {
        return new Item(itemId, itemName, description, icon, isStackable, maxStackSize);
    }

    public bool CanStackWith(Item other)
    {
        if (other == null) return false;
        return isStackable && itemId == other.itemId;
    }

    public override string ToString()
    {
        return $"{itemName} (ID: {itemId})";
    }
}
