using System;
using UnityEngine;

[System.Serializable]
public class InventorySlot
{
    [SerializeField] private Item item;
    [SerializeField] private int quantity;

    public event Action OnSlotChanged;

    public Item Item => item;
    public int Quantity => quantity;
    public bool IsEmpty => item == null || quantity <= 0;
    public bool IsFull => item != null && quantity >= item.MaxStackSize;

    public InventorySlot()
    {
        item = null;
        quantity = 0;
    }

    public void SetItem(Item newItem, int newQuantity = 1)
    {
        if (newItem == null)
        {
            Clear();
            return;
        }

        item = newItem;
        quantity = Mathf.Max(1, newQuantity);

        // Enforce max stack size
        if (item.MaxStackSize > 0)
        {
            quantity = Mathf.Min(quantity, item.MaxStackSize);
        }

        OnSlotChanged?.Invoke();
    }

    public int AddQuantity(int amount)
    {
        if (item == null || amount <= 0)
            return amount;

        // If item is unstackable, can't add more
        if (!item.IsStackable)
        {
            return amount;
        }

        int maxStack = item.MaxStackSize > 0 ? item.MaxStackSize : 999;
        int canAdd = Mathf.Min(amount, maxStack - quantity);
        quantity += canAdd;

        OnSlotChanged?.Invoke();
        return amount - canAdd; // Return overflow amount
    }

    public int AddItem(Item newItem, int amount)
    {
        if (newItem == null || amount <= 0)
            return amount;

        // If slot is empty, just add the item
        if (IsEmpty)
        {
            item = newItem;
            int amountToAdd = Mathf.Min(amount, item.MaxStackSize);
            quantity = amountToAdd;
            OnSlotChanged?.Invoke();
            return amount - amountToAdd;
        }

        // If items don't match, can't add
        if (!item.CanStackWith(newItem))
            return amount;

        // Add to existing stack
        int spaceLeft = item.MaxStackSize - quantity;
        int amountAdded = Mathf.Min(amount, spaceLeft);
        quantity += amountAdded;
        OnSlotChanged?.Invoke();

        return amount - amountAdded; // Return overflow
    }

    /// <summary>
    /// Remove a specific amount from this slot
    /// </summary>
    /// <returns>Amount actually removed</returns>
    public int RemoveItem(int amount)
    {
        if (IsEmpty)
            return 0;

        int amountToRemove = Mathf.Min(amount, quantity);
        quantity -= amountToRemove;

        if (quantity <= 0)
        {
            Clear();
        }

        OnSlotChanged?.Invoke();
        return amountToRemove;
    }

    /// <summary>
    /// Remove quantity from this slot
    /// </summary>
    public void RemoveQuantity(int amount)
    {
        if (amount <= 0)
            return;

        quantity -= amount;
        if (quantity <= 0)
        {
            Clear();
        }

        OnSlotChanged?.Invoke();
    }

    /// <summary>
    /// Clear the slot completely
    /// </summary>
    public void Clear()
    {
        item = null;
        quantity = 0;
        OnSlotChanged?.Invoke();
    }

    /// <summary>
    /// Set the slot to a specific item and quantity (legacy support)
    /// </summary>
    public void SetSlot(Item newItem, int newQuantity)
    {
        item = newItem;
        quantity = newQuantity;
        OnSlotChanged?.Invoke();
    }

    /// <summary>
    /// Check if this slot can accept the given item
    /// </summary>
    public bool CanAccept(Item newItem)
    {
        if (newItem == null)
            return false;

        if (IsEmpty)
            return true;

        return item.CanStackWith(newItem) && !IsFull;
    }

    /// <summary>
    /// Get how much space is available in this slot for the given item
    /// </summary>
    public int GetAvailableSpace(Item checkItem)
    {
        if (checkItem == null)
            return 0;

        if (IsEmpty)
            return checkItem.MaxStackSize;

        if (!item.CanStackWith(checkItem))
            return 0;

        return item.MaxStackSize - quantity;
    }

    /// <summary>
    /// Move items from another slot to this one
    /// </summary>
    public bool MoveFrom(InventorySlot sourceSlot, int amount)
    {
        if (sourceSlot == null || sourceSlot.IsEmpty || amount <= 0)
            return false;

        // Can only move to empty slots or slots with same stackable item
        if (!IsEmpty && (item.ItemId != sourceSlot.Item.ItemId || !item.IsStackable))
        {
            return false;
        }

        if (IsEmpty)
        {
            SetItem(sourceSlot.Item, amount);
            sourceSlot.RemoveQuantity(amount);
            return true;
        }
        else if (item.ItemId == sourceSlot.Item.ItemId && item.IsStackable)
        {
            int overflow = AddQuantity(amount);
            sourceSlot.RemoveQuantity(amount - overflow);
            return overflow == 0;
        }

        return false;
    }

    public override string ToString()
    {
        if (IsEmpty)
            return "Empty Slot";

        return $"{item.ItemName} x{quantity}";
    }
}

