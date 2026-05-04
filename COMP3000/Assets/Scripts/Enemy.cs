using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Info")]
    [SerializeField] private string enemyName = "Enemy";
    [SerializeField] private int combatLevel = 1;

    [Header("Stats")]
    [SerializeField] private EquipmentStats stats;
    [SerializeField] private int attackSpeed = 4;
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth = 100;

    [Header("Combat State")]
    [SerializeField] private bool isInCombat = false;
    [SerializeField] private bool isDead = false;

    [Header("Loot System")]
    [SerializeField] private LootTable lootTable;

    [Header("UI")]
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private GameObject healthBarPrefab;
    private EnemyMovement enemyMovement;

    public event Action<int, int> OnHealthChanged;
    public event Action OnDeath;
    public event Action<int> OnDamageTaken;
    public string EnemyName => enemyName;
    public int CombatLevel => combatLevel;
    public EquipmentStats Stats => stats;
    public int AttackSpeed => attackSpeed;
    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;
    public bool IsInCombat => isInCombat;
    public bool IsDead => isDead;

    void Start()
    {
        currentHealth = maxHealth;
        if (healthBar == null && healthBarPrefab != null)
        {
            Debug.Log($"Spawning health bar for {enemyName}...");
            // Parent enemy health bar directly to the enemy GameObject (world space)
            GameObject healthBarObj = Instantiate(healthBarPrefab, transform);
            healthBar = healthBarObj.GetComponent<HealthBar>();
            Debug.Log($"Enemy health bar instantiated: {healthBarObj.name}");
            
            if (healthBar == null)
            {
                Debug.LogError("Health bar prefab doesn't have HealthBar component!");
            }
            else
            {
                Debug.Log($"Health bar successfully spawned for {enemyName}!");
            }
        }
        else if (healthBar == null && healthBarPrefab == null)
        {
            Debug.LogWarning($"Health bar prefab is NOT assigned for enemy {enemyName}!");
        }

        // Initialize health bar
        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth, maxHealth);
            // Hide health bar by default (only show during combat)
            healthBar.gameObject.SetActive(false);
            Debug.Log($"{enemyName} health bar initialized (hidden)");
        }

        // Get LootTable component if not assigned
        if (lootTable == null)
        {
            lootTable = GetComponent<LootTable>();
        }

        // Get or add EnemyMovement component
        enemyMovement = GetComponent<EnemyMovement>();
        if (enemyMovement == null)
        {
            enemyMovement = gameObject.AddComponent<EnemyMovement>();
            Debug.Log($"Added EnemyMovement component to {gameObject.name}");
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);

        Debug.Log($"{enemyName} took {damage} damage! Health: {currentHealth}/{maxHealth}");
        
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        OnDamageTaken?.Invoke(damage);
        
        // Update health bar
        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth, maxHealth);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        if (isDead) return;

        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);

        Debug.Log($"{enemyName} healed by {amount}! Health: {currentHealth}/{maxHealth}");

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        
        // Update health bar
        if (healthBar != null)
        {
            Debug.Log($"Updating health bar for {enemyName} during heal");
            healthBar.SetHealth(currentHealth, maxHealth);
        }
        else
        {
            Debug.LogWarning($"Health bar is NULL for {enemyName}!");
        }
    }

    public void SetInCombat(bool inCombat)
    {
        bool wasInCombat = isInCombat;
        isInCombat = inCombat;

        // Notify movement component
        if (enemyMovement != null)
        {
            if (inCombat && !wasInCombat)
                enemyMovement.OnCombatStarted();
            else if (!inCombat && wasInCombat)
                enemyMovement.OnCombatEnded();
        }

        // Show health bar during combat, hide when not in combat
        if (healthBar != null)
        {
            healthBar.gameObject.SetActive(inCombat);
        }
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;
        isInCombat = false;

        Debug.Log($"{enemyName} has been defeated!");
        
        // Drop loot
        DropLoot();
        
        OnDeath?.Invoke();

        // Optional: Add death animation
        // For now, just disable the enemy after a short delay
        Invoke(nameof(DisableEnemy), 2f);
    }

    private void DropLoot()
    {
        if (lootTable == null)
        {
            Debug.Log($"{enemyName} has no loot table configured");
            return;
        }

        // Roll for loot drops
        var drops = lootTable.RollLoot();

        if (drops.Count == 0)
        {
            Debug.Log($"{enemyName} dropped nothing");
            return;
        }

        Debug.Log($"<color=yellow>{enemyName} dropped {drops.Count} item(s):</color>");

        // Try to add items to player inventory
        PlayerInventory playerInventory = FindFirstObjectByType<PlayerInventory>();

        foreach (var lootDrop in drops)
        {
            string rarityColor = GetRarityColor(lootDrop.Rarity);
            Debug.Log($"<color={rarityColor}>  • {lootDrop.Quantity}x {lootDrop.Item.ItemName} ({lootDrop.Rarity})</color>");

            // Auto-pickup to player inventory if found
            if (playerInventory != null)
            {
                bool added = playerInventory.AddItem(lootDrop.Item, lootDrop.Quantity);
                if (added)
                {
                    Debug.Log($"<color=green>    Added to inventory!</color>");
                }
                else
                {
                    Debug.Log($"<color=red>    Inventory full! Items lost.</color>");
                }
            }
        }
    }

    private string GetRarityColor(LootRarity rarity)
    {
        return rarity switch
        {
            LootRarity.Common => "white",
            LootRarity.Uncommon => "lime",
            LootRarity.Rare => "cyan",
            LootRarity.Epic => "magenta",
            LootRarity.Legendary => "orange",
            _ => "white"
        };
    }

    private void DisableEnemy()
    {
        gameObject.SetActive(false);
    }

    public void Reset()
    {
        currentHealth = maxHealth;
        isDead = false;
        isInCombat = false;
        gameObject.SetActive(true);

        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        // Update health bar
        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth, maxHealth);
            // Hide health bar when resetting (not in combat)
            healthBar.gameObject.SetActive(false);
        }

        // Notify movement to restart wandering
        if (enemyMovement != null)
        {
            enemyMovement.StartWandering();
        }
    }

    public int GetTotalAccuracy()
    {
        return combatLevel + stats.Accuracy;
    }

    public int GetTotalDefence()
    {
        return combatLevel + stats.Defence;
    }

    public int GetAttackPower()
    {
        return combatLevel + stats.Strength;
    }

    void OnDrawGizmos()
    {
        if (isInCombat && !isDead)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 0.5f);
        }
    }
}
