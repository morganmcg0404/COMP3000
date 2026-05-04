using UnityEngine;

/// <summary>
/// Factory class that creates temporary test items for each equipment slot
/// Use this to quickly create items for testing the equipment system
/// </summary>
public static class TemporaryEquipmentFactory
{
    // Helmet items
    public static EquipmentItem CreateBronzeHelmet()
    {
        return new EquipmentItem(
            "Bronze Helmet",
            EquipmentSlot.Helmet,
            new EquipmentStats(accuracy: 2, defence: 5, strength: 0),
            "A basic bronze helmet offering modest protection."
        );
    }

    public static EquipmentItem CreateIronHelmet()
    {
        return new EquipmentItem(
            "Iron Helmet",
            EquipmentSlot.Helmet,
            new EquipmentStats(accuracy: 5, defence: 12, strength: 0),
            "A sturdy iron helmet with good defensive capabilities."
        );
    }

    public static EquipmentItem CreateSteelHelmet()
    {
        return new EquipmentItem(
            "Steel Helmet",
            EquipmentSlot.Helmet,
            new EquipmentStats(accuracy: 8, defence: 20, strength: 0),
            "A masterfully crafted steel helmet."
        );
    }

    // Body items
    public static EquipmentItem CreateBronzeChestplate()
    {
        return new EquipmentItem(
            "Bronze Chestplate",
            EquipmentSlot.Body,
            new EquipmentStats(accuracy: 3, defence: 10, strength: 0),
            "A basic bronze chestplate for body protection."
        );
    }

    public static EquipmentItem CreateIronChestplate()
    {
        return new EquipmentItem(
            "Iron Chestplate",
            EquipmentSlot.Body,
            new EquipmentStats(accuracy: 7, defence: 25, strength: 0),
            "A solid iron chestplate offering excellent protection."
        );
    }

    public static EquipmentItem CreateSteelChestplate()
    {
        return new EquipmentItem(
            "Steel Chestplate",
            EquipmentSlot.Body,
            new EquipmentStats(accuracy: 12, defence: 40, strength: 0),
            "Elite armor made from the finest steel."
        );
    }

    // Legs items
    public static EquipmentItem CreateBronzeLegs()
    {
        return new EquipmentItem(
            "Bronze Platelegs",
            EquipmentSlot.Legs,
            new EquipmentStats(accuracy: 2, defence: 8, strength: 0),
            "Bronze platelegs for leg protection."
        );
    }

    public static EquipmentItem CreateIronLegs()
    {
        return new EquipmentItem(
            "Iron Platelegs",
            EquipmentSlot.Legs,
            new EquipmentStats(accuracy: 5, defence: 18, strength: 0),
            "Durable iron platelegs."
        );
    }

    public static EquipmentItem CreateSteelLegs()
    {
        return new EquipmentItem(
            "Steel Platelegs",
            EquipmentSlot.Legs,
            new EquipmentStats(accuracy: 9, defence: 30, strength: 0),
            "Premium steel platelegs with superior defense."
        );
    }

    // Hands items
    public static EquipmentItem CreateLeatherGloves()
    {
        return new EquipmentItem(
            "Leather Gloves",
            EquipmentSlot.Hands,
            new EquipmentStats(accuracy: 3, defence: 2, strength: 0),
            "Simple leather gloves for better grip."
        );
    }

    public static EquipmentItem CreateChainGloves()
    {
        return new EquipmentItem(
            "Chain Gloves",
            EquipmentSlot.Hands,
            new EquipmentStats(accuracy: 6, defence: 5, strength: 0),
            "Chainmail gloves offering balance between mobility and protection."
        );
    }

    public static EquipmentItem CreatePlateGauntlets()
    {
        return new EquipmentItem(
            "Plate Gauntlets",
            EquipmentSlot.Hands,
            new EquipmentStats(accuracy: 10, defence: 10, strength: 2),
            "Heavy plate gauntlets with enhanced grip strength."
        );
    }

    // Feet items
    public static EquipmentItem CreateLeatherBoots()
    {
        return new EquipmentItem(
            "Leather Boots",
            EquipmentSlot.Feet,
            new EquipmentStats(accuracy: 1, defence: 3, strength: 0),
            "Comfortable leather boots."
        );
    }

    public static EquipmentItem CreateChainBoots()
    {
        return new EquipmentItem(
            "Chain Boots",
            EquipmentSlot.Feet,
            new EquipmentStats(accuracy: 4, defence: 7, strength: 0),
            "Chainmail boots with reinforced soles."
        );
    }

    public static EquipmentItem CreateSteelBoots()
    {
        return new EquipmentItem(
            "Steel Boots",
            EquipmentSlot.Feet,
            new EquipmentStats(accuracy: 7, defence: 15, strength: 0),
            "Heavy steel boots for maximum protection."
        );
    }

    // Cape items
    public static EquipmentItem CreateTravelersCloak()
    {
        return new EquipmentItem(
            "Traveler's Cloak",
            EquipmentSlot.Cape,
            new EquipmentStats(accuracy: 1, defence: 2, strength: 0),
            "A simple cloak for protection from the elements."
        );
    }

