using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    [Header("Tilemap References")]
    [SerializeField] private Tilemap[] walkableTilemaps;
    [SerializeField] private Tilemap[] obstacleTilemaps;

    [Header("Pathfinding Settings")]
    [SerializeField] private bool allowDiagonalMovement = true;
    
    private Grid grid;
    private BoundsInt bounds;

    void Awake()
    {
        Debug.Log("=== GridManager Awake() called ===");
        
        if (walkableTilemaps == null || walkableTilemaps.Length == 0)
        {
            Debug.LogError("GridManager: No walkable tilemaps assigned! Please assign at least one walkable tilemap.");
            return;
        }
        
        walkableTilemaps = System.Array.FindAll(walkableTilemaps, tilemap => tilemap != null);
        if (obstacleTilemaps != null)
        {
            obstacleTilemaps = System.Array.FindAll(obstacleTilemaps, tilemap => tilemap != null);
        }
        
        if (walkableTilemaps.Length == 0)
        {
            Debug.LogError("GridManager: All walkable tilemaps are null!");
            return;
        }
        
        Debug.Log($"Found {walkableTilemaps.Length} walkable tilemap(s)");
        if (obstacleTilemaps != null && obstacleTilemaps.Length > 0)
        {
            Debug.Log($"Found {obstacleTilemaps.Length} obstacle tilemap(s)");
        }
        
        grid = walkableTilemaps[0].layoutGrid;
        
        if (grid == null)
        {
            Debug.LogError("GridManager: Could not get Grid from Tilemap! Make sure your Tilemap is a child of a Grid GameObject.");
            return;
        }
        
        CalculateCombinedBounds();
        
        Debug.Log($"GridManager initialized successfully!");
        Debug.Log($"Combined tilemap bounds: {bounds}");
        Debug.Log($"Grid cell size: {grid.cellSize}");
    }
    
    void CalculateCombinedBounds()
    {
        bool firstBounds = true;
        
        foreach (Tilemap tilemap in walkableTilemaps)
        {
            if (tilemap == null) continue;
            
            tilemap.CompressBounds();
            BoundsInt tilemapBounds = tilemap.cellBounds;
            
            if (firstBounds)
            {
                bounds = tilemapBounds;
                firstBounds = false;
            }
            else
            {
                Vector3Int min = Vector3Int.Min(bounds.min, tilemapBounds.min);
                Vector3Int max = Vector3Int.Max(bounds.max, tilemapBounds.max);
                bounds = new BoundsInt(min, max - min);
            }
        }
    }

    public Vector3 GridToWorld(Vector3Int gridPosition)
    {
        if (grid == null)
        {
            Debug.LogError("Grid is null in GridToWorld!");
            return Vector3.zero;
        }
        return grid.GetCellCenterWorld(gridPosition);
    }

    public Vector3Int WorldToGrid(Vector3 worldPosition)
    {
        if (grid == null)
        {
            Debug.LogError("Grid is null in WorldToGrid!");
            return Vector3Int.zero;
        }
        return grid.WorldToCell(worldPosition);
    }

    public bool IsTileWalkable(Vector3Int gridPosition)
    {
        if (walkableTilemaps == null || walkableTilemaps.Length == 0)
        {
            Debug.LogError("No walkable tilemaps assigned!");
            return false;
        }
        
        bool hasTile = false;
        TileBase foundTile = null;
        Tilemap foundTilemap = null;
        
        foreach (Tilemap tilemap in walkableTilemaps)
        {
            if (tilemap != null && tilemap.HasTile(gridPosition))
            {
                hasTile = true;
                foundTile = tilemap.GetTile(gridPosition);
                foundTilemap = tilemap;
                break;
            }
        }
        
        if (!hasTile)
        {
            return false;
        }
        
        if (foundTile is UnwalkableTile)
        {
            return false;
        }

        if (obstacleTilemaps != null && obstacleTilemaps.Length > 0)
        {
            foreach (Tilemap obstacleTilemap in obstacleTilemaps)
            {
                if (obstacleTilemap != null && obstacleTilemap.HasTile(gridPosition))
                {
                    return false;
                }
            }
        }

        Vector3 worldPosition = GridToWorld(gridPosition);
        Collider2D[] colliders = Physics2D.OverlapCircleAll(worldPosition, 0.3f);
        foreach (Collider2D col in colliders)
        {
            if (!col.isTrigger && col.gameObject.tag != "Player")
            {
                return false;
            }
        }

        return true;
    }

    public bool IsWithinBounds(Vector3Int gridPosition)
    {
        return bounds.Contains(gridPosition);
    }

    public List<Vector3Int> GetNeighbors(Vector3Int gridPosition)
    {
        List<Vector3Int> neighbors = new List<Vector3Int>();

        Vector3Int[] cardinalDirections = new Vector3Int[]
        {
            new Vector3Int(0, 1, 0),
            new Vector3Int(0, -1, 0),
            new Vector3Int(-1, 0, 0),
            new Vector3Int(1, 0, 0)
        };

        Vector3Int[] diagonalDirections = new Vector3Int[]
        {
            new Vector3Int(-1, 1, 0),
            new Vector3Int(1, 1, 0),
            new Vector3Int(-1, -1, 0),
            new Vector3Int(1, -1, 0)
        };

        foreach (Vector3Int direction in cardinalDirections)
        {
            Vector3Int neighborPos = gridPosition + direction;
            if (IsWithinBounds(neighborPos) && IsTileWalkable(neighborPos))
            {
                neighbors.Add(neighborPos);
            }
        }

        if (allowDiagonalMovement)
        {
            foreach (Vector3Int direction in diagonalDirections)
            {
                Vector3Int neighborPos = gridPosition + direction;
                if (IsWithinBounds(neighborPos) && IsTileWalkable(neighborPos))
                {
                    Vector3Int adjacentX = gridPosition + new Vector3Int(direction.x, 0, 0);
                    Vector3Int adjacentY = gridPosition + new Vector3Int(0, direction.y, 0);
                    
                    if (IsTileWalkable(adjacentX) || IsTileWalkable(adjacentY))
                    {
                        neighbors.Add(neighborPos);
                    }
                }
            }
        }

        return neighbors;
    }

    public BoundsInt Bounds => bounds;
    public Tilemap[] WalkableTilemaps => walkableTilemaps;
    public Tilemap[] ObstacleTilemaps => obstacleTilemaps;
}
