using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemData
{
    [SerializeField] public string itemId;
    [SerializeField] public string itemName;
    [SerializeField] public string description;
    [SerializeField] public Sprite icon;
    [SerializeField] public bool isStackable = true;
    [SerializeField] public int maxStackSize = 999;

    public ItemData() { }

    public ItemData(string id, string name, string desc, Sprite itemIcon, bool stackable, int maxStack)
    {
        itemId = id;
        itemName = name;
        description = desc;
        icon = itemIcon;
        isStackable = stackable;
        maxStackSize = stackable ? maxStack : 1;
    }

    public Item ToItem()
    {
        return new Item(itemId, itemName, description, icon, isStackable, maxStackSize);
    }
}

public class ItemManager : MonoBehaviour
{
    [SerializeField] private List<ItemData> items = new List<ItemData>();

    private static ItemManager instance;
    private Dictionary<string, ItemData> itemCache = new Dictionary<string, ItemData>();

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        BuildCache();
    }

    private void BuildCache()
    {
        itemCache.Clear();

        foreach (ItemData item in items)
        {
            if (string.IsNullOrEmpty(item.itemId))
            {
                Debug.LogWarning("Found item with empty ID in ItemManager!");
                continue;
            }

            if (itemCache.ContainsKey(item.itemId))
            {
                Debug.LogWarning($"Duplicate item ID in ItemManager: '{item.itemId}'");
                continue;
            }

            itemCache[item.itemId] = item;
        }

        Debug.Log($"<color=cyan>ItemManager initialized with {itemCache.Count} items</color>");
    }

    public static Item GetItem(string itemId)
    {
        if (instance == null)
        {
            Debug.LogError("ItemManager not found in scene!");
            return null;
        }

        if (!instance.itemCache.ContainsKey(itemId))
        {
            Debug.LogError($"Item '{itemId}' not found in ItemManager!");
            return null;
        }

        return instance.itemCache[itemId].ToItem();
    }

    public static bool Exists(string itemId)
    {
        if (instance == null) return false;
        return instance.itemCache.ContainsKey(itemId);
    }

    public static List<ItemData> GetAllItems()
    {
        if (instance == null) return new List<ItemData>();
        return instance.items;
    }

    public static void AddItem(ItemData itemData)
    {
        if (instance == null)
        {
            Debug.LogError("ItemManager not found!");
            return;
        }

        if (instance.itemCache.ContainsKey(itemData.itemId))
        {
            Debug.LogWarning($"Item '{itemData.itemId}' already exists!");
            return;
        }

        instance.items.Add(itemData);
        instance.itemCache[itemData.itemId] = itemData;
        Debug.Log($"<color=green>Added item: {itemData.itemName}</color>");
    }

    public static void PrintAllItems()
    {
        if (instance == null)
        {
            Debug.LogError("ItemManager not found!");
            return;
        }

        Debug.Log("=== ITEM MANAGER ===");
        foreach (var item in instance.items)
        {
            Debug.Log($"[{item.itemId}] {item.itemName} | Stackable: {item.isStackable} | MaxStack: {item.maxStackSize}");
        }
        Debug.Log($"Total: {instance.items.Count} items");
    }