    public static EquipmentItem CreateWarriorsCloak()
    {
        return new EquipmentItem(
            "Warrior's Cloak",
            EquipmentSlot.Cape,
            new EquipmentStats(accuracy: 3, defence: 5, strength: 2),
            "A battle-worn cloak that inspires strength."
        );
    }

    public static EquipmentItem CreateDragonCloak()
    {
        return new EquipmentItem(
            "Dragon Cloak",
            EquipmentSlot.Cape,
            new EquipmentStats(accuracy: 8, defence: 12, strength: 5),
            "Legendary cloak said to be woven from dragon scales."
        );
    }

    // Ring items
    public static EquipmentItem CreateBronzeRing()
    {
        return new EquipmentItem(
            "Bronze Ring",
            EquipmentSlot.Ring,
            new EquipmentStats(accuracy: 2, defence: 1, strength: 1),
            "A simple bronze ring with minor enchantments."
        );
    }

    public static EquipmentItem CreateSilverRing()
    {
        return new EquipmentItem(
            "Silver Ring",
            EquipmentSlot.Ring,
            new EquipmentStats(accuracy: 5, defence: 3, strength: 3),
            "An enchanted silver ring that enhances combat prowess."
        );
    }

    public static EquipmentItem CreateGoldRing()
    {
        return new EquipmentItem(
            "Gold Ring",
            EquipmentSlot.Ring,
            new EquipmentStats(accuracy: 10, defence: 5, strength: 5),
            "A powerful gold ring radiating with magical energy."
        );
    }

    // Necklace items
    public static EquipmentItem CreateBronzeAmulet()
    {
        return new EquipmentItem(
            "Bronze Amulet",
            EquipmentSlot.Necklace,
            new EquipmentStats(accuracy: 1, defence: 2, strength: 1),
            "A basic protective amulet."
        );
    }

    public static EquipmentItem CreateSapphireAmulet()
    {
        return new EquipmentItem(
            "Sapphire Amulet",
            EquipmentSlot.Necklace,
            new EquipmentStats(accuracy: 6, defence: 8, strength: 2),
            "An amulet with an embedded sapphire that boosts defense."
        );
    }

    public static EquipmentItem CreateDiamondAmulet()
    {
        return new EquipmentItem(
            "Diamond Amulet",
            EquipmentSlot.Necklace,
            new EquipmentStats(accuracy: 12, defence: 15, strength: 8),
            "A rare diamond amulet with exceptional magical properties."
        );
    }

    // Main Hand weapons - Daggers (Fast, Low Strength)
    public static EquipmentItem CreateBronzeDagger()
    {
        return new EquipmentItem(
            "Bronze Dagger",
            EquipmentSlot.MainHand,
            new EquipmentStats(accuracy: 8, defence: 0, strength: 5, attackSpeed: 3),
            "A lightweight bronze dagger. Strikes quickly but lacks power."
        );
    }

    public static EquipmentItem CreateIronDagger()
    {
        return new EquipmentItem(
            "Iron Dagger",
            EquipmentSlot.MainHand,
            new EquipmentStats(accuracy: 15, defence: 0, strength: 10, attackSpeed: 3),
            "A sharp iron dagger perfect for quick attacks."
        );
    }

    public static EquipmentItem CreateSteelDagger()
    {
        return new EquipmentItem(
            "Steel Dagger",
            EquipmentSlot.MainHand,
            new EquipmentStats(accuracy: 25, defence: 0, strength: 18, attackSpeed: 3),
            "A masterfully crafted steel dagger with incredible speed."
        );
    }

    // Main Hand weapons - Swords (Medium Speed, Medium Strength)
    public static EquipmentItem CreateBronzeSword()
    {
        return new EquipmentItem(
            "Bronze Sword",
            EquipmentSlot.MainHand,
            new EquipmentStats(accuracy: 5, defence: 0, strength: 15, attackSpeed: 4),
            "A basic bronze sword for beginners. Balanced weapon."
        );
    }

    public static EquipmentItem CreateIronSword()
    {
        return new EquipmentItem(
            "Iron Sword",
            EquipmentSlot.MainHand,
            new EquipmentStats(accuracy: 10, defence: 0, strength: 28, attackSpeed: 4),
            "A reliable iron sword with good damage output."
        );
    }

    public static EquipmentItem CreateSteelSword()
    {
        return new EquipmentItem(
            "Steel Sword",
            EquipmentSlot.MainHand,
            new EquipmentStats(accuracy: 18, defence: 0, strength: 45, attackSpeed: 4),
            "A masterwork steel sword with deadly precision."
        );
    }

    // Main Hand weapons - Two-Handed Swords (Slow, Very High Strength)
    public static EquipmentItem CreateBronzeGreatsword()
    {
        return new EquipmentItem(
            "Bronze Greatsword",
            EquipmentSlot.MainHand,
            new EquipmentStats(accuracy: 3, defence: 0, strength: 35, attackSpeed: 6),
            "A heavy bronze greatsword. Slow but devastating."
        );
    }

