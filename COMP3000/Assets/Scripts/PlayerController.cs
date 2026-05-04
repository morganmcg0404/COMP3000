using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float runSpeed = 5f;
    [SerializeField] private bool isRunning = true;
    
    private float moveSpeed;
    
    [Header("Skill System")]
    [SerializeField] private SkillSystem skillSystem;
    
    [Header("Equipment System")]
    [SerializeField] private PlayerEquipment playerEquipment;
    
    [Header("Combat System")]
    [SerializeField] private CombatSystem combatSystem;
    
    private GridManager gridManager;
    private Pathfinder pathfinder;
    private List<Vector3Int> currentPath;
    private bool isMoving = false;
    private Vector3Int currentGridPosition;
    private Coroutine followPathCoroutine;

    public bool IsRunning => isRunning;
    public bool IsWalking => !isRunning;
    public float CurrentSpeed => moveSpeed;
    public Vector3Int CurrentGridPosition => currentGridPosition;

    public System.Action<bool> OnMovementModeChanged;

    void Start()
    {
        Debug.Log("=== PlayerController Start() called ===");
        
        moveSpeed = isRunning ? runSpeed : walkSpeed;
        
        gridManager = FindFirstObjectByType<GridManager>();
        pathfinder = new Pathfinder();
        
        if (gridManager == null)
        {
            Debug.LogError("PlayerController: GridManager not found! Make sure you have a GridManager in the scene.");
            return;
        }
        
        Debug.Log("GridManager found successfully!");
        
        if (skillSystem == null)
        {
            skillSystem = FindFirstObjectByType<SkillSystem>();
            if (skillSystem == null)
            {
                Debug.LogWarning("PlayerController: SkillSystem not found! Skills will not be tracked.");
            }
            else
            {
                Debug.Log("SkillSystem found successfully!");
            }
        }
        
        if (playerEquipment == null)
        {
            playerEquipment = GetComponent<PlayerEquipment>();
            if (playerEquipment == null)
            {
                Debug.LogWarning("PlayerController: PlayerEquipment not found! Equipment stats will not be available.");
            }
            else
            {
                Debug.Log("PlayerEquipment found successfully!");
            }
        }
        
        if (combatSystem == null)
        {
            combatSystem = FindFirstObjectByType<CombatSystem>();
            if (combatSystem == null)
            {
                Debug.LogWarning("PlayerController: CombatSystem not found! Combat will not be available.");
            }
            else
            {
                Debug.Log("CombatSystem found successfully!");
            }
        }
        
        currentGridPosition = gridManager.WorldToGrid(transform.position);
        transform.position = gridManager.GridToWorld(currentGridPosition);
        Debug.Log($"Player starting at grid position: {currentGridPosition}, world position: {transform.position}");
        Debug.Log($"Player Y position: {transform.position.y}");
    }

    void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        bool mouseClicked = false;
        Vector3 mousePosition = Vector3.zero;
        
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (IsPointerOverButton())
                return;

            mouseClicked = true;
            mousePosition = Mouse.current.position.ReadValue();
            Debug.Log($"MOUSE CLICKED at screen position: {mousePosition}");
        }
        
        if (!mouseClicked)
        {
            return;
        }
        
        Debug.Log("HandleInput: Processing mouse click!");
        
        if (gridManager == null)
        {
            Debug.LogError("GridManager is null!");
            return;
        }
        
        if (Camera.main == null)
        {
            Debug.LogError("Main Camera is null!");
            return;
        }
        
        Vector3 screenPos = new Vector3(mousePosition.x, mousePosition.y, 10f);
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(screenPos);
        mouseWorldPos.z = 0;
        
        Debug.Log($"Screen pos: {mousePosition}, World pos: {mouseWorldPos}");
        
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(mousePosition.x, mousePosition.y, 0));
        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);
        
        Debug.Log($"Raycast performed - Hit something: {hit.collider != null}");
        if (hit.collider != null)
        {
            Debug.Log($"Hit object: {hit.collider.gameObject.name}");
            Enemy enemy = hit.collider.GetComponent<Enemy>();
            if (enemy != null && !enemy.IsDead)
            {
                Debug.Log($"Clicked on enemy: {enemy.EnemyName}");
                
                Vector3 enemyPosNoOffset = enemy.transform.position;
                enemyPosNoOffset.y -= 0.5f;
                Vector3Int enemyGridPos = gridManager.WorldToGrid(enemyPosNoOffset);
                Vector3Int nearestAdjacentTile = FindNearestAdjacentTile(enemyGridPos);
                
                if (nearestAdjacentTile != Vector3Int.zero)
                {
                    Debug.Log($"Moving to adjacent tile {nearestAdjacentTile} to engage enemy");
                    StartCoroutine(MoveToEnemyAndStartCombat(nearestAdjacentTile, enemy));
                }
                else
                {
                    Debug.LogWarning("Could not find walkable tile adjacent to enemy!");
                }
                return;
            }
            else if (enemy == null)
            {
                Debug.Log("Hit object doesn't have Enemy component");
            }
            else
            {
                Debug.Log("Enemy is dead");
            }
        }
        
        Vector3Int targetGridPos = gridManager.WorldToGrid(mouseWorldPos);
        
        Debug.Log($"Target grid position: {targetGridPos}");
        
        if (gridManager.IsTileWalkable(targetGridPos))
        {
            Debug.Log($"Tile at {targetGridPos} is walkable. Moving...");
            MoveTo(targetGridPos);
        }
        else
        {
            Debug.LogWarning($"Tile at {targetGridPos} is NOT walkable. Finding closest walkable tile...");
            Vector3Int closestWalkable = FindClosestWalkableTile(targetGridPos);
            
            if (closestWalkable != Vector3Int.zero)
            {
                Debug.Log($"Found closest walkable tile at {closestWalkable}. Moving there instead.");
                MoveTo(closestWalkable);
            }
            else
            {
                Debug.LogWarning("Could not find any walkable tile nearby!");
            }
        }
    }
    
    Vector3Int FindClosestWalkableTile(Vector3Int targetPos)
    {
        int maxSearchRadius = 20;
        Vector3Int closestTile = Vector3Int.zero;
        float closestDistanceToPlayer = float.MaxValue;
        
        for (int radius = 1; radius <= maxSearchRadius; radius++)
        {
            List<Vector3Int> tilesAtThisDistance = new List<Vector3Int>();
            
            for (int x = -radius; x <= radius; x++)
            {
                for (int y = -radius; y <= radius; y++)
                {
                    if (Mathf.Abs(x) == radius || Mathf.Abs(y) == radius)
                    {
                        Vector3Int checkPos = targetPos + new Vector3Int(x, y, 0);
                        
                        if (gridManager.IsTileWalkable(checkPos))
                        {
                            tilesAtThisDistance.Add(checkPos);
                        }
                    }
                }
            }
            
            if (tilesAtThisDistance.Count > 0)
            {
                foreach (Vector3Int tile in tilesAtThisDistance)
                {
                    float distanceToPlayer = Vector3Int.Distance(tile, currentGridPosition);
                    
                    if (distanceToPlayer < closestDistanceToPlayer)
                    {
                        closestDistanceToPlayer = distanceToPlayer;
                        closestTile = tile;
                    }
                }
                
                return closestTile;
            }
        }
        
        return Vector3Int.zero;
    }

    public void MoveTo(Vector3Int targetGridPosition)
    {
        if (gridManager == null) return;

        Vector3Int startPos = currentGridPosition;
        
        Debug.Log($"Finding path from {startPos} to {targetGridPosition}");

        if (followPathCoroutine != null)
        {
            StopCoroutine(followPathCoroutine);
            Debug.Log("Interrupted previous movement");
        }

        currentPath = pathfinder.FindPath(startPos, targetGridPosition, gridManager);

        if (currentPath != null && currentPath.Count > 0)
        {
            Debug.Log($"Path found with {currentPath.Count} steps!");
            followPathCoroutine = StartCoroutine(FollowPath());
        }
        else
        {
            Debug.LogWarning("No path found!");
        }
    }

    IEnumerator FollowPath()
    {
        isMoving = true;

        foreach (Vector3Int gridPos in currentPath)
        {
            Vector3 targetPosition = gridManager.GridToWorld(gridPos);
            
            currentGridPosition = gridPos;
            
            while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
                yield return null;
            }

            transform.position = targetPosition;
        }

        isMoving = false;
        currentPath = null;
        followPathCoroutine = null;
    }

    public void ToggleMovementMode()
    {
        isRunning = !isRunning;
        moveSpeed = isRunning ? runSpeed : walkSpeed;
        
        Debug.Log($"<color=cyan>Movement mode changed to: {(isRunning ? "RUNNING" : "WALKING")} (Speed: {moveSpeed})</color>");
        
        OnMovementModeChanged?.Invoke(isRunning);
    }

    public void SetRunning(bool running)
    {
        if (isRunning != running)
        {
            isRunning = running;
            moveSpeed = isRunning ? runSpeed : walkSpeed;
            
            Debug.Log($"<color=cyan>Movement mode set to: {(isRunning ? "RUNNING" : "WALKING")} (Speed: {moveSpeed})</color>");
            
            OnMovementModeChanged?.Invoke(isRunning);
        }
    }

    Vector3Int FindNearestAdjacentTile(Vector3Int targetPos)
    {
        Vector3Int closestTile = Vector3Int.zero;
        float closestDistance = float.MaxValue;
        
        Vector3Int[] adjacentOffsets = new Vector3Int[]
        {
            new Vector3Int(1, 0, 0),
            new Vector3Int(-1, 0, 0),
            new Vector3Int(0, 1, 0),
            new Vector3Int(0, -1, 0),
            new Vector3Int(1, 1, 0),
            new Vector3Int(-1, 1, 0),
            new Vector3Int(1, -1, 0),
            new Vector3Int(-1, -1, 0)
        };
        
        foreach (Vector3Int offset in adjacentOffsets)
        {
            Vector3Int checkPos = targetPos + offset;
            
            if (gridManager.IsTileWalkable(checkPos))
            {
                float distanceToPlayer = Vector3Int.Distance(checkPos, currentGridPosition);
                
                if (distanceToPlayer < closestDistance)
                {
                    closestDistance = distanceToPlayer;
                    closestTile = checkPos;
                }
            }
        }
        
        return closestTile;
    }
    
    IEnumerator MoveToEnemyAndStartCombat(Vector3Int adjacentTile, Enemy enemy)
    {
        MoveTo(adjacentTile);
        
        while (isMoving)
        {
            yield return null;
        }
        
        Debug.Log($"Reached adjacent tile, starting combat with {enemy.EnemyName}");
        if (combatSystem != null)
        {
            combatSystem.StartCombat(enemy);
        }
        else
        {
            Debug.LogError("CombatSystem is null!");
        }
    }

    private bool IsPointerOverButton()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = Mouse.current.position.ReadValue();
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);
        
        foreach (var result in results)
        {
            if (result.gameObject.GetComponent<Canvas>() != null)
                continue;
            
            if (result.gameObject.GetComponent<CanvasRenderer>() != null)
                return true;
        }
        return false;
    }

    void OnDrawGizmos()
    {
        if (currentPath != null && currentPath.Count > 0 && gridManager != null)
        {
            Gizmos.color = Color.yellow;
            for (int i = 0; i < currentPath.Count; i++)
            {
                Vector3 pos = gridManager.GridToWorld(currentPath[i]);
                Gizmos.DrawWireSphere(pos, 0.3f);
                
                if (i < currentPath.Count - 1)
                {
                    Vector3 nextPos = gridManager.GridToWorld(currentPath[i + 1]);
                    Gizmos.DrawLine(pos, nextPos);
                }
            }
        }
    }
}
