using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LootTable : MonoBehaviour
{
    [Header("Drop Table Configuration")]
    [Tooltip("List of items that can drop from this enemy")]
    [SerializeField] private List<ItemDrop> dropTable = new List<ItemDrop>();

    [Header("Drop Settings")]
    [Tooltip("Multiply all drop chances by this value")]
    [Range(0.1f, 10f)]
    [SerializeField] private float dropRateMultiplier = 1f;

    [Tooltip("Maximum number of different items that can drop at once (0 = unlimited)")]
    [SerializeField] private int maxDropsPerKill = 0;

    public List<ItemDrop> DropTable => dropTable;

    public List<LootDrop> RollLoot()
    {
        List<LootDrop> droppedItems = new List<LootDrop>();

        foreach (ItemDrop itemDrop in dropTable)
        {
            // Check if we've hit max drops limit
            if (maxDropsPerKill > 0 && droppedItems.Count >= maxDropsPerKill)
                break;

            // Apply drop rate multiplier
            float adjustedDropChance = itemDrop.dropChance * dropRateMultiplier;
            adjustedDropChance = Mathf.Clamp(adjustedDropChance, 0f, 100f);

            // Roll for this item
            bool shouldDrop = itemDrop.alwaysDrop || (Random.Range(0f, 100f) <= adjustedDropChance);

            if (shouldDrop)
            {
                Item item = itemDrop.CreateItem();
                
                if (item != null)
                {
                    int quantity = itemDrop.GetRandomQuantity();
                    droppedItems.Add(new LootDrop(item, quantity, itemDrop.rarity));
                    
                    Debug.Log($"<color=cyan>Rolled: {quantity}x {item.ItemName} ({itemDrop.rarity})</color>");
                }
                else
                {
                    Debug.LogWarning($"Failed to create item with ID: {itemDrop.itemId}");
                }
            }
        }

        return droppedItems;
    }

    public void AddItemDrop(ItemDrop itemDrop)
    {
        if (itemDrop != null)
        {
            dropTable.Add(itemDrop);
        }
    }

    public void RemoveItemDrop(ItemDrop itemDrop)
    {
        dropTable.Remove(itemDrop);
    }

    public void ClearDropTable()
    {
        dropTable.Clear();
    }

    public List<ItemDrop> GetDropsByRarity(LootRarity rarity)
    {
        return dropTable.FindAll(drop => drop.rarity == rarity);
    }

    /// <summary>
    /// Set drop rate multiplier (useful for events or boosts)
    /// </summary>
    public void SetDropRateMultiplier(float multiplier)
    {
        dropRateMultiplier = Mathf.Max(0.1f, multiplier);
    }

    /// <summary>
    /// Get summary of drop table
    /// </summary>
    public string GetDropTableSummary()
    {
        if (dropTable.Count == 0)
            return "No drops configured";

        System.Text.StringBuilder summary = new System.Text.StringBuilder();
        summary.AppendLine("=== DROP TABLE ===");

        foreach (ItemDrop drop in dropTable)
        {
            string alwaysText = drop.alwaysDrop ? " [ALWAYS]" : "";
            summary.AppendLine($"{drop.itemName} ({drop.rarity}): {drop.dropChance}% - Qty: {drop.minQuantity}-{drop.maxQuantity}{alwaysText}");
        }

        return summary.ToString();
    }
}

/// <summary>
/// Pre-configured drop table templates for common enemy types
/// </summary>
public static class LootTableTemplates
{
    /// <summary>
    /// Create a basic enemy drop table (low-level)
    /// </summary>
    public static List<ItemDrop> CreateBasicEnemyDrops()
    {
        return new List<ItemDrop>
        {
            new ItemDrop { itemId = "copper_ore", itemName = "Copper Ore", rarity = LootRarity.Common, dropChance = 40f, minQuantity = 1, maxQuantity = 3 },
            new ItemDrop { itemId = "raw_shrimp", itemName = "Raw Shrimp", rarity = LootRarity.Common, dropChance = 30f, minQuantity = 1, maxQuantity = 2 },
            new ItemDrop { itemId = "gold_coin", itemName = "Gold Coin", rarity = LootRarity.Common, dropChance = 80f, minQuantity = 5, maxQuantity = 15 },
            new ItemDrop { itemId = "health_potion", itemName = "Health Potion", rarity = LootRarity.Uncommon, dropChance = 15f, minQuantity = 1, maxQuantity = 1 }
        };
    }