    public static EquipmentItem CreateIronGreatsword()
    {
        return new EquipmentItem(
            "Iron Greatsword",
            EquipmentSlot.MainHand,
            new EquipmentStats(accuracy: 6, defence: 0, strength: 65, attackSpeed: 6),
            "A massive iron greatsword that crushes foes."
        );
    }

    public static EquipmentItem CreateSteelGreatsword()
    {
        return new EquipmentItem(
            "Steel Greatsword",
            EquipmentSlot.MainHand,
            new EquipmentStats(accuracy: 12, defence: 0, strength: 100, attackSpeed: 6),
            "An enormous steel greatsword with unparalleled power."
        );
    }

    public static EquipmentItem CreateDragonSlayer()
    {
        return new EquipmentItem(
            "Dragon Slayer",
            EquipmentSlot.MainHand,
            new EquipmentStats(accuracy: 30, defence: 5, strength: 150, attackSpeed: 6),
            "Legendary greatsword forged from dragon bones. Unmatched in combat."
        );
    }

    // Off Hand items (shields)
    public static EquipmentItem CreateWoodenShield()
    {
        return new EquipmentItem(
            "Wooden Shield",
            EquipmentSlot.OffHand,
            new EquipmentStats(accuracy: 0, defence: 8, strength: 0),
            "A basic wooden shield offering minimal protection."
        );
    }

    public static EquipmentItem CreateBronzeShield()
    {
        return new EquipmentItem(
            "Bronze Shield",
            EquipmentSlot.OffHand,
            new EquipmentStats(accuracy: 2, defence: 15, strength: 0),
            "A bronze shield with decent defensive capabilities."
        );
    }

    public static EquipmentItem CreateIronShield()
    {
        return new EquipmentItem(
            "Iron Shield",
            EquipmentSlot.OffHand,
            new EquipmentStats(accuracy: 5, defence: 28, strength: 0),
            "A sturdy iron shield that blocks most attacks."
        );
    }

    public static EquipmentItem CreateSteelShield()
    {
        return new EquipmentItem(
            "Steel Shield",
            EquipmentSlot.OffHand,
            new EquipmentStats(accuracy: 8, defence: 45, strength: 2),
            "An excellent steel shield providing superior protection."
        );
    }

    /// <summary>
    /// Creates a complete set of basic (bronze/leather tier) equipment
    /// </summary>
    public static EquipmentItem[] CreateBasicSet()
    {
        return new EquipmentItem[]
        {
            CreateBronzeHelmet(),
            CreateBronzeChestplate(),
            CreateBronzeLegs(),
            CreateLeatherGloves(),
            CreateLeatherBoots(),
            CreateTravelersCloak(),
            CreateBronzeRing(),
            CreateBronzeAmulet(),
            CreateBronzeSword(),
            CreateWoodenShield()
        };
    }

    /// <summary>
    /// Creates a complete set of intermediate (iron tier) equipment
    /// </summary>
    public static EquipmentItem[] CreateIntermediateSet()
    {
        return new EquipmentItem[]
        {
            CreateIronHelmet(),
            CreateIronChestplate(),
            CreateIronLegs(),
            CreateChainGloves(),
            CreateChainBoots(),
            CreateWarriorsCloak(),
            CreateSilverRing(),
            CreateSapphireAmulet(),
            CreateIronSword(),
            CreateIronShield()
        };
    }

    /// <summary>
    /// Creates a complete set of advanced (steel tier) equipment
    /// </summary>
    public static EquipmentItem[] CreateAdvancedSet()
    {
        return new EquipmentItem[]
        {
            CreateSteelHelmet(),
            CreateSteelChestplate(),
            CreateSteelLegs(),
            CreatePlateGauntlets(),
            CreateSteelBoots(),
            CreateDragonCloak(),
            CreateGoldRing(),
            CreateDiamondAmulet(),
            CreateSteelSword(),
            CreateSteelShield()
        };
    }

    /// <summary>
    /// Creates a dagger-focused set (fast attacks)
    /// </summary>
    public static EquipmentItem[] CreateDaggerSet()
    {
        return new EquipmentItem[]
        {
            CreateIronHelmet(),
            CreateBronzeChestplate(),
            CreateBronzeLegs(),
            CreateLeatherGloves(),
            CreateLeatherBoots(),
            CreateTravelersCloak(),
            CreateSilverRing(),
            CreateBronzeAmulet(),
            CreateSteelDagger(),
            null // No shield for dual-wielding potential
        };
    }

    /// <summary>
    /// Creates a greatsword-focused set (heavy hits)
    /// </summary>
    public static EquipmentItem[] CreateGreatswordSet()
    {
        return new EquipmentItem[]
        {
            CreateSteelHelmet(),
            CreateSteelChestplate(),
            CreateSteelLegs(),
            CreatePlateGauntlets(),
            CreateSteelBoots(),
            CreateWarriorsCloak(),
            CreateGoldRing(),
            CreateSapphireAmulet(),
            CreateSteelGreatsword(),
            null // Two-handed weapons don't use offhand
        };
    }
}
