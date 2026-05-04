using UnityEngine;

public static class ItemFactory
{

    public static Item CreateCopperOre()
    {
        return new Item("copper_ore", "Copper Ore", "Basic ore used in smithing", null, true, 999);
    }

    public static Item CreateIronOre()
    {
        return new Item("iron_ore", "Iron Ore", "Common ore for smithing", null, true, 999);
    }

    public static Item CreateGoldOre()
    {
        return new Item("gold_ore", "Gold Ore", "Valuable ore for crafting", null, true, 999);
    }

    public static Item CreateMithrilOre()
    {
        return new Item("mithril_ore", "Mithril Ore", "Rare and powerful ore", null, true, 999);
    }

    public static Item CreateAdamantiteOre()
    {
        return new Item("adamantite_ore", "Adamantite Ore", "Strongest ore in the realm", null, true, 999);
    }

    public static Item CreateTinOre()
    {
        return new Item("tin_ore", "Tin Ore", "Basic ore for alloys", null, true, 999);
    }


    public static Item CreateCopperBar()
    {
        return new Item("copper_bar", "Copper Bar", "Smelted copper ready for smithing", null, true, 999);
    }

    public static Item CreateIronBar()
    {
        return new Item("iron_bar", "Iron Bar", "Smelted iron ready for smithing", null, true, 999);
    }

    public static Item CreateGoldBar()
    {
        return new Item("gold_bar", "Gold Bar", "Pure gold bar", null, true, 999);
    }

    public static Item CreateSteelBar()
    {
        return new Item("steel_bar", "Steel Bar", "Strong alloy bar", null, true, 999);
    }


    public static Item CreateRawShrimp()
    {
        return new Item("raw_shrimp", "Raw Shrimp", "Needs to be cooked", null, true, 999);
    }

    public static Item CreateRawAnchovy()
    {
        return new Item("raw_anchovy", "Raw Anchovy", "Small fish for cooking", null, true, 999);
    }

    public static Item CreateRawHerring()
    {
        return new Item("raw_herring", "Raw Herring", "Medium fish for cooking", null, true, 999);
    }

    public static Item CreateRawSalmon()
    {
        return new Item("raw_salmon", "Raw Salmon", "Fresh fish for cooking", null, true, 999);
    }

    public static Item CreateRawTuna()
    {
        return new Item("raw_tuna", "Raw Tuna", "Large fish for cooking", null, true, 999);
    }

    public static Item CreateRawSwordfish()
    {
        return new Item("raw_swordfish", "Raw Swordfish", "Mighty fish for cooking", null, true, 999);
    }

    public static Item CreateRawShark()
    {
        return new Item("raw_shark", "Raw Shark", "Powerful fish that heals well when cooked", null, true, 999);
    }


    public static Item CreateCookedShrimp()
    {
        return new Item("cooked_shrimp", "Cooked Shrimp", "Heals 3 HP", null, true, 999);
    }

    public static Item CreateCookedSalmon()
    {
        return new Item("cooked_salmon", "Cooked Salmon", "Heals 9 HP", null, true, 999);
    }

    public static Item CreateCookedTuna()
    {
        return new Item("cooked_tuna", "Cooked Tuna", "Heals 15 HP", null, true, 999);
    }

    public static Item CreateCookedShark()
    {
        return new Item("cooked_shark", "Cooked Shark", "Heals 30 HP", null, true, 999);
    }

    // Consumables
    public static Item CreateHealthPotion()
    {
        return new Item("health_potion", "Health Potion", "Restores 20 HP", null, true, 100);
    }

    public static Item CreateManaPotion()
    {
        return new Item("mana_potion", "Mana Potion", "Restores 20 MP", null, true, 100);
    }

    public static Item CreateStaminaPotion()
    {
        return new Item("stamina_potion", "Stamina Potion", "Restores 20 Stamina", null, true, 100);
    }

    // Gems
    public static Item CreateSapphire()
    {
        return new Item("sapphire", "Sapphire", "Blue precious gem", null, true, 999);
    }

