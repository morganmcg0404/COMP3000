using System.Collections;
using UnityEngine;

public class CombatSystem : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SkillSystem skillSystem;
    [SerializeField] private PlayerEquipment playerEquipment;
    [SerializeField] private GridManager gridManager;
    [SerializeField] private HealthBar playerHealthBar;
    [SerializeField] private GameObject playerHealthBarPrefab;
    [SerializeField] private GameObject hitSplatPrefab;

    [Header("Combat Settings")]
    [SerializeField] private float tickDuration = 0.6f; // 1 tick = 0.6 seconds
    [SerializeField] private int playerMaxHealth = 100;
    [SerializeField] private int playerCurrentHealth = 100;
    [SerializeField] private int combatRange = 1;

    [Header("Combat State")]
    [SerializeField] private bool isInCombat = false;
    [SerializeField] private Enemy currentTarget;
    [SerializeField] private int playerTicksUntilNextAttack = 0;
    [SerializeField] private int enemyTicksUntilNextAttack = 0;
    [SerializeField] private int playerHealthRegenTicks = 0;

    public System.Action<int, int> OnPlayerHealthChanged;
    public System.Action OnPlayerDeath;
    public System.Action<Enemy> OnCombatStarted;
    public System.Action OnCombatEnded;
    public bool IsInCombat => isInCombat;
    public Enemy CurrentTarget => currentTarget;
    public int PlayerCurrentHealth => playerCurrentHealth;
    public int PlayerMaxHealth => playerMaxHealth;

    private Coroutine combatCoroutine;
    private PlayerController playerController;

    void Awake()
    {
        InitializeCombat();
    }

    public void InitializeCombat()
    {
        // Find references
        if (skillSystem == null)
            skillSystem = FindFirstObjectByType<SkillSystem>();

        if (playerEquipment == null)
            playerEquipment = FindFirstObjectByType<PlayerEquipment>();

        if (gridManager == null)
            gridManager = FindFirstObjectByType<GridManager>();

        if (playerController == null)
            playerController = FindFirstObjectByType<PlayerController>();

        // Initialize combat state
        playerCurrentHealth = playerMaxHealth;
        isInCombat = false;
        currentTarget = null;
        playerTicksUntilNextAttack = 0;
        enemyTicksUntilNextAttack = 0;
        playerHealthRegenTicks = 0;

        Debug.Log("CombatSystem initialized");
    }

    // Public method to update GridManager reference when scenes change
    public void UpdateGridManager()
    {
        gridManager = FindFirstObjectByType<GridManager>();
        Debug.Log($"CombatSystem GridManager updated: {gridManager != null}");
    }

    void Start()
    {
            playerController = FindFirstObjectByType<PlayerController>();

        // Calculate max health based on health skill level
        if (skillSystem != null && skillSystem.HealthSkill != null)
        {
            int healthLevel = skillSystem.HealthSkill.Level;
            playerMaxHealth = healthLevel;
            Debug.Log($"Player Max Health: {playerMaxHealth} (Health skill level)");
        }
        else
        {
            Debug.LogWarning("Could not calculate max health - SkillSystem or HealthSkill not found!");
        }

        // Set current health to max
        playerCurrentHealth = playerMaxHealth;

        Debug.Log($"CombatSystem Start - Health Bar Prefab assigned: {playerHealthBarPrefab != null}");
        Debug.Log($"CombatSystem Start - Health Bar assigned: {playerHealthBar != null}");

        // Spawn player health bar if prefab is assigned and health bar not already set
        if (playerHealthBar == null && playerHealthBarPrefab != null)
        {
            Debug.Log("Spawning player health bar from prefab...");
            
            // Find the player GameObject (which persists across scenes)
            PlayerController playerController = FindFirstObjectByType<PlayerController>();
            GameObject playerObject = null;
            
            if (playerController != null)
            {
                playerObject = playerController.gameObject;
                Debug.Log($"Found player object: {playerObject.name}");
            }
            else
            {
                Debug.LogError("PlayerController not found! Cannot create persistent health bar canvas.");
            }
            
            if (playerObject != null)
            {
                // Find or create a persistent canvas as child of player
                Transform healthBarCanvasTransform = playerObject.transform.Find("HealthBarCanvas");
                Canvas healthBarCanvas = null;
                
                if (healthBarCanvasTransform != null)
                {
                    healthBarCanvas = healthBarCanvasTransform.GetComponent<Canvas>();
                    Debug.Log("Found existing HealthBarCanvas on player");
                }
                else
                {
                    // Create new canvas as child of player
                    GameObject canvasObj = new GameObject("HealthBarCanvas");
                    canvasObj.transform.SetParent(playerObject.transform);
                    canvasObj.transform.localPosition = Vector3.zero;
                    canvasObj.transform.localRotation = Quaternion.identity;
                    canvasObj.transform.localScale = Vector3.one;
                    
                    healthBarCanvas = canvasObj.AddComponent<Canvas>();
                    healthBarCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                    
                    // Configure CanvasScaler to scale with screen size
                    UnityEngine.UI.CanvasScaler canvasScaler = canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
                    canvasScaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
                    canvasScaler.referenceResolution = new Vector2(800, 600); // Reference resolution for scaling
                    canvasScaler.screenMatchMode = UnityEngine.UI.CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                    canvasScaler.matchWidthOrHeight = 0f; // Match width primarily
                    
                    canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();
                    
                    // Mark as DontDestroyOnLoad since it's a child of the persistent player
                    DontDestroyOnLoad(canvasObj);
                    Debug.Log("Created new HealthBarCanvas on player with screen size scaling and marked as DontDestroyOnLoad");
                }
                
                if (healthBarCanvas != null)
                {
                    GameObject healthBarObj = Instantiate(playerHealthBarPrefab, healthBarCanvas.transform);
                    playerHealthBar = healthBarObj.GetComponent<HealthBar>();
                    Debug.Log($"Player health bar instantiated under persistent canvas: {healthBarObj.name}");
                    
                    if (playerHealthBar == null)
                    {
                        Debug.LogError("Player health bar prefab doesn't have HealthBar component!");
                    }
                    else
                    {
                        Debug.Log("Player health bar successfully spawned under persistent canvas!");
                    }
                }
            }
        }
        else if (playerHealthBar == null && playerHealthBarPrefab == null)
        {
            Debug.LogWarning("Player health bar prefab is NOT assigned in CombatSystem!");
        }

        // Initialize player health bar
        if (playerHealthBar != null)
        {
            playerHealthBar.SetHealth(playerCurrentHealth, playerMaxHealth);
            Debug.Log("Player health bar initialized");
        }

        if (skillSystem == null)
            Debug.LogWarning("CombatSystem: SkillSystem not found!");
        
        if (playerEquipment == null)
            Debug.LogWarning("CombatSystem: PlayerEquipment not found!");

        if (gridManager == null)
            Debug.LogWarning("CombatSystem: GridManager not found!");

        if (playerController == null)
            Debug.LogWarning("CombatSystem: PlayerController not found!");
    }

    void Update()
    {
        // Out-of-combat health regen
        if (!isInCombat)
        {
            playerHealthRegenTicks++;
            if (playerHealthRegenTicks >= 10)
            {
                HealPlayer(1);
                playerHealthRegenTicks = 0;
            }
        }
    }

    public void StartCombat(Enemy enemy)
    {
        if (enemy == null || enemy.IsDead)
        {
            Debug.LogWarning("Cannot start combat with null or dead enemy!");
            return;
        }

        // If already in combat with this enemy, do nothing
        if (isInCombat && currentTarget == enemy)
        {
            Debug.Log("Already in combat with this enemy!");
            return;
        }

        // End previous combat if any
        if (isInCombat)
        {
            EndCombat();
        }

        Debug.Log($"<color=yellow>Starting combat with {enemy.EnemyName}!</color>");

        isInCombat = true;
        currentTarget = enemy;
        currentTarget.SetInCombat(true);

        // Initialize attack timers
        playerTicksUntilNextAttack = GetPlayerAttackSpeed();
        enemyTicksUntilNextAttack = enemy.AttackSpeed;

        // Subscribe to enemy death
        currentTarget.OnDeath += OnEnemyDied;

        // Start combat loop
        if (combatCoroutine != null)
            StopCoroutine(combatCoroutine);
        
        combatCoroutine = StartCoroutine(CombatLoop());

        OnCombatStarted?.Invoke(enemy);
    }

    private IEnumerator CombatLoop()
    {
        while (isInCombat && currentTarget != null && !currentTarget.IsDead)
        {
            yield return new WaitForSeconds(tickDuration);

            // Double-check that target is still valid
            if (currentTarget == null || currentTarget.IsDead)
            {
                break;
            }

            // Decrement attack timers
            playerTicksUntilNextAttack--;
            enemyTicksUntilNextAttack--;

            // Handle player health regen
            playerHealthRegenTicks++;
            if (playerHealthRegenTicks >= 10)
            {
                HealPlayer(1);
                playerHealthRegenTicks = 0;
            }

            // Player attack
            if (playerTicksUntilNextAttack <= 0)
            {
                PerformPlayerAttack();
                playerTicksUntilNextAttack = GetPlayerAttackSpeed();
            }

            // Enemy attack
            if (enemyTicksUntilNextAttack <= 0 && currentTarget != null && !currentTarget.IsDead)
            {
                PerformEnemyAttack();
                if (currentTarget != null)
                {
                    enemyTicksUntilNextAttack = currentTarget.AttackSpeed;
                }
            }

            // Check if player died
            if (playerCurrentHealth <= 0)
            {
                OnPlayerDied();
                break;
            }
        }
    }

    private void PerformPlayerAttack()
    {
        if (currentTarget == null || currentTarget.IsDead)
        {
            EndCombat();
            return;
        }

        // Check if player is in range
        if (playerController != null && gridManager != null)
        {
            Vector3Int playerPos = playerController.CurrentGridPosition;
            // Remove the 0.5 Y offset from enemy position before calculating grid position
            Vector3 enemyPosNoOffset = currentTarget.transform.position;
            enemyPosNoOffset.y -= 0.5f;
            Vector3Int enemyPos = gridManager.WorldToGrid(enemyPosNoOffset);
            
            if (!IsInCombatRange(playerPos, enemyPos))
            {
                Debug.Log($"<color=yellow>Player out of range! Cannot attack {currentTarget.EnemyName}!</color>");
                return;
            }
        }

        // Calculate hit chance
        int playerAccuracy = GetPlayerAccuracy();
        int enemyDefence = currentTarget.GetTotalDefence();

        bool didHit = CalculateHitChance(playerAccuracy, enemyDefence);

        if (didHit)
        {
            // Calculate damage
            int damage = CalculateDamage(GetPlayerStrength());
            
            Debug.Log($"<color=green>Player hit {currentTarget.EnemyName} for {damage} damage!</color>");
            
            currentTarget.TakeDamage(damage);

            // Spawn hit splat
            SpawnHitSplat(damage, currentTarget.transform.position, currentTarget.transform);

            // Grant experience
            if (skillSystem != null)
            {
                skillSystem.OnEnemyHit(damage);
            }
        }
        else
        {
            Debug.Log($"<color=yellow>Player missed {currentTarget.EnemyName}!</color>");
            
            // Spawn miss hit splat (0 damage)
            SpawnHitSplat(0, currentTarget.transform.position, currentTarget.transform);
        }
    }

    /// <summary>
    /// Perform an enemy attack on the player
    /// </summary>
    private void PerformEnemyAttack()
    {
        if (currentTarget == null || currentTarget.IsDead)
            return;

        // Check if enemy is in range
        if (playerController != null && gridManager != null)
        {
            Vector3Int playerPos = playerController.CurrentGridPosition;
            // Remove the 0.5 Y offset from enemy position before calculating grid position
            Vector3 enemyPosNoOffset = currentTarget.transform.position;
            enemyPosNoOffset.y -= 0.5f;
            Vector3Int enemyPos = gridManager.WorldToGrid(enemyPosNoOffset);
            
            if (!IsInCombatRange(enemyPos, playerPos))
            {
                Debug.Log($"<color=yellow>{currentTarget.EnemyName} out of range! Cannot attack player!</color>");
                return;
            }
        }

        // Calculate hit chance
        int enemyAccuracy = currentTarget.GetTotalAccuracy();
        int playerDefence = GetPlayerDefence();

        bool didHit = CalculateHitChance(enemyAccuracy, playerDefence);

        if (didHit)
        {
            // Calculate damage
            int damage = CalculateDamage(currentTarget.GetAttackPower());
            
            Debug.Log($"<color=red>{currentTarget.EnemyName} hit player for {damage} damage!</color>");
            
            TakePlayerDamage(damage);

            // Spawn hit splat on player
            if (playerController != null)
            {
                SpawnHitSplat(damage, playerController.transform.position, playerController.transform);
            }
        }
        else
        {
            Debug.Log($"<color=yellow>{currentTarget.EnemyName} missed player!</color>");
            
            // Spawn miss hit splat (0 damage) on player
            if (playerController != null)
            {
                SpawnHitSplat(0, playerController.transform.position, playerController.transform);
            }
        }
    }

    /// <summary>
    /// Calculate if an attack hits based on accuracy vs defence
    /// Formula: If (attackerAccuracy >= targetDefence) then 100% hit, else roll chance
    /// </summary>
    private bool CalculateHitChance(int attackerAccuracy, int targetDefence)
    {
        // If accuracy >= defence, guaranteed hit
        if (attackerAccuracy >= targetDefence)
        {
            return true;
        }

        // Otherwise, calculate hit chance based on difference
        // Formula: (accuracy / (accuracy + defence)) * 100
        float hitChance = (float)attackerAccuracy / (attackerAccuracy + targetDefence);
        float roll = Random.Range(0f, 1f);

        return roll <= hitChance;
    }

    /// <summary>
    /// Calculate damage based on strength stat
    /// Formula: Random range based on strength with some variance
    /// </summary>
    private int CalculateDamage(int strength)
    {
        // Base damage formula: 0 to (strength / 10) + (strength / 5)
        // This gives a reasonable damage range that scales with strength
        int maxHit = (strength / 10) + (strength / 5);
        maxHit = Mathf.Max(1, maxHit); // Minimum 1 damage

        int damage = Random.Range(1, maxHit + 1);
        return damage;
    }

    /// <summary>
    /// Get player's total accuracy (Combat Level + Equipment Accuracy)
    /// </summary>
    private int GetPlayerAccuracy()
    {
        int combatLevel = skillSystem != null ? skillSystem.CombatSkill.Level : 1;
        int equipmentAccuracy = playerEquipment != null ? playerEquipment.TotalStats.Accuracy : 0;
        
        return combatLevel + equipmentAccuracy;
    }

    /// <summary>
    /// Get player's total defence (Health Level + Equipment Defence)
    /// </summary>
    private int GetPlayerDefence()
    {
        int healthLevel = skillSystem != null ? skillSystem.HealthSkill.Level : 1;
        int equipmentDefence = playerEquipment != null ? playerEquipment.TotalStats.Defence : 0;
        
        return healthLevel + equipmentDefence;
    }

    /// <summary>
    /// Get player's total strength (Combat Level + Equipment Strength)
    /// </summary>
    private int GetPlayerStrength()
    {
        int combatLevel = skillSystem != null ? skillSystem.CombatSkill.Level : 1;
        int equipmentStrength = playerEquipment != null ? playerEquipment.TotalStats.Strength : 0;
        
        return combatLevel + equipmentStrength;
    }

    /// <summary>
    /// Get player's attack speed from equipped weapon
    /// </summary>
    private int GetPlayerAttackSpeed()
    {
        int attackSpeed = playerEquipment != null ? playerEquipment.TotalStats.AttackSpeed : 4;
        
        // If no weapon equipped or attack speed is 0, default to 4 (unarmed)
        return attackSpeed > 0 ? attackSpeed : 4;
    }

    /// <summary>
    /// Player takes damage
    /// </summary>
    private void TakePlayerDamage(int damage)
    {
        playerCurrentHealth -= damage;
        playerCurrentHealth = Mathf.Max(0, playerCurrentHealth);

        OnPlayerHealthChanged?.Invoke(playerCurrentHealth, playerMaxHealth);
        
        // Update health bar
        if (playerHealthBar != null)
        {
            playerHealthBar.SetHealth(playerCurrentHealth, playerMaxHealth);
        }

        if (playerCurrentHealth <= 0)
        {
            OnPlayerDied();
        }
    }

    /// <summary>
    /// Player heals
    /// </summary>
    private void HealPlayer(int amount)
    {
        if (playerCurrentHealth >= playerMaxHealth)
            return;

        playerCurrentHealth += amount;
        playerCurrentHealth = Mathf.Min(playerCurrentHealth, playerMaxHealth);

        Debug.Log($"Player healed by {amount}! Health: {playerCurrentHealth}/{playerMaxHealth}");

        OnPlayerHealthChanged?.Invoke(playerCurrentHealth, playerMaxHealth);
        
        // Update health bar
        if (playerHealthBar != null)
        {
            playerHealthBar.SetHealth(playerCurrentHealth, playerMaxHealth);
        }
    }

    /// <summary>
    /// Called when the enemy dies
    /// </summary>
    private void OnEnemyDied()
    {
        Debug.Log($"<color=green>{currentTarget.EnemyName} has been defeated!</color>");
        EndCombat();
    }

    /// <summary>
    /// Called when the player dies
    /// </summary>
    private void OnPlayerDied()
    {
        Debug.Log("<color=red>Player has been defeated!</color>");
        EndCombat();
        OnPlayerDeath?.Invoke();

        // Respawn logic could go here
        Invoke(nameof(RespawnPlayer), 3f);
    }

    /// <summary>
    /// Respawn the player
    /// </summary>
    private void RespawnPlayer()
    {
        playerCurrentHealth = playerMaxHealth;
        OnPlayerHealthChanged?.Invoke(playerCurrentHealth, playerMaxHealth);
        Debug.Log("Player respawned!");
    }

    /// <summary>
    /// End combat
    /// </summary>
    public void EndCombat()
    {
        if (!isInCombat)
            return;

        Debug.Log("Combat ended.");

        isInCombat = false;

        if (currentTarget != null)
        {
            currentTarget.SetInCombat(false);
            currentTarget.OnDeath -= OnEnemyDied;
            currentTarget = null;
        }

        if (combatCoroutine != null)
        {
            StopCoroutine(combatCoroutine);
            combatCoroutine = null;
        }

        OnCombatEnded?.Invoke();
    }

    /// <summary>
    /// Check if attacker is in range to attack target
    /// </summary>
    private bool IsInCombatRange(Vector3Int attacker, Vector3Int defender)
    {
        int distance = GetManhattanDistance(attacker, defender);
        return distance <= combatRange;
    }

    /// <summary>
    /// Calculate Manhattan distance between two grid positions
    /// </summary>
    private int GetManhattanDistance(Vector3Int pos1, Vector3Int pos2)
    {
        return Mathf.Abs(pos1.x - pos2.x) + Mathf.Abs(pos1.y - pos2.y);
    }

    /// <summary>
    /// Manual combat stop (e.g., player runs away)
    /// </summary>
    public void StopCombat()
    {
        EndCombat();
    }

    /// <summary>
    /// Spawn a hit splat above a target
    /// </summary>
    private void SpawnHitSplat(int damage, Vector3 targetPosition, Transform targetTransform = null)
    {
        if (hitSplatPrefab == null)
        {
            Debug.LogWarning("CombatSystem: Hit splat prefab not assigned!");
            return;
        }

        GameObject splat = Instantiate(hitSplatPrefab);
        HitSplat hitSplat = splat.GetComponent<HitSplat>();
        
        if (hitSplat != null)
        {
            hitSplat.Initialize(damage, targetPosition, targetTransform);
        }
        else
        {
            Debug.LogError("Hit splat prefab doesn't have HitSplat component!");
            Destroy(splat);
        }
    }

    void OnDestroy()
    {
        if (currentTarget != null)
        {
            currentTarget.OnDeath -= OnEnemyDied;
        }
    }
}
