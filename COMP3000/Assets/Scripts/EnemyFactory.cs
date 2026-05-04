using UnityEngine;

public static class EnemyFactory
{
    private static void SetupMovement(Enemy enemy, int wanderRange = 5, int aggressionRange = 15, float wanderInterval = 3f)
    {
        EnemyMovement movement = enemy.GetComponent<EnemyMovement>();
        if (movement == null)
            movement = enemy.gameObject.AddComponent<EnemyMovement>();

        movement.GetType().GetField("wanderRange", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(movement, wanderRange);
        movement.GetType().GetField("aggressionRange", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(movement, aggressionRange);
        movement.GetType().GetField("wanderInterval", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(movement, wanderInterval);
    }

    public static void SetupGoblin(Enemy enemy)
    {
        enemy.GetType().GetField("enemyName", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(enemy, "Goblin");
        enemy.GetType().GetField("combatLevel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(enemy, 5);
        enemy.GetType().GetField("stats", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(enemy, new EquipmentStats(accuracy: 5, defence: 8, strength: 10, attackSpeed: 0));
        enemy.GetType().GetField("attackSpeed", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(enemy, 4);
        enemy.GetType().GetField("maxHealth", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(enemy, 50);
        enemy.GetType().GetField("currentHealth", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(enemy, 50);

        LootTable lootTable = enemy.GetComponent<LootTable>();
        if (lootTable == null)
            lootTable = enemy.gameObject.AddComponent<LootTable>();
        
        var drops = LootTableTemplates.CreateBasicEnemyDrops();
        foreach (var drop in drops)
            lootTable.AddItemDrop(drop);

        SetupMovement(enemy, wanderRange: 3, aggressionRange: 10);
    }

    public static void SetupOrcWarrior(Enemy enemy)
    {
        enemy.GetType().GetField("enemyName", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(enemy, "Orc Warrior");
        enemy.GetType().GetField("combatLevel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(enemy, 15);
        enemy.GetType().GetField("stats", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(enemy, new EquipmentStats(accuracy: 15, defence: 25, strength: 30, attackSpeed: 0));
        enemy.GetType().GetField("attackSpeed", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(enemy, 5);
        enemy.GetType().GetField("maxHealth", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(enemy, 120);
        enemy.GetType().GetField("currentHealth", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(enemy, 120);

        SetupMovement(enemy, wanderRange: 5, aggressionRange: 15);
    }

    public static void SetupShadowAssassin(Enemy enemy)
    {
        enemy.GetType().GetField("enemyName", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(enemy, "Shadow Assassin");
        enemy.GetType().GetField("combatLevel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(enemy, 20);
        enemy.GetType().GetField("stats", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(enemy, new EquipmentStats(accuracy: 30, defence: 15, strength: 25, attackSpeed: 0));
        enemy.GetType().GetField("attackSpeed", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(enemy, 3);
        enemy.GetType().GetField("maxHealth", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(enemy, 80);
        enemy.GetType().GetField("currentHealth", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(enemy, 80);

        SetupMovement(enemy, wanderRange: 7, aggressionRange: 20, wanderInterval: 2f);
    }

    public static void SetupIronGolem(Enemy enemy)
    {
        enemy.GetType().GetField("enemyName", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(enemy, "Iron Golem");
        enemy.GetType().GetField("combatLevel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(enemy, 25);
        enemy.GetType().GetField("stats", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(enemy, new EquipmentStats(accuracy: 10, defence: 50, strength: 40, attackSpeed: 0));
        enemy.GetType().GetField("attackSpeed", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(enemy, 6);
        enemy.GetType().GetField("maxHealth", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(enemy, 200);
        enemy.GetType().GetField("currentHealth", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(enemy, 200);

        SetupMovement(enemy, wanderRange: 3, aggressionRange: 10, wanderInterval: 5f);
    }

    public static void SetupDragon(Enemy enemy)
    {
        enemy.GetType().GetField("enemyName", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(enemy, "Dragon");
        enemy.GetType().GetField("combatLevel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(enemy, 50);
        enemy.GetType().GetField("stats", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(enemy, new EquipmentStats(accuracy: 40, defence: 60, strength: 80, attackSpeed: 0));
        enemy.GetType().GetField("attackSpeed", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(enemy, 5);
        enemy.GetType().GetField("maxHealth", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(enemy, 500);
        enemy.GetType().GetField("currentHealth", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(enemy, 500);

        LootTable lootTable = enemy.GetComponent<LootTable>();
        if (lootTable == null)
            lootTable = enemy.gameObject.AddComponent<LootTable>();
        
        var drops = LootTableTemplates.CreateBossEnemyDrops();
        foreach (var drop in drops)
            lootTable.AddItemDrop(drop);

        SetupMovement(enemy, wanderRange: 8, aggressionRange: 25);
    }

    public static void SetupTrainingDummy(Enemy enemy)
    {
        enemy.GetType().GetField("enemyName", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(enemy, "Training Dummy");
        enemy.GetType().GetField("combatLevel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(enemy, 1);
        enemy.GetType().GetField("stats", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(enemy, new EquipmentStats(accuracy: 0, defence: 1, strength: 0, attackSpeed: 0));
        enemy.GetType().GetField("attackSpeed", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(enemy, 999);
        enemy.GetType().GetField("maxHealth", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(enemy, 10000);
        enemy.GetType().GetField("currentHealth", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(enemy, 10000);

        SetupMovement(enemy, wanderRange: 0, aggressionRange: 0);
    }

    public static void SetupKnight(Enemy enemy)
    {
        enemy.GetType().GetField("enemyName", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(enemy, "Knight");
        enemy.GetType().GetField("combatLevel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(enemy, 30);
        enemy.GetType().GetField("stats", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(enemy, new EquipmentStats(accuracy: 25, defence: 35, strength: 35, attackSpeed: 0));
        enemy.GetType().GetField("attackSpeed", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(enemy, 4);
        enemy.GetType().GetField("maxHealth", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(enemy, 150);
        enemy.GetType().GetField("currentHealth", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(enemy, 150);

        SetupMovement(enemy, wanderRange: 5, aggressionRange: 15);
    }
}
