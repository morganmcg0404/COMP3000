using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEquipment : MonoBehaviour
{
    [Header("Equipped Items")]
    [SerializeField] private EquipmentItem helmetSlot;
    [SerializeField] private EquipmentItem bodySlot;
    [SerializeField] private EquipmentItem legsSlot;
    [SerializeField] private EquipmentItem handsSlot;
    [SerializeField] private EquipmentItem feetSlot;
    [SerializeField] private EquipmentItem capeSlot;
    [SerializeField] private EquipmentItem ringSlot;
    [SerializeField] private EquipmentItem necklaceSlot;
    [SerializeField] private EquipmentItem mainHandSlot;
    [SerializeField] private EquipmentItem offHandSlot;

    [Header("Total Stats")]
    [SerializeField] private EquipmentStats totalStats;
    public event Action<EquipmentSlot, EquipmentItem> OnEquipmentChanged;
    public event Action<EquipmentStats> OnStatsChanged;


    public EquipmentItem HelmetSlot => helmetSlot;
    public EquipmentItem BodySlot => bodySlot;
    public EquipmentItem LegsSlot => legsSlot;
    public EquipmentItem HandsSlot => handsSlot;
    public EquipmentItem FeetSlot => feetSlot;
    public EquipmentItem CapeSlot => capeSlot;
    public EquipmentItem RingSlot => ringSlot;
    public EquipmentItem NecklaceSlot => necklaceSlot;
    public EquipmentItem MainHandSlot => mainHandSlot;
    public EquipmentItem OffHandSlot => offHandSlot;

    public EquipmentStats TotalStats => totalStats;

    void Awake()
    {
        InitializeEquipment();
    }

    public void InitializeEquipment()
    {
        // Initialize equipment slots
        helmetSlot = null;
        bodySlot = null;
        legsSlot = null;
        handsSlot = null;
        feetSlot = null;
        capeSlot = null;
        ringSlot = null;
        necklaceSlot = null;
        mainHandSlot = null;
        offHandSlot = null;

        // Initialize total stats
        totalStats = new EquipmentStats();

        Debug.Log("PlayerEquipment initialized");
    }

    void Start()
    {
        // Clear all equipment slots to ensure clean state
        helmetSlot = null;
        bodySlot = null;
        legsSlot = null;
        handsSlot = null;
        feetSlot = null;
        capeSlot = null;
        ringSlot = null;
        necklaceSlot = null;
        mainHandSlot = null;
        offHandSlot = null;

        Debug.Log("Cleared all equipment slots at start");

        // Calculate initial stats from any pre-equipped items
        RecalculateTotalStats();
        Debug.Log($"After null clearing - Stats: {totalStats}");

        // If player has no equipment, equip starting gear
        bool hasValidEquipment = 
            (helmetSlot != null && helmetSlot.Stats != EquipmentStats.Zero) ||
            (bodySlot != null && bodySlot.Stats != EquipmentStats.Zero) ||
            (legsSlot != null && legsSlot.Stats != EquipmentStats.Zero) ||
            (mainHandSlot != null && mainHandSlot.Stats != EquipmentStats.Zero) ||
            (offHandSlot != null && offHandSlot.Stats != EquipmentStats.Zero);
        
        Debug.Log($"Checking equipment slots - Helmet: {(helmetSlot == null ? "Empty" : helmetSlot.Stats == EquipmentStats.Zero ? "Empty(0 stats)" : "Equipped")}, Body: {(bodySlot == null ? "Empty" : bodySlot.Stats == EquipmentStats.Zero ? "Empty(0 stats)" : "Equipped")}, Legs: {(legsSlot == null ? "Empty" : legsSlot.Stats == EquipmentStats.Zero ? "Empty(0 stats)" : "Equipped")}, MainHand: {(mainHandSlot == null ? "Empty" : mainHandSlot.Stats == EquipmentStats.Zero ? "Empty(0 stats)" : "Equipped")}, OffHand: {(offHandSlot == null ? "Empty" : offHandSlot.Stats == EquipmentStats.Zero ? "Empty(0 stats)" : "Equipped")}");
        
        if (!hasValidEquipment)
        {
            Debug.Log("No valid equipment found - equipping starting gear");
            EquipStartingGear();
        }
        else
        {
            Debug.Log("Player already has valid equipment equipped");
        }

        Debug.Log($"Player Equipment initialized. Total Stats: {totalStats}");
    }

    private bool IsSlotEmpty(EquipmentSlot slot)
    {
        return GetSlot(slot) == null;
    }

    private EquipmentItem GetSlot(EquipmentSlot slot)
    {
        return slot switch
        {
            EquipmentSlot.Helmet => helmetSlot,
            EquipmentSlot.Body => bodySlot,
            EquipmentSlot.Legs => legsSlot,
            EquipmentSlot.Hands => handsSlot,
            EquipmentSlot.Feet => feetSlot,
            EquipmentSlot.Cape => capeSlot,
            EquipmentSlot.Ring => ringSlot,
            EquipmentSlot.Necklace => necklaceSlot,
            EquipmentSlot.MainHand => mainHandSlot,
            EquipmentSlot.OffHand => offHandSlot,
            _ => null
        };
    }

    private void EquipStartingGear()
    {
        Debug.Log("=== EQUIPPING STARTING GEAR ===");

        // Create bronze starting equipment with basic stats
        EquipmentItem bronzeHelmet = new EquipmentItem(
            "Bronze Helmet",
            EquipmentSlot.Helmet,
            new EquipmentStats(accuracy: 0, defence: 5, strength: 0, attackSpeed: 0),
            "Starting helmet armor"
        );

        EquipmentItem bronzeChest = new EquipmentItem(
            "Bronze Chest Plate",
            EquipmentSlot.Body,
            new EquipmentStats(accuracy: 0, defence: 8, strength: 0, attackSpeed: 0),
            "Starting chest armor"
        );

        EquipmentItem bronzeLegs = new EquipmentItem(
            "Bronze Plate Legs",
            EquipmentSlot.Legs,
            new EquipmentStats(accuracy: 0, defence: 6, strength: 0, attackSpeed: 0),
            "Starting leg armor"
        );

        EquipmentItem bronzeSword = new EquipmentItem(
            "Bronze Sword",
            EquipmentSlot.MainHand,
            new EquipmentStats(accuracy: 5, defence: 0, strength: 8, attackSpeed: 0),
            "Starting melee weapon"
        );

        EquipmentItem bronzeShield = new EquipmentItem(
            "Bronze Shield",
            EquipmentSlot.OffHand,
            new EquipmentStats(accuracy: 0, defence: 10, strength: 0, attackSpeed: 0),
            "Starting defensive shield"
        );

        // Directly set equipment slots and recalculate stats
        Debug.Log("Setting helmet slot...");
        helmetSlot = bronzeHelmet;
        Debug.Log($"Helmet slot set to: {helmetSlot.ItemName}");

        Debug.Log("Setting body slot...");
        bodySlot = bronzeChest;
        Debug.Log($"Body slot set to: {bodySlot.ItemName}");

        Debug.Log("Setting legs slot...");
        legsSlot = bronzeLegs;
        Debug.Log($"Legs slot set to: {legsSlot.ItemName}");

        Debug.Log("Setting mainhand slot...");
        mainHandSlot = bronzeSword;
        Debug.Log($"MainHand slot set to: {mainHandSlot.ItemName}");

        Debug.Log("Setting offhand slot...");
        offHandSlot = bronzeShield;
        Debug.Log($"OffHand slot set to: {offHandSlot.ItemName}");

        // Recalculate stats after all items are equipped
        Debug.Log("Recalculating total stats...");
        RecalculateTotalStats();

        Debug.Log($"<color=green>=== STARTING GEAR EQUIPPED ===</color>");
        Debug.Log($"<color=green>Final Stats: {totalStats}</color>");
        Debug.Log($"<color=green>Helmet: {helmetSlot?.ItemName}, Chest: {bodySlot?.ItemName}, Legs: {legsSlot?.ItemName}, Sword: {mainHandSlot?.ItemName}, Shield: {offHandSlot?.ItemName}</color>");
    }

    /// <summary>
    /// Equips an item in the appropriate slot
    /// </summary>
    /// <param name="item">The item to equip</param>
    /// <returns>The previously equipped item (if any)</returns>
    public EquipmentItem EquipItem(EquipmentItem item)
    {
        if (item == null)
        {
            Debug.LogWarning("Attempted to equip null item!");
            return null;
        }

        EquipmentItem previousItem = null;

        // Get the current item in the slot and replace it
        switch (item.Slot)
        {
            case EquipmentSlot.Helmet:
                previousItem = helmetSlot;
                helmetSlot = item;
                break;
            case EquipmentSlot.Body:
                previousItem = bodySlot;
                bodySlot = item;
                break;
            case EquipmentSlot.Legs:
                previousItem = legsSlot;
                legsSlot = item;
                break;
            case EquipmentSlot.Hands:
                previousItem = handsSlot;
                handsSlot = item;
                break;
            case EquipmentSlot.Feet:
                previousItem = feetSlot;
                feetSlot = item;
                break;
            case EquipmentSlot.Cape:
                previousItem = capeSlot;
                capeSlot = item;
                break;
            case EquipmentSlot.Ring:
                previousItem = ringSlot;
                ringSlot = item;
                break;
            case EquipmentSlot.Necklace:
                previousItem = necklaceSlot;
                necklaceSlot = item;
                break;
            case EquipmentSlot.MainHand:
                previousItem = mainHandSlot;
                mainHandSlot = item;
                break;
            case EquipmentSlot.OffHand:
                previousItem = offHandSlot;
                offHandSlot = item;
                break;
        }

        // Recalculate stats and trigger events
        RecalculateTotalStats();
        OnEquipmentChanged?.Invoke(item.Slot, item);

        Debug.Log($"Equipped {item.ItemName} in {item.Slot} slot. New total stats: {totalStats}");

        return previousItem;
    }

    /// <summary>
    /// Unequips an item from a specific slot
    /// </summary>
    /// <param name="slot">The slot to unequip</param>
    /// <returns>The unequipped item (if any)</returns>
    public EquipmentItem UnequipItem(EquipmentSlot slot)
    {
        EquipmentItem unequippedItem = null;

        switch (slot)
        {
            case EquipmentSlot.Helmet:
                unequippedItem = helmetSlot;
                helmetSlot = null;
                break;
            case EquipmentSlot.Body:
                unequippedItem = bodySlot;
                bodySlot = null;
                break;
            case EquipmentSlot.Legs:
                unequippedItem = legsSlot;
                legsSlot = null;
                break;
            case EquipmentSlot.Hands:
                unequippedItem = handsSlot;
                handsSlot = null;
                break;
            case EquipmentSlot.Feet:
                unequippedItem = feetSlot;
                feetSlot = null;
                break;
            case EquipmentSlot.Cape:
                unequippedItem = capeSlot;
                capeSlot = null;
                break;
            case EquipmentSlot.Ring:
                unequippedItem = ringSlot;
                ringSlot = null;
                break;
            case EquipmentSlot.Necklace:
                unequippedItem = necklaceSlot;
                necklaceSlot = null;
                break;
            case EquipmentSlot.MainHand:
                unequippedItem = mainHandSlot;
                mainHandSlot = null;
                break;
            case EquipmentSlot.OffHand:
                unequippedItem = offHandSlot;
                offHandSlot = null;
                break;
        }

        if (unequippedItem != null)
        {
            // Recalculate stats and trigger events
            RecalculateTotalStats();
            OnEquipmentChanged?.Invoke(slot, null);

            Debug.Log($"Unequipped {unequippedItem.ItemName} from {slot} slot. New total stats: {totalStats}");
        }

        return unequippedItem;
    }

    /// <summary>
    /// Gets the item currently equipped in a specific slot
    /// </summary>
    public EquipmentItem GetEquippedItem(EquipmentSlot slot)
    {
        return slot switch
        {
            EquipmentSlot.Helmet => helmetSlot,
            EquipmentSlot.Body => bodySlot,
            EquipmentSlot.Legs => legsSlot,
            EquipmentSlot.Hands => handsSlot,
            EquipmentSlot.Feet => feetSlot,
            EquipmentSlot.Cape => capeSlot,
            EquipmentSlot.Ring => ringSlot,
            EquipmentSlot.Necklace => necklaceSlot,
            EquipmentSlot.MainHand => mainHandSlot,
            EquipmentSlot.OffHand => offHandSlot,
            _ => null
        };
    }

    /// <summary>
    /// Gets all currently equipped items
    /// </summary>
    public List<EquipmentItem> GetAllEquippedItems()
    {
        List<EquipmentItem> equippedItems = new List<EquipmentItem>();

        if (helmetSlot != null) equippedItems.Add(helmetSlot);
        if (bodySlot != null) equippedItems.Add(bodySlot);
        if (legsSlot != null) equippedItems.Add(legsSlot);
        if (handsSlot != null) equippedItems.Add(handsSlot);
        if (feetSlot != null) equippedItems.Add(feetSlot);
        if (capeSlot != null) equippedItems.Add(capeSlot);
        if (ringSlot != null) equippedItems.Add(ringSlot);
        if (necklaceSlot != null) equippedItems.Add(necklaceSlot);
        if (mainHandSlot != null) equippedItems.Add(mainHandSlot);
        if (offHandSlot != null) equippedItems.Add(offHandSlot);

        return equippedItems;
    }

    /// <summary>
    /// Checks if a specific slot has an item equipped
    /// </summary>
    public bool IsSlotEquipped(EquipmentSlot slot)
    {
        return GetEquippedItem(slot) != null;
    }

    /// <summary>
    /// Unequips all items
    /// </summary>
    public void UnequipAll()
    {
        helmetSlot = null;
        bodySlot = null;
        legsSlot = null;
        handsSlot = null;
        feetSlot = null;
        capeSlot = null;
        ringSlot = null;
        necklaceSlot = null;
        mainHandSlot = null;
        offHandSlot = null;

        RecalculateTotalStats();
        Debug.Log("Unequipped all items.");
    }

    /// <summary>
    /// Recalculates the total stats from all equipped items
    /// </summary>
    private void RecalculateTotalStats()
    {
        totalStats = EquipmentStats.Zero;

        // Add stats from each equipped item
        if (helmetSlot != null) totalStats += helmetSlot.Stats;
        if (bodySlot != null) totalStats += bodySlot.Stats;
        if (legsSlot != null) totalStats += legsSlot.Stats;
        if (handsSlot != null) totalStats += handsSlot.Stats;
        if (feetSlot != null) totalStats += feetSlot.Stats;
        if (capeSlot != null) totalStats += capeSlot.Stats;
        if (ringSlot != null) totalStats += ringSlot.Stats;
        if (necklaceSlot != null) totalStats += necklaceSlot.Stats;
        if (mainHandSlot != null) totalStats += mainHandSlot.Stats;
        if (offHandSlot != null) totalStats += offHandSlot.Stats;

        // Trigger stats changed event
        OnStatsChanged?.Invoke(totalStats);
    }

    /// <summary>
    /// Returns a formatted string of all equipped items
    /// </summary>
    public string GetEquipmentSummary()
    {
        System.Text.StringBuilder summary = new System.Text.StringBuilder();
        summary.AppendLine("=== EQUIPPED ITEMS ===");
        
        foreach (EquipmentSlot slot in Enum.GetValues(typeof(EquipmentSlot)))
        {
            EquipmentItem item = GetEquippedItem(slot);
            if (item != null)
            {
                summary.AppendLine($"{slot}: {item.ItemName} ({item.Stats})");
            }
            else
            {
                summary.AppendLine($"{slot}: Empty");
            }
        }

        summary.AppendLine($"\nTOTAL STATS: {totalStats}");
        return summary.ToString();
    }
}
