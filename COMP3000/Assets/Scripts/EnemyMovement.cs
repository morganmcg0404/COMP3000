using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GridManager gridManager;
    [SerializeField] private Pathfinder pathfinder;
    [SerializeField] private Enemy enemy;
    [SerializeField] private CombatSystem combatSystem;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float wanderInterval = 3f; // Seconds between random movements
    [SerializeField] private float yOffset = 0.5f; // Y offset to align with player sprite

    [Header("Range Settings")]
    [SerializeField] private int wanderRange = 5;
    [SerializeField] private int aggressionRange = 15;

    [Header("Reset Settings")]
    [SerializeField] private float hpResetDelay = 10f;

    [Header("Current State")]
    [SerializeField] private Vector3Int spawnPoint;
    [SerializeField] private Vector3Int currentGridPosition;
    [SerializeField] private EnemyState currentState = EnemyState.Idle;
    [SerializeField] private bool isMoving = false;
    private List<Vector3Int> currentPath;
    private Coroutine wanderCoroutine;
    private Coroutine moveCoroutine;
    private float lastCombatTime;
    private float timeAtSpawn;
    private bool hasResetHP = true;
    private Transform playerTransform;

    public Vector3Int SpawnPoint => spawnPoint;
    public Vector3Int CurrentGridPosition => currentGridPosition;
    public EnemyState CurrentState => currentState;
    public int WanderRange => wanderRange;
    public int AggressionRange => aggressionRange;

    void Start()
    {
        InitializeMovement();
    }

    void Update()
    {
        if (enemy != null && enemy.IsDead)
        {
            StopAllMovement();
            return;
        }

        // Update state based on combat and position
        UpdateState();

        // Check for HP reset at spawn
        CheckHPReset();
    }

    private void InitializeMovement()
    {
        // Find references
        if (gridManager == null)
            gridManager = FindFirstObjectByType<GridManager>();

        if (enemy == null)
            enemy = GetComponent<Enemy>();

        if (combatSystem == null)
            combatSystem = FindFirstObjectByType<CombatSystem>();

        if (pathfinder == null)
            pathfinder = new Pathfinder();

        if (gridManager == null)
        {
            Debug.LogError($"EnemyMovement: GridManager not found for {gameObject.name}!");
            return;
        }

        // Find player
        PlayerController player = FindFirstObjectByType<PlayerController>();
        if (player != null)
            playerTransform = player.transform;

        // Set spawn point to current position (without offset for grid)
        // Subtract offset before calculating grid position
        Vector3 unoffsetPos = transform.position;
        unoffsetPos.y -= yOffset;
        spawnPoint = gridManager.WorldToGrid(unoffsetPos);
        currentGridPosition = spawnPoint;
        
        // Set visual position at grid with Y offset for rendering
        Vector3 spawnPos = gridManager.GridToWorld(spawnPoint);
        spawnPos.y += yOffset;
        transform.position = spawnPos;

        // Start wandering
        StartWandering();
    }

    private void UpdateState()
    {
        if (enemy == null || gridManager == null) return;

        // If in combat, track last combat time
        if (enemy.IsInCombat)
        {
            lastCombatTime = Time.time;
            
            // Check if player is too far away
            if (playerTransform != null)
            {
                Vector3Int playerGridPos = gridManager.WorldToGrid(playerTransform.position);
                int distanceToPlayer = GetManhattanDistance(currentGridPosition, playerGridPos);

                if (distanceToPlayer > aggressionRange)
                {
                    // Player ran too far, return to spawn and cancel combat
                    SetState(EnemyState.ReturningToSpawn);
                    enemy.SetInCombat(false);
                    
                    // End combat on CombatSystem too
                    if (combatSystem != null)
                    {
                        combatSystem.EndCombat();
                    }
                    
                    ReturnToSpawn();
                }
                else
                {
                    // Follow player - move towards them if not adjacent
                    SetState(EnemyState.InCombat);
                    if (distanceToPlayer > 1 && !isMoving)
                    {
                        // Find the closest adjacent tile to the player
                        Vector3Int adjacentPos = GetClosestAdjacentTile(currentGridPosition, playerGridPos);
                        MoveToPosition(adjacentPos);
                    }
                }
            }
        }
        else
        {
            // Not in combat - check if should return to spawn or wander
            int distanceToSpawn = GetManhattanDistance(currentGridPosition, spawnPoint);

            if (distanceToSpawn > wanderRange && currentState != EnemyState.ReturningToSpawn)
            {
                // Too far from spawn, return
                Debug.Log($"{gameObject.name} too far from spawn. Distance: {distanceToSpawn} > {wanderRange}");
                SetState(EnemyState.ReturningToSpawn);
                ReturnToSpawn();
            }
            else if (currentState == EnemyState.ReturningToSpawn && distanceToSpawn <= 1)
            {
                // Reached spawn point (within 1 tile)
                Debug.Log($"{gameObject.name} reached spawn point");
                SetState(EnemyState.Idle);
                timeAtSpawn = Time.time;
                hasResetHP = false;
                StartWandering();
            }
            else if (currentState != EnemyState.ReturningToSpawn && currentState != EnemyState.Wandering)
            {
                SetState(EnemyState.Idle);
            }
        }
    }

    /// <summary>
    /// Check if HP should reset
    /// </summary>
    private void CheckHPReset()
    {
        if (hasResetHP || enemy == null || enemy.IsDead) return;

        // Check if at spawn point
        if (currentGridPosition == spawnPoint)
        {
            // Check if enough time has passed without combat
            float timeSinceLastCombat = Time.time - lastCombatTime;
            float timeAtSpawnPoint = Time.time - timeAtSpawn;

            if (timeSinceLastCombat >= hpResetDelay && timeAtSpawnPoint >= hpResetDelay)
            {
                // Reset HP
                int missingHealth = enemy.MaxHealth - enemy.CurrentHealth;
                if (missingHealth > 0)
                {
                    enemy.Heal(missingHealth);
                }
                hasResetHP = true;
            }
        }
    }

    /// <summary>
    /// Start wandering behavior
    /// </summary>
    public void StartWandering()
    {
        if (wanderCoroutine != null)
            StopCoroutine(wanderCoroutine);

        wanderCoroutine = StartCoroutine(WanderRoutine());
    }

    /// <summary>
    /// Stop wandering behavior
    /// </summary>
    public void StopWandering()
    {
        if (wanderCoroutine != null)
        {
            StopCoroutine(wanderCoroutine);
            wanderCoroutine = null;
        }
    }

    /// <summary>
    /// Wander routine - moves randomly within wander range
    /// </summary>
    private IEnumerator WanderRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(wanderInterval);

            // Don't wander if in combat or dead
            if (enemy != null && (enemy.IsInCombat || enemy.IsDead))
                continue;

            // Don't wander if already moving
            if (isMoving)
                continue;

            // Pick random position within wander range
            Vector3Int targetPos = GetRandomWanderPosition();

            if (targetPos != currentGridPosition)
            {
                SetState(EnemyState.Wandering);
                MoveToPosition(targetPos);
            }
        }
    }

    /// <summary>
    /// Get a random valid position within wander range
    /// </summary>
    private Vector3Int GetRandomWanderPosition()
    {
        List<Vector3Int> validPositions = new List<Vector3Int>();

        // Check positions within wander range
        for (int x = -wanderRange; x <= wanderRange; x++)
        {
            for (int y = -wanderRange; y <= wanderRange; y++)
            {
                Vector3Int checkPos = spawnPoint + new Vector3Int(x, y, 0);
                
                // Check if within wander range (Manhattan distance)
                if (GetManhattanDistance(spawnPoint, checkPos) <= wanderRange)
                {
                    // Check if tile is walkable
                    if (gridManager.IsTileWalkable(checkPos))
                    {
                        validPositions.Add(checkPos);
                    }
                }
            }
        }

        if (validPositions.Count == 0)
            return currentGridPosition;

        // Return random valid position
        return validPositions[Random.Range(0, validPositions.Count)];
    }

    /// <summary>
    /// Return enemy to spawn point
    /// </summary>
    public void ReturnToSpawn()
    {
        StopWandering();
        
        // Stop current movement and reset isMoving flag
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
            moveCoroutine = null;
        }
        isMoving = false;
        
        MoveToPosition(spawnPoint);
    }

    /// <summary>
    /// Move to a specific grid position
    /// </summary>
    public void MoveToPosition(Vector3Int targetPosition)
    {
        if (gridManager == null)
        {
            Debug.LogError($"EnemyMovement: GridManager is null!");
            return;
        }

        if (isMoving)
        {
            return;
        }

        // Find path
        currentPath = pathfinder.FindPath(currentGridPosition, targetPosition, gridManager);

        if (currentPath != null && currentPath.Count > 0)
        {
            if (moveCoroutine != null)
                StopCoroutine(moveCoroutine);
            
            moveCoroutine = StartCoroutine(FollowPath());
        }
    }

    /// <summary>
    /// Follow the current path
    /// </summary>
    private IEnumerator FollowPath()
    {
        isMoving = true;

        foreach (Vector3Int gridPos in currentPath)
        {
            // Get target position at true grid (no offset)
            Vector3 targetPosition = gridManager.GridToWorld(gridPos);
            // Apply offset for visual display
            Vector3 visualTargetPosition = targetPosition;
            visualTargetPosition.y += yOffset;
            
            // Move smoothly to the tile
            while (Vector3.Distance(transform.position, visualTargetPosition) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, visualTargetPosition, moveSpeed * Time.deltaTime);
                yield return null;
            }

            // Set final position with offset
            transform.position = visualTargetPosition;
            // Update grid position without offset
            currentGridPosition = gridPos;
        }

        isMoving = false;
        currentPath = null;
        moveCoroutine = null;
    }

    /// <summary>
    /// Stop all movement
    /// </summary>
    public void StopAllMovement()
    {
        StopWandering();
        
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
            moveCoroutine = null;
        }

        isMoving = false;
        currentPath = null;
    }

    /// <summary>
    /// Set current state
    /// </summary>
    private void SetState(EnemyState newState)
    {
        if (currentState != newState)
        {
            currentState = newState;
        }
    }

    /// <summary>
    /// Calculate Manhattan distance between two grid positions
    /// </summary>
    private int GetManhattanDistance(Vector3Int a, Vector3Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    /// <summary>
    /// Get the closest walkable adjacent tile to a target position
    /// Prioritizes direction towards the target when there are ties
    /// </summary>
    private Vector3Int GetClosestAdjacentTile(Vector3Int fromPos, Vector3Int targetPos)
    {
        // The four tiles adjacent to the target (player)
        Vector3Int[] adjacentTiles = new Vector3Int[]
        {
            targetPos + Vector3Int.up,    // North of target
            targetPos + Vector3Int.down,  // South of target
            targetPos + Vector3Int.left,  // West of target
            targetPos + Vector3Int.right  // East of target
        };

        Vector3Int closest = Vector3Int.zero;
        int bestDistance = int.MaxValue;

        foreach (Vector3Int tile in adjacentTiles)
        {
            // Skip if not walkable
            if (!gridManager.IsTileWalkable(tile))
                continue;

            // Calculate how much this tile reduces distance from enemy to target
            int distanceToTarget = GetManhattanDistance(fromPos, tile);
            
            // Pick the tile with shortest distance from enemy's current position
            if (distanceToTarget < bestDistance)
            {
                bestDistance = distanceToTarget;
                closest = tile;
            }
        }

        // If no valid adjacent tile found, return current position
        return closest != Vector3Int.zero ? closest : fromPos;
    }

    /// <summary>
    /// Called when combat starts
    /// </summary>
    public void OnCombatStarted()
    {
        lastCombatTime = Time.time;
        hasResetHP = true; // Don't reset HP while in combat
        StopWandering();
        SetState(EnemyState.InCombat);
    }

    /// <summary>
    /// Called when combat ends
    /// </summary>
    public void OnCombatEnded()
    {
        // Will be handled by Update() state check
    }

    /// <summary>
    /// Draw gizmos for debugging
    /// </summary>
    void OnDrawGizmosSelected()
    {
        if (gridManager == null)
            gridManager = FindFirstObjectByType<GridManager>();

        if (gridManager == null)
            return;

        // Try to get spawn point, but don't draw if grid isn't initialized
        try
        {
            Vector3 posForGridCalc = transform.position;
            posForGridCalc.y -= yOffset; // Remove offset before calculating grid position
            Vector3Int drawSpawnPoint = spawnPoint == Vector3Int.zero ? gridManager.WorldToGrid(posForGridCalc) : spawnPoint;

            // Draw spawn point
            Gizmos.color = Color.green;
            Vector3 spawnWorldPos = gridManager.GridToWorld(drawSpawnPoint);
            Gizmos.DrawWireSphere(spawnWorldPos, 0.5f);

            // Draw wander range
            Gizmos.color = Color.yellow;
            for (int x = -wanderRange; x <= wanderRange; x++)
            {
                for (int y = -wanderRange; y <= wanderRange; y++)
                {
                    Vector3Int checkPos = drawSpawnPoint + new Vector3Int(x, y, 0);
                    if (GetManhattanDistance(drawSpawnPoint, checkPos) == wanderRange)
                    {
                        Vector3 worldPos = gridManager.GridToWorld(checkPos);
                        Gizmos.DrawWireCube(worldPos, Vector3.one * 0.3f);
                    }
                }
            }

            // Draw aggression range
            Gizmos.color = Color.red;
            for (int x = -aggressionRange; x <= aggressionRange; x++)
            {
                for (int y = -aggressionRange; y <= aggressionRange; y++)
                {
                    Vector3Int checkPos = drawSpawnPoint + new Vector3Int(x, y, 0);
                    if (GetManhattanDistance(drawSpawnPoint, checkPos) == aggressionRange)
                    {
                        Vector3 worldPos = gridManager.GridToWorld(checkPos);
                        Gizmos.DrawWireCube(worldPos, Vector3.one * 0.2f);
                    }
                }
            }

            // Draw current path
            if (currentPath != null && currentPath.Count > 0)
            {
                Gizmos.color = Color.cyan;
                for (int i = 0; i < currentPath.Count - 1; i++)
                {
                    Vector3 pos = gridManager.GridToWorld(currentPath[i]);
                    Vector3 nextPos = gridManager.GridToWorld(currentPath[i + 1]);
                    Gizmos.DrawLine(pos, nextPos);
                }
            }
        }
        catch (System.Exception)
        {
            // Silently fail if grid isn't initialized yet
            // This happens in editor before scene is fully loaded
        }
    }
}

/// <summary>
/// Enemy AI states
/// </summary>
public enum EnemyState
{
    Idle,
    Wandering,
    InCombat,
    ReturningToSpawn
}
