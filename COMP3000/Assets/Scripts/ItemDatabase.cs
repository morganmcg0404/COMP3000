using System.Collections.Generic;
using UnityEngine;

public static class ItemDatabase
{
    private static Dictionary<string, Item> itemDatabase = new Dictionary<string, Item>();
    private static bool initialized = false;

    public static void Initialize()
    {
        if (initialized)
            return;

        itemDatabase.Clear();

        RegisterItem(ItemFactory.CreateCopperOre());
        RegisterItem(ItemFactory.CreateTinOre());
        RegisterItem(ItemFactory.CreateIronOre());
        RegisterItem(ItemFactory.CreateGoldOre());
        RegisterItem(ItemFactory.CreateMithrilOre());
        RegisterItem(ItemFactory.CreateAdamantiteOre());

        RegisterItem(ItemFactory.CreateCopperBar());
        RegisterItem(ItemFactory.CreateIronBar());
        RegisterItem(ItemFactory.CreateGoldBar());
        RegisterItem(ItemFactory.CreateSteelBar());

        RegisterItem(ItemFactory.CreateRawShrimp());
        RegisterItem(ItemFactory.CreateRawAnchovy());
        RegisterItem(ItemFactory.CreateRawHerring());
        RegisterItem(ItemFactory.CreateRawSalmon());
        RegisterItem(ItemFactory.CreateRawTuna());
        RegisterItem(ItemFactory.CreateRawSwordfish());

        RegisterItem(ItemFactory.CreateCookedShrimp());
        RegisterItem(ItemFactory.CreateCookedSalmon());
        RegisterItem(ItemFactory.CreateCookedTuna());
        RegisterItem(ItemFactory.CreateCookedShark());

        RegisterItem(ItemFactory.CreateHealthPotion());
        RegisterItem(ItemFactory.CreateManaPotion());

        initialized = true;
        Debug.Log($"<color=cyan>ItemDatabase initialized with {itemDatabase.Count} items</color>");
    }

    private static void RegisterItem(Item item)
    {
        if (item == null)
        {
            Debug.LogError("Cannot register null item!");
            return;
        }

        if (itemDatabase.ContainsKey(item.ItemId))
        {
            Debug.LogWarning($"Item '{item.ItemId}' already registered. Overwriting...");
        }

        itemDatabase[item.ItemId] = item.Clone();
    }

    public static Item GetItem(string itemId)
    {
        if (!initialized)
            Initialize();

        if (string.IsNullOrEmpty(itemId))
        {
            Debug.LogError("Item ID cannot be null or empty!");
            return null;
        }

        if (itemDatabase.ContainsKey(itemId))
        {
            return itemDatabase[itemId].Clone();
        }

        Debug.LogError($"Item '{itemId}' not found in database!");
        return null;
    }

    public static bool Exists(string itemId)
    {
        if (!initialized)
            Initialize();

        return itemDatabase.ContainsKey(itemId);
    }

    public static List<string> GetAllItemIds()
    {
        if (!initialized)
            Initialize();

        return new List<string>(itemDatabase.Keys);
    }

    public static void PrintAllItems()
    {
        if (!initialized)
            Initialize();

        Debug.Log("=== ITEM DATABASE ===");
        foreach (var kvp in itemDatabase)
        {
            var item = kvp.Value;
            Debug.Log($"ID: {item.ItemId} | Name: {item.ItemName} | Stackable: {item.IsStackable} | MaxStack: {item.MaxStackSize}");
        }
        Debug.Log($"Total Items: {itemDatabase.Count}");
    }
}
