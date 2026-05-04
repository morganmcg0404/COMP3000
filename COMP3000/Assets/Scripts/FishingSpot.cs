using System.Collections;
using UnityEngine;

public class FishingSpot : MonoBehaviour
{
    [Header("Fishing Settings")]
    [SerializeField] private int fishIndex = 0;
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private float respawnDelay = 30f;

    [Header("References")]
    [SerializeField] private FishingSystem fishingSystem;
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

        if (fishingSystem == null)
            fishingSystem = FindFirstObjectByType<FishingSystem>();

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
    }

    void OnMouseDown()
    {
        if (isDepleted)
        {
            Debug.Log("This fishing spot is depleted! It will respawn soon.");
            return;
        }

        if (fishingSystem == null || gridManager == null || playerController == null)
        {
            Debug.LogError("FishingSpot: Missing required references!");
            return;
        }

        Debug.Log($"Started fishing for {GetFishName()}");
        StartCoroutine(PathToAndFish());
    }

    private IEnumerator PathToAndFish()
    {
        Vector3Int spotGridPos = gridManager.WorldToGrid(transform.position);
        Vector3Int nearestAdjacentTile = FindNearestAdjacentTile(spotGridPos);

        if (nearestAdjacentTile == Vector3Int.zero)
        {
            Debug.LogWarning("Could not find walkable tile adjacent to fishing spot!");
            yield break;
        }

        Debug.Log($"Pathing to adjacent tile {nearestAdjacentTile}");

        Vector3Int playerPos = playerController.CurrentGridPosition;
        var currentPath = pathfinder.FindPath(playerPos, nearestAdjacentTile, gridManager);

        if (currentPath == null || currentPath.Count == 0)
        {
            Debug.LogWarning("Could not find path to fishing spot!");
            yield break;
        }

        playerController.MoveTo(nearestAdjacentTile);

        while (playerController.CurrentGridPosition != nearestAdjacentTile)
        {
            yield return null;
        }

        Debug.Log($"Player reached adjacent tile, starting to fish for {GetFishName()}");
        fishingSystem.StartFishing(fishIndex, this);
    }

    private Vector3Int FindNearestAdjacentTile(Vector3Int spotPos)
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

        foreach (Vector3Int offset in adjacentOffsets)
        {
            Vector3Int checkPos = spotPos + offset;

            if (gridManager.IsTileWalkable(checkPos))
            {
                float distanceToPlayer = Vector3Int.Distance(checkPos, playerController.CurrentGridPosition);

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
        if (fishingSystem != null)
        {
            fishingSystem.StopFishing();
        }
    }

    public void OnFishCaught()
    {
        currentHealth--;
        Debug.Log($"{GetFishName()} spot health: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            Deplete();
        }
    }

    private void Deplete()
    {
        isDepleted = true;
        Debug.Log($"<color=red>{GetFishName()} spot depleted!</color>");

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

        Debug.Log($"<color=green>{GetFishName()} spot respawned!</color>");

        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
    }

    private string GetFishName()
    {
        if (fishingSystem != null)
        {
            FishingSystem.FishData[] allFish = fishingSystem.GetAllFishData();
            if (fishIndex >= 0 && fishIndex < allFish.Length)
            {
                return allFish[fishIndex].name;
            }
        }
        return $"Fish (Index {fishIndex})";
    }

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public bool IsDepleted => isDepleted;
}