#if UNITY_EDITOR
    [ContextMenu("Load Default Items")]
    public void LoadDefaultItems()
    {
        items.Clear();

        items.Add(new ItemData("copper_ore", "Copper Ore", "Basic ore used in smithing", null, true, 999));
        items.Add(new ItemData("tin_ore", "Tin Ore", "Basic ore for alloys", null, true, 999));
        items.Add(new ItemData("iron_ore", "Iron Ore", "Common ore for smithing", null, true, 999));
        items.Add(new ItemData("gold_ore", "Gold Ore", "Valuable ore for crafting", null, true, 999));
        items.Add(new ItemData("mithril_ore", "Mithril Ore", "Rare and powerful ore", null, true, 999));
        items.Add(new ItemData("adamantite_ore", "Adamantite Ore", "Strongest ore in the realm", null, true, 999));

        items.Add(new ItemData("copper_bar", "Copper Bar", "Smelted copper ready for smithing", null, true, 999));
        items.Add(new ItemData("iron_bar", "Iron Bar", "Smelted iron ready for smithing", null, true, 999));
        items.Add(new ItemData("gold_bar", "Gold Bar", "Pure gold bar", null, true, 999));
        items.Add(new ItemData("steel_bar", "Steel Bar", "Strong alloy bar", null, true, 999));

        items.Add(new ItemData("raw_shrimp", "Raw Shrimp", "Needs to be cooked", null, true, 999));
        items.Add(new ItemData("raw_anchovy", "Raw Anchovy", "Small fish for cooking", null, true, 999));
        items.Add(new ItemData("raw_herring", "Raw Herring", "Medium fish for cooking", null, true, 999));
        items.Add(new ItemData("raw_salmon", "Raw Salmon", "Fresh fish for cooking", null, true, 999));
        items.Add(new ItemData("raw_tuna", "Raw Tuna", "Large fish for cooking", null, true, 999));
        items.Add(new ItemData("raw_swordfish", "Raw Swordfish", "Mighty fish for cooking", null, true, 999));

        items.Add(new ItemData("cooked_shrimp", "Cooked Shrimp", "Heals 3 HP", null, true, 999));
        items.Add(new ItemData("cooked_salmon", "Cooked Salmon", "Heals 9 HP", null, true, 999));
        items.Add(new ItemData("cooked_tuna", "Cooked Tuna", "Heals 15 HP", null, true, 999));

        items.Add(new ItemData("health_potion", "Health Potion", "Restores 20 HP", null, true, 100));
        items.Add(new ItemData("mana_potion", "Mana Potion", "Restores 20 MP", null, true, 100));
        items.Add(new ItemData("stamina_potion", "Stamina Potion", "Restores 20 Stamina", null, true, 100));

        items.Add(new ItemData("sapphire", "Sapphire", "Blue precious gem", null, true, 999));
        items.Add(new ItemData("emerald", "Emerald", "Green precious gem", null, true, 999));
        items.Add(new ItemData("ruby", "Ruby", "Red precious gem", null, true, 999));
        items.Add(new ItemData("diamond", "Diamond", "Rare and valuable gem", null, true, 999));

        items.Add(new ItemData("oak_logs", "Oak Logs", "Basic wood for crafting", null, true, 999));
        items.Add(new ItemData("willow_logs", "Willow Logs", "Flexible wood", null, true, 999));
        items.Add(new ItemData("maple_logs", "Maple Logs", "Strong hardwood", null, true, 999));
        items.Add(new ItemData("yew_logs", "Yew Logs", "Premium wood for bows", null, true, 999));

        items.Add(new ItemData("quest_scroll", "Quest Scroll", "Important quest document", null, false, 1));
        items.Add(new ItemData("ancient_key", "Ancient Key", "Opens mysterious doors", null, false, 1));
        items.Add(new ItemData("magic_amulet", "Magic Amulet", "Unique magical artifact", null, false, 1));

        items.Add(new ItemData("gold_coin", "Gold Coin", "Currency of the realm", null, true, 999999));
        items.Add(new ItemData("silver_coin", "Silver Coin", "Minor currency", null, true, 999999));

        items.Add(new ItemData("bronze_helmet", "Bronze Helmet", "Starting helmet armor", null, false, 1));
        items.Add(new ItemData("bronze_chest", "Bronze Chest Plate", "Starting chest armor", null, false, 1));
        items.Add(new ItemData("bronze_legs", "Bronze Plate Legs", "Starting leg armor", null, false, 1));
        items.Add(new ItemData("bronze_sword", "Bronze Sword", "Starting melee weapon", null, false, 1));
        items.Add(new ItemData("bronze_shield", "Bronze Shield", "Starting defensive shield", null, false, 1));

        BuildCache();
        Debug.Log($"<color=green>Loaded {items.Count} default items</color>");
    }
#endif
}
