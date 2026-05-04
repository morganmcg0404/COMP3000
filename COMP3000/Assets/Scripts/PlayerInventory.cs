using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Header("Inventory Settings")]
    [SerializeField] private int maxSlots = 28;
    
    [Header("Inventory Slots")]
    [SerializeField] private List<InventorySlot> slots;
    public event Action<int> OnInventorySlotsChanged;
    public event Action<int, InventorySlot> OnSlotUpdated; // slotIndex, slot
    public event Action<Item, int> OnItemAdded; // item, amount
    public event Action<Item, int> OnItemRemoved; // item, amount

    public int MaxSlots => maxSlots;
    public List<InventorySlot> Slots => slots;

    void Awake()
    {
        ItemDatabase.Initialize();
        InitializeInventory();
    }

    private void InitializeInventory()
    {
        // If slots already exist, only clear the empty ones (preserve items)
        if (slots != null && slots.Count == maxSlots)
        {
            // Already initialized with correct size - only clear empty slots
            for (int i = 0; i < slots.Count; i++)
            {
                if (slots[i].IsEmpty)
                {
                    slots[i].Clear();
                }
            }
            Debug.Log($"<color=cyan>Player Inventory refreshed - preserved {GetUsedSlotCount()} slots with items</color>");
            return;
        }

        // First time initialization - create all new empty slots
        if (slots != null)
        {
            slots.Clear();
        }

        slots = new List<InventorySlot>();
        
        for (int i = 0; i < maxSlots; i++)
        {
            InventorySlot slot = new InventorySlot();
            slot.OnSlotChanged += () => OnSlotChanged(i);
            slots.Add(slot);
        }

        Debug.Log($"<color=cyan>Player Inventory initialized with {maxSlots} empty slots</color>");
    }

    public bool AddItem(Item item, int quantity = 1)
    {
        if (item == null)
        {
            Debug.LogWarning("Cannot add null item to inventory!");
            return false;
        }

        // Try to stack the item if it's stackable
        if (item.IsStackable)
        {
            // Look for an existing slot with this item
            foreach (InventorySlot slot in slots)
            {
                if (slot.Item != null && slot.Item.ItemId == item.ItemId)
                {
                    slot.AddQuantity(quantity);
                    OnItemAdded?.Invoke(item, quantity);
                    Debug.Log($"<color=green>Added {quantity}x {item.ItemName} to existing stack</color>");
                    return true;
                }
            }
        }

        // Find an empty slot for new item or unstackable
        foreach (int i in GetEmptySlots())
        {
            slots[i].SetItem(item, item.IsStackable ? quantity : 1);
            OnItemAdded?.Invoke(item, item.IsStackable ? quantity : 1);
            Debug.Log($"<color=green>Added {quantity}x {item.ItemName} to inventory</color>");
            return true;
        }

        Debug.LogWarning($"Inventory is full! Could not add {item.ItemName}");
        return false;
    }

    public bool AddItem(string itemId, int quantity = 1)
    {
        // Try ItemManager first (if in scene), then fall back to ItemDatabase
        Item item = null;

        if (ItemManager.Exists(itemId))
        {
            item = ItemManager.GetItem(itemId);
        }
        else
        {
            item = ItemDatabase.GetItem(itemId);
        }

        if (item == null)
        {
            Debug.LogError($"Cannot add item with ID '{itemId}' - item not found!");
            return false;
        }

        return AddItem(item, quantity);
    }

    public bool RemoveItem(string itemId, int quantity = 1)
    {
        int removed = 0;

        for (int i = slots.Count - 1; i >= 0; i--)
        {
            if (slots[i].Item != null && slots[i].Item.ItemId == itemId)
            {
                int canRemove = Mathf.Min(quantity - removed, slots[i].Quantity);
                slots[i].RemoveQuantity(canRemove);

                if (slots[i].Quantity <= 0)
                {
                    slots[i].Clear();
                }

                removed += canRemove;

                if (removed >= quantity)
                    break;
            }
        }

        if (removed > 0)
        {
            Debug.Log($"<color=orange>Removed {removed} items from inventory</color>");
            return true;
        }

        Debug.LogWarning($"Could not remove {itemId} - not found in inventory");
        return false;
    }

    /// <summary>
    /// Get all empty slot indices
    /// </summary>
    public List<int> GetEmptySlots()
    {
        List<int> emptySlots = new List<int>();
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].IsEmpty)
                emptySlots.Add(i);
        }
        return emptySlots;
    }

    /// <summary>
    /// Get the total count of an item
    /// </summary>
    public int GetItemCount(string itemId)
    {
        int count = 0;
        foreach (InventorySlot slot in slots)
        {
            if (slot.Item != null && slot.Item.ItemId == itemId)
                count += slot.Quantity;
        }
        return count;
    }

    /// <summary>
    /// Check if inventory has item
    /// </summary>
    public bool HasItem(string itemId, int minQuantity = 1)
    {
        return GetItemCount(itemId) >= minQuantity;
    }

    /// <summary>
    /// Get a slot by index
    /// </summary>
    public InventorySlot GetSlot(int index)
    {
        if (index >= 0 && index < slots.Count)
            return slots[index];
        return null;
    }

    /// <summary>
    /// Get number of used slots
    /// </summary>
    public int GetUsedSlotCount()
    {
        int count = 0;
        foreach (InventorySlot slot in slots)
        {
            if (!slot.IsEmpty)
                count++;
        }
        return count;
    }

    private void OnSlotChanged(int slotIndex)
    {
        OnSlotUpdated?.Invoke(slotIndex, slots[slotIndex]);
    }

    /// <summary>
    /// Save inventory to PlayerPrefs (basic serialization)
    /// </summary>
    public void SaveInventory()
    {
        // Implementation for saving inventory data
        Debug.Log("Inventory saved!");
    }

    /// <summary>
    /// Load inventory from PlayerPrefs
    /// </summary>
    public void LoadInventory()
    {
        // Implementation for loading inventory data
        Debug.Log("Inventory loaded!");
    }

#if UNITY_EDITOR
    /// <summary>
    /// Display inventory contents in Inspector (Editor only)
    /// </summary>
    [ContextMenu("View Inventory Contents")]
    public void ViewInventoryContents()
    {
        if (slots == null || slots.Count == 0)
        {
            Debug.LogWarning("Inventory is empty!");
            return;
        }

        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.AppendLine($"\n========== PLAYER INVENTORY ({GetUsedSlotCount()}/{MaxSlots}) ==========");
        
        int itemCount = 0;
        for (int i = 0; i < slots.Count; i++)
        {
            if (!slots[i].IsEmpty)
            {
                itemCount++;
                sb.AppendLine($"[Slot {i}] {slots[i].Item.ItemName} x{slots[i].Quantity}");
            }
        }

        if (itemCount == 0)
        {
            sb.AppendLine("(All slots empty)");
        }

        sb.AppendLine("==========================================\n");
        Debug.Log(sb.ToString());
    }
#endif
}
