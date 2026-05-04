using System.Collections;
using UnityEngine;

public class OreRock : MonoBehaviour
{
    [Header("Ore Settings")]
    [SerializeField] private int oreIndex = 0;
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private float respawnDelay = 30f;

    [Header("References")]
    [SerializeField] private MiningSystem miningSystem;
    [SerializeField] private GridManager gridManager;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private int currentHealth;
    private bool isDepleted = false;
    private Color originalColor;
    private PlayerController playerController;
    private Pathfinder pathfinder;

    void Start()
    {
        currentHealth = maxHealth;

        if (miningSystem == null)
            miningSystem = FindFirstObjectByType<MiningSystem>();

        if (gridManager == null)
            gridManager = FindFirstObjectByType<GridManager>();

        if (playerController == null)
            playerController = FindFirstObjectByType<PlayerController>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (pathfinder == null)
            pathfinder = new Pathfinder();

        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;

        if (gridManager != null)
        {
            Collider2D col = GetComponent<Collider2D>();
            if (col == null)
            {
                col = gameObject.AddComponent<BoxCollider2D>();
            }
            col.isTrigger = true;
        }
    }

    void OnMouseDown()
    {
        if (isDepleted)
        {
            Debug.Log("This ore rock is depleted! It will respawn soon.");
            return;
        }

        if (miningSystem == null || gridManager == null || playerController == null)
        {
            Debug.LogError("OreRock: Missing required references!");
            return;
        }

        Debug.Log($"Started mining {GetOreName()}");
        StartCoroutine(PathToAndMinOre());
    }

    private IEnumerator PathToAndMinOre()
    {
        // Subtract 0.5 offset from ore position to get true grid position
        Vector3 adjustedOrePos = transform.position - new Vector3(0.5f, 0.5f, 0);
        Vector3Int oreGridPos = gridManager.WorldToGrid(adjustedOrePos);
        Vector3Int playerPos = playerController.CurrentGridPosition;
        
        Debug.Log($"=== MINING DEBUG ===");
        Debug.Log($"Ore adjusted position: {adjustedOrePos}, grid: {oreGridPos}");
        Debug.Log($"Player position: {playerController.transform.position}, grid: {playerPos}");
        
        Vector3Int nearestAdjacentTile = FindNearestAdjacentTile(oreGridPos);

        Debug.Log($"Nearest adjacent tile: {nearestAdjacentTile}");
        
        if (nearestAdjacentTile == Vector3Int.zero)
        {
            Debug.LogWarning("Could not find walkable tile adjacent to ore rock!");
            yield break;
        }

        // If the nearest adjacent tile is the player's current position, start mining immediately
        if (nearestAdjacentTile == playerPos)
        {
            Debug.Log("Player already on adjacent tile, starting mining");
            miningSystem.StartMining(oreIndex, this);
            yield break;
        }

        Debug.Log($"Pathing to adjacent tile {nearestAdjacentTile}");

        var currentPath = pathfinder.FindPath(playerPos, nearestAdjacentTile, gridManager);

        if (currentPath == null || currentPath.Count == 0)
        {
            Debug.LogWarning("Could not find path to ore rock!");
            yield break;
        }

        Debug.Log($"Path found with {currentPath.Count} tiles");

        playerController.MoveTo(nearestAdjacentTile);

        // Wait for player to reach the adjacent tile (with timeout)
        float timeout = 0f;
        while (playerController.CurrentGridPosition != nearestAdjacentTile && timeout < 30f)
        {
            timeout += Time.deltaTime;
            yield return null;
        }

        if (playerController.CurrentGridPosition != nearestAdjacentTile)
        {
            Debug.LogWarning("Player did not reach adjacent tile in time!");
            yield break;
        }

        Debug.Log($"Player reached adjacent tile {nearestAdjacentTile}, starting to mine {GetOreName()}");
        miningSystem.StartMining(oreIndex, this);
    }

    private Vector3Int FindNearestAdjacentTile(Vector3Int orePos)
    {
        Vector3Int playerGridPos = playerController.CurrentGridPosition;
        
        // Only check cardinal directions (up, down, left, right)
        Vector3Int[] cardinalOffsets = new Vector3Int[]
        {
            new Vector3Int(1, 0, 0),   // Right
            new Vector3Int(-1, 0, 0),  // Left
            new Vector3Int(0, 1, 0),   // Up
            new Vector3Int(0, -1, 0)   // Down
        };

        // First check if player is already on any cardinal adjacent tile
        foreach (Vector3Int offset in cardinalOffsets)
        {
            Vector3Int checkPos = orePos + offset;
            if (checkPos == playerGridPos)
            {
                Debug.Log($"Player already on adjacent tile {checkPos}");
                return checkPos;
            }
        }

        // Find the closest walkable cardinal tile
        Vector3Int closestTile = Vector3Int.zero;
        float closestDistance = float.MaxValue;
        
        foreach (Vector3Int offset in cardinalOffsets)
        {
            Vector3Int checkPos = orePos + offset;

            if (gridManager.IsTileWalkable(checkPos))
            {
                float distanceToPlayer = Vector3Int.Distance(checkPos, playerGridPos);
                if (distanceToPlayer < closestDistance)
                {
                    closestDistance = distanceToPlayer;
                    closestTile = checkPos;
                }
            }
        }

        return closestTile;
    }

    void OnMouseUp()
    {
        if (miningSystem != null)
        {
            miningSystem.StopMining();
        }
    }

    public void OnOreMined()
    {
        currentHealth--;
        Debug.Log($"{GetOreName()} health: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            Deplete();
        }
    }

    private void Deplete()
    {
        isDepleted = true;
        Debug.Log($"<color=red>{GetOreName()} rock depleted!</color>");

        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.gray;
        }

        Invoke(nameof(Respawn), respawnDelay);
    }

    private void Respawn()
    {
        currentHealth = maxHealth;
        isDepleted = false;

        Debug.Log($"<color=green>{GetOreName()} rock respawned!</color>");

        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
    }

    private string GetOreName()
    {
        if (miningSystem != null)
        {
            MiningSystem.OreData[] allOre = miningSystem.GetAllOreData();
            if (oreIndex >= 0 && oreIndex < allOre.Length)
            {
                return allOre[oreIndex].name;
            }
        }
        return $"Ore (Index {oreIndex})";
    }

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public bool IsDepleted => isDepleted;
}
