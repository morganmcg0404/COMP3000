using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class SpawnPoint
    {
        [SerializeField] public Transform transform;
        [SerializeField] public string name = "Spawn Point";
        private bool isOccupied = false;

        public bool IsOccupied => isOccupied;
        public void SetOccupied(bool occupied) => isOccupied = occupied;
    }

    [System.Serializable]
    public enum EnemyType
    {
        Goblin,
        OrcWarrior,
        ShadowAssassin,
        Skeleton,
        Dragon
    }

    [Header("Spawn Configuration")]
    [SerializeField] private List<SpawnPoint> spawnPoints = new List<SpawnPoint>();
    [SerializeField] private int enemiesToSpawn = 5;
    [SerializeField] private EnemyType enemyType = EnemyType.Goblin;
    [SerializeField] private int spawnLevel = 1;
    [SerializeField] private float spawnDelay = 0.5f;
    [SerializeField] private float respawnDelay = 10f;

    [Header("Enemy Prefab")]
    [SerializeField] private GameObject enemyPrefab;

    [Header("References")]
    [SerializeField] private GridManager gridManager;

    private List<Enemy> spawnedEnemies = new List<Enemy>();

    void Start()
    {
        if (gridManager == null)
            gridManager = FindFirstObjectByType<GridManager>();

        SpawnEnemies();
    }

    [ContextMenu("Spawn Enemies Now")]
    public void DebugSpawnEnemies()
    {
        SpawnEnemies();
    }

    public void SpawnEnemies()
    {
        if (spawnPoints.Count == 0)
        {
            Debug.LogError("EnemySpawner: No spawn points assigned!");
            return;
        }

        if (enemyPrefab == null)
        {
            Debug.LogError("EnemySpawner: Enemy prefab not assigned!");
            return;
        }

        int enemyCount = Mathf.Min(enemiesToSpawn, spawnPoints.Count);
        
        if (enemiesToSpawn > spawnPoints.Count)
        {
            Debug.LogWarning($"EnemySpawner: Trying to spawn {enemiesToSpawn} enemies but only {spawnPoints.Count} spawn points available. Spawning {enemyCount} enemies instead.");
        }

        foreach (var point in spawnPoints)
        {
            point.SetOccupied(false);
        }

        List<SpawnPoint> selectedPoints = SelectRandomSpawnPoints(enemyCount);

        StartCoroutine(SpawnEnemiesCoroutine(selectedPoints));
    }

    private List<SpawnPoint> SelectRandomSpawnPoints(int count)
    {
        List<SpawnPoint> selected = new List<SpawnPoint>();
        List<int> availableIndices = new List<int>();

        for (int i = 0; i < spawnPoints.Count; i++)
        {
            availableIndices.Add(i);
        }

        for (int i = 0; i < count; i++)
        {
            int randomIndex = Random.Range(0, availableIndices.Count);
            int spawnPointIndex = availableIndices[randomIndex];
            selected.Add(spawnPoints[spawnPointIndex]);
            availableIndices.RemoveAt(randomIndex);
        }

        return selected;
    }

    private System.Collections.IEnumerator SpawnEnemiesCoroutine(List<SpawnPoint> spawnPoints)
    {
        foreach (SpawnPoint point in spawnPoints)
        {
            SpawnEnemyAtPoint(point);
            yield return new WaitForSeconds(spawnDelay);
        }

        Debug.Log($"<color=yellow>Spawned {spawnPoints.Count} enemies of type {enemyType}</color>");
    }

    private void SpawnEnemyAtPoint(SpawnPoint point)
    {
        if (point.transform == null)
        {
            Debug.LogError("EnemySpawner: Spawn point has no transform!");
            return;
        }

        GameObject enemyObj = Instantiate(enemyPrefab, point.transform.position, Quaternion.identity);
        Enemy enemy = enemyObj.GetComponent<Enemy>();

        if (enemy == null)
        {
            Debug.LogError("EnemySpawner: Enemy prefab doesn't have Enemy component!");
            Destroy(enemyObj);
            return;
        }

        SetupEnemyType(enemy);

        if (gridManager != null)
        {
            Vector3Int gridPos = gridManager.WorldToGrid(enemyObj.transform.position);
            enemyObj.transform.position = gridManager.GridToWorld(gridPos);
        }

        point.SetOccupied(true);

        spawnedEnemies.Add(enemy);

        enemy.OnDeath += () => OnEnemyDied(enemy, point);

        Debug.Log($"<color=cyan>Spawned {enemy.EnemyName} at {point.name}</color>");
    }

    private void SetupEnemyType(Enemy enemy)
    {
        switch (enemyType)
        {
            case EnemyType.Goblin:
                EnemyFactory.SetupGoblin(enemy);
                break;
            case EnemyType.OrcWarrior:
                EnemyFactory.SetupOrcWarrior(enemy);
                break;
            case EnemyType.ShadowAssassin:
                EnemyFactory.SetupShadowAssassin(enemy);
                break;
            case EnemyType.Skeleton:
                SetupSkeleton(enemy);
                break;
            case EnemyType.Dragon:
                SetupDragon(enemy);
                break;
        }

        SetEnemyLevel(enemy, spawnLevel);
    }

    private void SetEnemyLevel(Enemy enemy, int level)
    {
        enemy.GetType().GetField("combatLevel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(enemy, level);
    }

    private void SetupSkeleton(Enemy enemy)
    {
        enemy.GetType().GetField("enemyName", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(enemy, "Skeleton");
        enemy.GetType().GetField("combatLevel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(enemy, 12);
        enemy.GetType().GetField("stats", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(enemy, new EquipmentStats(accuracy: 12, defence: 12, strength: 15, attackSpeed: 0));
        enemy.GetType().GetField("attackSpeed", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(enemy, 4);
        enemy.GetType().GetField("maxHealth", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(enemy, 75);
        enemy.GetType().GetField("currentHealth", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(enemy, 75);

        LootTable lootTable = enemy.GetComponent<LootTable>();
        if (lootTable == null)
            lootTable = enemy.gameObject.AddComponent<LootTable>();

        var drops = LootTableTemplates.CreateBasicEnemyDrops();
        foreach (var drop in drops)
            lootTable.AddItemDrop(drop);

        EnemyMovement movement = enemy.GetComponent<EnemyMovement>();
        if (movement == null)
            movement = enemy.gameObject.AddComponent<EnemyMovement>();

        movement.GetType().GetField("wanderRange", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(movement, 4);
        movement.GetType().GetField("aggressionRange", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(movement, 12);
    }

    private void SetupDragon(Enemy enemy)
    {
        enemy.GetType().GetField("enemyName", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(enemy, "Dragon");
        enemy.GetType().GetField("combatLevel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(enemy, 50);
        enemy.GetType().GetField("stats", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(enemy, new EquipmentStats(accuracy: 50, defence: 60, strength: 80, attackSpeed: 0));
        enemy.GetType().GetField("attackSpeed", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(enemy, 6);
        enemy.GetType().GetField("maxHealth", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(enemy, 500);
        enemy.GetType().GetField("currentHealth", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(enemy, 500);

        LootTable lootTable = enemy.GetComponent<LootTable>();
        if (lootTable == null)
            lootTable = enemy.gameObject.AddComponent<LootTable>();

        var drops = LootTableTemplates.CreateBasicEnemyDrops();
        foreach (var drop in drops)
            lootTable.AddItemDrop(drop);

        EnemyMovement movement = enemy.GetComponent<EnemyMovement>();
        if (movement == null)
            movement = enemy.gameObject.AddComponent<EnemyMovement>();

        movement.GetType().GetField("wanderRange", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(movement, 10);
        movement.GetType().GetField("aggressionRange", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(movement, 30);
    }

    public void ClearSpawnedEnemies()
    {
        foreach (Enemy enemy in spawnedEnemies)
        {
            if (enemy != null)
                Destroy(enemy.gameObject);
        }

        spawnedEnemies.Clear();

        foreach (var point in spawnPoints)
        {
            point.SetOccupied(false);
        }

        Debug.Log("Cleared all spawned enemies");
    }

    public void AddSpawnPoint(Transform spawnTransform, string name = "Spawn Point")
    {
        spawnPoints.Add(new SpawnPoint { transform = spawnTransform, name = name });
    }

    public void RemoveSpawnPoint(Transform spawnTransform)
    {
        spawnPoints.RemoveAll(sp => sp.transform == spawnTransform);
    }

    public int GetSpawnedEnemyCount()
    {
        spawnedEnemies.RemoveAll(e => e == null);
        return spawnedEnemies.Count;
    }

    public int GetAvailableSpawnPoints()
    {
        return spawnPoints.Count(sp => !sp.IsOccupied);
    }

    private void OnEnemyDied(Enemy enemy, SpawnPoint spawnPoint)
    {
        Debug.Log($"{enemy.EnemyName} died at {spawnPoint.name}, respawning in {respawnDelay} seconds...");
        StartCoroutine(RespawnEnemyCoroutine(enemy, spawnPoint));
    }

    private System.Collections.IEnumerator RespawnEnemyCoroutine(Enemy enemy, SpawnPoint spawnPoint)
    {
        yield return new WaitForSeconds(respawnDelay);

        if (enemy == null || spawnPoint == null || spawnPoint.transform == null)
        {
            Debug.LogWarning("EnemySpawner: Cannot respawn enemy - enemy or spawn point is null!");
            yield break;
        }

        enemy.CancelInvoke();

        enemy.gameObject.SetActive(true);

        Vector3 spawnWorldPos = spawnPoint.transform.position;
        spawnWorldPos.y -= 0.5f;
        Vector3Int gridPos = gridManager.WorldToGrid(spawnWorldPos);
        
        Vector3 snappedPos = gridManager.GridToWorld(gridPos);
        snappedPos.y += 0.5f;
        enemy.gameObject.transform.position = snappedPos;

        enemy.Reset();

        spawnPoint.SetOccupied(true);

        Debug.Log($"<color=green>Respawned {enemy.EnemyName} at {spawnPoint.name}</color>");
    }
}
