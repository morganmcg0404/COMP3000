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
        Vector3Int oreGridPos = gridManager.WorldToGrid(transform.position);
        Vector3Int nearestAdjacentTile = FindNearestAdjacentTile(oreGridPos);

        if (nearestAdjacentTile == Vector3Int.zero)
        {
            Debug.LogWarning("Could not find walkable tile adjacent to ore rock!");
            yield break;
        }

        Debug.Log($"Pathing to adjacent tile {nearestAdjacentTile}");

        Vector3Int playerPos = playerController.CurrentGridPosition;
        var currentPath = pathfinder.FindPath(playerPos, nearestAdjacentTile, gridManager);

        if (currentPath == null || currentPath.Count == 0)
        {
            Debug.LogWarning("Could not find path to ore rock!");
            yield break;
        }

        playerController.MoveTo(nearestAdjacentTile);

        while (playerController.CurrentGridPosition != nearestAdjacentTile)
        {
            yield return null;
        }

        Debug.Log($"Player reached adjacent tile, starting to mine {GetOreName()}");
        miningSystem.StartMining(oreIndex, this);
    }

    private Vector3Int FindNearestAdjacentTile(Vector3Int orePos)
    {
        Vector3Int closestTile = Vector3Int.zero;
        float closestDistance = float.MaxValue;

        Vector3Int[] adjacentOffsets = new Vector3Int[]
        {
            new Vector3Int(1, 0, 0),
            new Vector3Int(-1, 0, 0),
            new Vector3Int(0, 1, 0),
            new Vector3Int(0, -1, 0)
        };

        Vector3Int playerGridPos = playerController.CurrentGridPosition;

        foreach (Vector3Int offset in adjacentOffsets)
        {
            Vector3Int checkPos = orePos + offset;

            if (checkPos == orePos)
                continue;

            if (gridManager.IsTileWalkable(checkPos) && checkPos != orePos)
            {
                float distanceToPlayer = Vector3Int.Distance(checkPos, playerGridPos);

                if (distanceToPlayer < closestDistance)
                {
                    closestDistance = distanceToPlayer;
                    closestTile = checkPos;
                }
            }
        }

        if (closestTile == Vector3Int.zero)
        {
            Debug.LogWarning($"No walkable tile found adjacent to ore at {orePos}");
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