    public static Item CreateEmerald()
    {
        return new Item("emerald", "Emerald", "Green precious gem", null, true, 999);
    }

    public static Item CreateRuby()
    {
        return new Item("ruby", "Ruby", "Red precious gem", null, true, 999);
    }

    public static Item CreateDiamond()
    {
        return new Item("diamond", "Diamond", "Rare and valuable gem", null, true, 999);
    }

    // Logs (for future woodcutting)
    public static Item CreateOakLogs()
    {
        return new Item("oak_logs", "Oak Logs", "Basic wood for crafting", null, true, 999);
    }

    public static Item CreateWillowLogs()
    {
        return new Item("willow_logs", "Willow Logs", "Flexible wood", null, true, 999);
    }

    public static Item CreateMapleLogs()
    {
        return new Item("maple_logs", "Maple Logs", "Strong hardwood", null, true, 999);
    }

    public static Item CreateYewLogs()
    {
        return new Item("yew_logs", "Yew Logs", "Premium wood for bows", null, true, 999);
    }

    // Quest Items (non-stackable examples)
    public static Item CreateQuestScroll()
    {
        return new Item("quest_scroll", "Quest Scroll", "Important quest document", null, false, 1);
    }

    public static Item CreateAncientKey()
    {
        return new Item("ancient_key", "Ancient Key", "Opens mysterious doors", null, false, 1);
    }

    public static Item CreateMagicAmulet()
    {
        return new Item("magic_amulet", "Magic Amulet", "Unique magical artifact", null, false, 1);
    }

    // Currency
    public static Item CreateGoldCoin()
    {
        return new Item("gold_coin", "Gold Coin", "Currency of the realm", null, true, 999999);
    }

    public static Item CreateSilverCoin()
    {
        return new Item("silver_coin", "Silver Coin", "Minor currency", null, true, 999999);
    }

    /// <summary>
    /// Get an item by its ID
    /// </summary>
    public static Item GetItemById(string itemId)
    {
        return itemId switch
        {
            "copper_ore" => CreateCopperOre(),
            "iron_ore" => CreateIronOre(),
            "gold_ore" => CreateGoldOre(),
            "mithril_ore" => CreateMithrilOre(),
            "adamantite_ore" => CreateAdamantiteOre(),
            "tin_ore" => CreateTinOre(),
            "copper_bar" => CreateCopperBar(),
            "iron_bar" => CreateIronBar(),
            "gold_bar" => CreateGoldBar(),
            "steel_bar" => CreateSteelBar(),
            "raw_shrimp" => CreateRawShrimp(),
            "raw_anchovy" => CreateRawAnchovy(),
            "raw_herring" => CreateRawHerring(),
            "raw_salmon" => CreateRawSalmon(),
            "raw_tuna" => CreateRawTuna(),
            "raw_swordfish" => CreateRawSwordfish(),
            "raw_shark" => CreateRawShark(),
            "cooked_shrimp" => CreateCookedShrimp(),
            "cooked_salmon" => CreateCookedSalmon(),
            "cooked_tuna" => CreateCookedTuna(),
            "cooked_shark" => CreateCookedShark(),
            "health_potion" => CreateHealthPotion(),
            "mana_potion" => CreateManaPotion(),
            "stamina_potion" => CreateStaminaPotion(),
            "sapphire" => CreateSapphire(),
            "emerald" => CreateEmerald(),
            "ruby" => CreateRuby(),
            "diamond" => CreateDiamond(),
            "oak_logs" => CreateOakLogs(),
            "willow_logs" => CreateWillowLogs(),
            "maple_logs" => CreateMapleLogs(),
            "yew_logs" => CreateYewLogs(),
            "quest_scroll" => CreateQuestScroll(),
            "ancient_key" => CreateAncientKey(),
            "magic_amulet" => CreateMagicAmulet(),
            "gold_coin" => CreateGoldCoin(),
            "silver_coin" => CreateSilverCoin(),
            _ => null
        };
    }
}
