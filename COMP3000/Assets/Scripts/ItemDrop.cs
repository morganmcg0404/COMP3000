using System;
using System.Collections.Generic;
using UnityEngine;

public enum LootRarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}

[System.Serializable]
public class ItemDrop
{
    [Tooltip("Unique ID of the item that drops")]
    public string itemId;
    
    [Tooltip("Display name for the item")]
    public string itemName;
    
    [Tooltip("Rarity tier of this drop")]
    public LootRarity rarity = LootRarity.Common;
    
    [Tooltip("Chance to drop (0-100%). Multiple items can drop from one kill.")]
    [Range(0f, 100f)]
    public float dropChance = 50f;
    
    [Tooltip("Minimum quantity that drops")]
    [Min(1)]
    public int minQuantity = 1;
    
    [Tooltip("Maximum quantity that drops")]
    [Min(1)]
    public int maxQuantity = 1;
    
    [Tooltip("If true, this item will always drop (ignores drop chance)")]
    public bool alwaysDrop = false;

    public bool RollDrop()
    {
        if (alwaysDrop)
            return true;

        float roll = UnityEngine.Random.Range(0f, 100f);
        return roll <= dropChance;
    }

    public int GetRandomQuantity()
    {
        return UnityEngine.Random.Range(minQuantity, maxQuantity + 1);
    }

    public Item CreateItem()
    {
        return ItemFactory.GetItemById(itemId);
    }
}

public class LootDrop
{
    public Item Item { get; set; }
    public int Quantity { get; set; }
    public LootRarity Rarity { get; set; }

    public LootDrop(Item item, int quantity, LootRarity rarity)
    {
        Item = item;
        Quantity = quantity;
        Rarity = rarity;
    }

    public override string ToString()
    {
        return $"{Quantity}x {Item.ItemName} ({Rarity})";
    }
}
