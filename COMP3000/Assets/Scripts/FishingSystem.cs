using System.Collections;
using UnityEngine;

public class FishingSystem : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SkillSystem skillSystem;
    [SerializeField] private PlayerInventory playerInventory;
    [SerializeField] private GridManager gridManager;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private InventoryUI inventoryUI;

    [Header("Fishing Settings")]
    [SerializeField] private float tickDuration = 0.6f;
    [SerializeField] private int attemptDuration = 4;
    [SerializeField] private float baseSuccessChance = 0.33f;
    [SerializeField] private int fishingRange = 3;
    [SerializeField] private float minTimeBetweenRangeChecks = 1f;

    private bool isFishing = false;
    private Coroutine fishingCoroutine;
    private FishingSpot currentSpot;
    private float fishingStartTime = 0f;

    private FishData[] fishData = new FishData[]
    {
        new FishData("Raw Shrimp", 1, "raw_shrimp", 100),
        new FishData("Raw Anchovy", 15, "raw_anchovy", 100),
        new FishData("Raw Herring", 30, "raw_herring", 100),
        new FishData("Raw Salmon", 45, "raw_salmon", 100),
        new FishData("Raw Tuna", 60, "raw_tuna", 100),
        new FishData("Raw Swordfish", 75, "raw_swordfish", 100)
    };

    public bool IsFishing => isFishing;

    void Start()
    {
        if (skillSystem == null)
            skillSystem = FindFirstObjectByType<SkillSystem>();

        if (playerInventory == null)
            playerInventory = FindFirstObjectByType<PlayerInventory>();

        if (gridManager == null)
            gridManager = FindFirstObjectByType<GridManager>();

        if (playerController == null)
            playerController = FindFirstObjectByType<PlayerController>();

        if (skillSystem == null)
            Debug.LogWarning("FishingSystem: SkillSystem not found!");
    }

    // Public method to update GridManager reference when scenes change
    public void UpdateGridManager()
    {
        gridManager = FindFirstObjectByType<GridManager>();
        Debug.Log($"FishingSystem GridManager updated: {gridManager != null}");
    }

    void Update()
    {
        if (isFishing && currentSpot != null && gridManager != null && playerController != null)
        {
            // Only check range after minimum time has passed to allow player to settle into position
            if (Time.time - fishingStartTime >= minTimeBetweenRangeChecks)
            {
                Vector3Int playerPos = playerController.CurrentGridPosition;
                Vector3 adjustedSpotPos = currentSpot.transform.position - new Vector3(0.5f, 0.5f, 0);
                Vector3Int spotPos = gridManager.WorldToGrid(adjustedSpotPos);
                int distanceToSpot = GetManhattanDistance(playerPos, spotPos);

                if (distanceToSpot > fishingRange)
                {
                    Debug.Log($"Walked too far from fishing spot! Distance: {distanceToSpot}, Range: {fishingRange}. Stopping fishing.");
                    StopFishing();
                }
            }
        }
    }

    private int GetManhattanDistance(Vector3Int pos1, Vector3Int pos2)
    {
        return Mathf.Abs(pos1.x - pos2.x) + Mathf.Abs(pos1.y - pos2.y);
    }

    public void StartFishing(int fishIndex, FishingSpot spot = null)
    {
        if (fishIndex < 0 || fishIndex >= fishData.Length)
        {
            Debug.LogError($"FishingSystem: Invalid fish index {fishIndex}");
            return;
        }

        if (isFishing)
        {
            Debug.LogWarning("Already fishing!");
            return;
        }

        if (playerInventory != null && playerInventory.GetUsedSlotCount() >= playerInventory.MaxSlots)
        {
            Debug.LogWarning("<color=red>Inventory is full! Stop fishing.</color>");
            return;
        }

        FishData fish = fishData[fishIndex];

        if (skillSystem != null && skillSystem.FishingSkill.Level < fish.requiredLevel)
        {
            Debug.LogWarning($"You need Fishing level {fish.requiredLevel} to catch {fish.name}. You have {skillSystem.FishingSkill.Level}");
            return;
        }

        currentSpot = spot;
        fishingStartTime = Time.time;
        Debug.Log($"<color=cyan>Starting to fish for {fish.name}...</color>");

        if (fishingCoroutine != null)
            StopCoroutine(fishingCoroutine);

        fishingCoroutine = StartCoroutine(FishingRoutine(fish));
    }

    public void StopFishing()
    {
        if (fishingCoroutine != null)
        {
            StopCoroutine(fishingCoroutine);
            fishingCoroutine = null;
        }
        isFishing = false;
        Debug.Log("Stopped fishing.");
    }

    private IEnumerator FishingRoutine(FishData fish)
    {
        isFishing = true;

        while (isFishing)
        {
            for (int tick = 0; tick < attemptDuration; tick++)
            {
                yield return new WaitForSeconds(tickDuration);
            }

            float successChance = CalculateSuccessChance(fish);
            float roll = Random.Range(0f, 1f);

            if (roll <= successChance)
            {
                Debug.Log($"<color=green>Successfully caught {fish.name}! (XP: {fish.experience})</color>");
                AddFishToInventory(fish);

                if (currentSpot != null)
                {
                    currentSpot.OnFishCaught();
                }

                if (skillSystem != null)
                {
                    skillSystem.OnFishCaught(fish.experience);
                }
            }
            else
            {
                Debug.Log($"<color=yellow>Failed to catch {fish.name}...</color>");
            }
        }
    }

    private float CalculateSuccessChance(FishData fish)
    {
        if (skillSystem == null)
            return baseSuccessChance;

        int playerLevel = skillSystem.FishingSkill.Level;
        int levelDifference = playerLevel - fish.requiredLevel;

        float chance = baseSuccessChance + (levelDifference * 0.01f);

        return Mathf.Clamp(chance, 0.01f, 0.99f);
    }

    private void AddFishToInventory(FishData fish)
    {
        if (playerInventory != null)
        {
            bool added = playerInventory.AddItem(fish.itemId, 1);
            if (!added)
            {
                Debug.LogWarning("<color=red>Could not add fish to inventory - inventory full!</color>");
                StopFishing();
            }
            else
            {
                // Update UI immediately after adding item
                if (inventoryUI == null)
                    inventoryUI = FindFirstObjectByType<InventoryUI>();
                
                if (inventoryUI != null)
                    inventoryUI.RefreshAllSlots();
            }
        }
        else
        {
            Debug.Log($"[FishingSystem] Would add to inventory: {fish.name}");
        }
    }

    public FishData[] GetAllFishData()
    {
        return fishData;
    }

    public class FishData
    {
        public string name;
        public int requiredLevel;
        public string itemId;
        public int experience;

        public FishData(string name, int requiredLevel, string itemId, int experience)
        {
            this.name = name;
            this.requiredLevel = requiredLevel;
            this.itemId = itemId;
            this.experience = experience;
        }
    }
}
