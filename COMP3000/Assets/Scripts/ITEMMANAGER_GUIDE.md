# ItemManager Setup Guide

## Quick Setup (5 minutes)

### Step 1: Create ItemManager GameObject
1. Right-click in Hierarchy → Create Empty
2. Rename to "ItemManager"
3. Add component: `ItemManager` script

### Step 2: Load Default Items
1. With ItemManager selected, look at Inspector
2. Click the dropdown menu (⋮) at top-right of ItemManager component
3. Select **"Load Default Items"**
4. Check the "Items" list - should now have 30+ items!

### Step 3: Use in Code
```csharp
// Add items by ID
inventory.AddItem("copper_ore", 5);
inventory.AddItem("raw_shrimp", 10);
inventory.AddItem("health_potion", 1);

// Print all items
ItemManager.PrintAllItems();
```

## Editing Items in Inspector

### View Items
- Select ItemManager in Hierarchy
- In Inspector, expand the "Items" list
- See all items with their properties

### Add New Item
1. Click the **+** button in the Items list
2. Fill in the fields:
   - **Item Id**: Unique identifier (e.g., "copper_ore")
   - **Item Name**: Display name (e.g., "Copper Ore")
   - **Description**: Item description
   - **Icon**: Drag sprite here (optional)
   - **Is Stackable**: Check if multiple can stack
   - **Max Stack Size**: Max quantity per slot (1-999999)

### Edit Existing Item
1. Expand an item in the list
2. Change any field directly in Inspector
3. Changes are immediate!

### Delete Item
1. Click the item in the list
2. Click the **-** button at bottom-right

## Item ID Examples

Use these IDs in your code:

**Ores:** copper_ore, tin_ore, iron_ore, gold_ore, mithril_ore, adamantite_ore

**Fish:** raw_shrimp, raw_anchovy, raw_herring, raw_salmon, raw_tuna, raw_swordfish

**Potions:** health_potion, mana_potion, stamina_potion

**Gems:** sapphire, emerald, ruby, diamond

**Quest Items:** quest_scroll, ancient_key, magic_amulet

**Currency:** gold_coin, silver_coin

## Adding Items to Inventory

```csharp
// Get reference
PlayerInventory inventory = GetComponent<PlayerInventory>();

// Add stackable items
inventory.AddItem("copper_ore", 5);      // 5x Copper Ore in 1 slot
inventory.AddItem("health_potion", 20);  // 20x Potion in 1 slot

// Add unstackable items (each takes separate slot)
inventory.AddItem("magic_amulet", 1);    // 1x Amulet in 1 slot
inventory.AddItem("ancient_key", 1);     // 1x Key in 1 slot

// Check what items exist
if (ItemManager.Exists("copper_ore"))
    Debug.Log("Copper Ore exists!");

// Get all items
List<ItemData> allItems = ItemManager.GetAllItems();
```

## How It Works

```
ItemManager (Inspector-editable list)
    ↓
    ├─ Builds cache for fast lookup
    └─ Returns Item objects when requested

PlayerInventory
    ├─ Receives itemId string
    ├─ Calls ItemManager.GetItem(itemId)
    └─ Adds the Item to inventory

Mining/Fishing Systems
    └─ Just call inventory.AddItem(itemId, qty)
```

## Making Changes

### Add New Item Type
1. Select ItemManager
2. In Items list, click **+**
3. Fill in all fields
4. Use the ID anywhere in code:
   ```csharp
   inventory.AddItem("my_new_item", 10);
   ```

### Change Item Properties
1. Select ItemManager
2. Expand the item in the list
3. Change Name, Description, Icon, Stackable, MaxStack
4. Changes take effect immediately

### Reset to Defaults
1. Select ItemManager
2. Click ⋮ dropdown
3. Select **"Load Default Items"**

## Tips

- **Item IDs must be unique!** Use lowercase with underscores: `copper_ore`, not `copper ore`
- **Set Is Stackable = false** for equipment/quest items (1 per slot)
- **Max Stack Size** should be high (999+) for common items, low (1) for quest items
- **Icon field** is optional - works without it
- **Description** helps players understand items in game

## Troubleshooting

**"Item not found" error:**
- Check the Item ID spelling (case-sensitive!)
- Verify ItemManager is in the scene
- Check Inspector → ItemManager → Items list

**Items not stacking:**
- Verify **Is Stackable** is checked in Inspector
- Check Item IDs match exactly

**Changes not appearing:**
- ItemManager is a singleton - changes persist
- To reset, use "Load Default Items" context menu

## Integration

ItemManager works with:
- ✓ PlayerInventory (automatic lookup)
- ✓ MiningSystem (add by ID)
- ✓ FishingSystem (add by ID)
- ✓ InventoryUI (displays items)


---

Now you can manage ALL your items from the Inspector without touching code!