    /// <summary>
    /// Create an intermediate enemy drop table (mid-level)
    /// </summary>
    public static List<ItemDrop> CreateIntermediateEnemyDrops()
    {
        return new List<ItemDrop>
        {
            new ItemDrop { itemId = "iron_ore", itemName = "Iron Ore", rarity = LootRarity.Common, dropChance = 50f, minQuantity = 2, maxQuantity = 5 },
            new ItemDrop { itemId = "raw_salmon", itemName = "Raw Salmon", rarity = LootRarity.Common, dropChance = 35f, minQuantity = 1, maxQuantity = 3 },
            new ItemDrop { itemId = "sapphire", itemName = "Sapphire", rarity = LootRarity.Uncommon, dropChance = 20f, minQuantity = 1, maxQuantity = 2 },
            new ItemDrop { itemId = "gold_coin", itemName = "Gold Coin", rarity = LootRarity.Common, dropChance = 90f, minQuantity = 25, maxQuantity = 75 },
            new ItemDrop { itemId = "health_potion", itemName = "Health Potion", rarity = LootRarity.Uncommon, dropChance = 25f, minQuantity = 1, maxQuantity = 2 },
            new ItemDrop { itemId = "emerald", itemName = "Emerald", rarity = LootRarity.Rare, dropChance = 10f, minQuantity = 1, maxQuantity = 1 }
        };
    }

    /// <summary>
    /// Create a boss enemy drop table (high-level)
    /// </summary>
    public static List<ItemDrop> CreateBossEnemyDrops()
    {
        return new List<ItemDrop>
        {
            new ItemDrop { itemId = "gold_ore", itemName = "Gold Ore", rarity = LootRarity.Uncommon, dropChance = 60f, minQuantity = 5, maxQuantity = 10 },
            new ItemDrop { itemId = "mithril_ore", itemName = "Mithril Ore", rarity = LootRarity.Rare, dropChance = 40f, minQuantity = 2, maxQuantity = 5 },
            new ItemDrop { itemId = "ruby", itemName = "Ruby", rarity = LootRarity.Rare, dropChance = 35f, minQuantity = 1, maxQuantity = 3 },
            new ItemDrop { itemId = "diamond", itemName = "Diamond", rarity = LootRarity.Epic, dropChance = 20f, minQuantity = 1, maxQuantity = 2 },
            new ItemDrop { itemId = "gold_coin", itemName = "Gold Coin", rarity = LootRarity.Common, dropChance = 100f, minQuantity = 100, maxQuantity = 500, alwaysDrop = true },
            new ItemDrop { itemId = "health_potion", itemName = "Health Potion", rarity = LootRarity.Uncommon, dropChance = 50f, minQuantity = 2, maxQuantity = 5 },
            new ItemDrop { itemId = "magic_amulet", itemName = "Magic Amulet", rarity = LootRarity.Legendary, dropChance = 5f, minQuantity = 1, maxQuantity = 1 }
        };
    }

    /// <summary>
    /// Create a rare enemy drop table (special encounters)
    /// </summary>
    public static List<ItemDrop> CreateRareEnemyDrops()
    {
        return new List<ItemDrop>
        {
            new ItemDrop { itemId = "diamond", itemName = "Diamond", rarity = LootRarity.Epic, dropChance = 50f, minQuantity = 1, maxQuantity = 3 },
            new ItemDrop { itemId = "ruby", itemName = "Ruby", rarity = LootRarity.Rare, dropChance = 70f, minQuantity = 2, maxQuantity = 5 },
            new ItemDrop { itemId = "gold_bar", itemName = "Gold Bar", rarity = LootRarity.Rare, dropChance = 45f, minQuantity = 3, maxQuantity = 8 },
            new ItemDrop { itemId = "gold_coin", itemName = "Gold Coin", rarity = LootRarity.Common, dropChance = 100f, minQuantity = 200, maxQuantity = 1000, alwaysDrop = true },
            new ItemDrop { itemId = "ancient_key", itemName = "Ancient Key", rarity = LootRarity.Legendary, dropChance = 10f, minQuantity = 1, maxQuantity = 1 }
        };
    }
}
