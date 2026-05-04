using System.Collections;
using UnityEngine;

public class MiningSystem : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SkillSystem skillSystem;
    [SerializeField] private PlayerInventory playerInventory;
    [SerializeField] private GridManager gridManager;
    [SerializeField] private PlayerController playerController;

    [Header("Mining Settings")]
    [SerializeField] private float tickDuration = 0.6f;
    [SerializeField] private int attemptDuration = 4;
    [SerializeField] private float baseSuccessChance = 0.33f;
    [SerializeField] private int miningRange = 2;

    private bool isMining = false;
    private Coroutine miningCoroutine;
    private OreRock currentRock;

    private OreData[] oreData = new OreData[]
    {
        new OreData("Copper Ore", 1, "copper_ore", 10),
        new OreData("Tin Ore", 15, "tin_ore", 20),
        new OreData("Iron Ore", 30, "iron_ore", 30),
        new OreData("Gold Ore", 45, "gold_ore", 40),
        new OreData("Mithril Ore", 60, "mithril_ore", 50),
        new OreData("Adamantite Ore", 75, "adamantite_ore", 60)
    };

    public bool IsMining => isMining;

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
            Debug.LogWarning("MiningSystem: SkillSystem not found!");
    }

    void Update()
    {
        if (isMining && currentRock != null && gridManager != null && playerController != null)
        {
            Vector3Int playerPos = playerController.CurrentGridPosition;
            Vector3Int orePos = gridManager.WorldToGrid(currentRock.transform.position);
            int distanceToOre = GetManhattanDistance(playerPos, orePos);

            if (distanceToOre > miningRange)
            {
                Debug.Log("Walked too far from ore! Stopping mining.");
                StopMining();
            }
        }
    }

    private int GetManhattanDistance(Vector3Int pos1, Vector3Int pos2)
    {
        return Mathf.Abs(pos1.x - pos2.x) + Mathf.Abs(pos1.y - pos2.y);
    }

    public void StartMining(int oreIndex, OreRock rock = null)
    {
        if (oreIndex < 0 || oreIndex >= oreData.Length)
        {
            Debug.LogError($"MiningSystem: Invalid ore index {oreIndex}");
            return;
        }

        if (isMining)
        {
            Debug.LogWarning("Already mining!");
            return;
        }

        if (playerInventory != null && playerInventory.GetUsedSlotCount() >= playerInventory.MaxSlots)
        {
            Debug.LogWarning("<color=red>Inventory is full! Stop mining.</color>");
            return;
        }

        OreData ore = oreData[oreIndex];

        if (skillSystem != null && skillSystem.MiningSkill.Level < ore.requiredLevel)
        {
            Debug.LogWarning($"You need Mining level {ore.requiredLevel} to mine {ore.name}. You have {skillSystem.MiningSkill.Level}");
            return;
        }

        currentRock = rock;
        Debug.Log($"<color=cyan>Starting to mine {ore.name}...</color>");

        if (miningCoroutine != null)
            StopCoroutine(miningCoroutine);

        miningCoroutine = StartCoroutine(MiningRoutine(ore));
    }

    public void StopMining()
    {
        if (miningCoroutine != null)
        {
            StopCoroutine(miningCoroutine);
            miningCoroutine = null;
        }
        isMining = false;
        Debug.Log("Stopped mining.");
    }

    private IEnumerator MiningRoutine(OreData ore)
    {
        isMining = true;

        while (isMining)
        {
            for (int tick = 0; tick < attemptDuration; tick++)
            {
                yield return new WaitForSeconds(tickDuration);
            }

            float successChance = CalculateSuccessChance(ore);
            float roll = Random.Range(0f, 1f);

            if (roll <= successChance)
            {
                Debug.Log($"<color=green>Successfully mined {ore.name}! (XP: {ore.experience})</color>");
                AddOreToInventory(ore);

                if (currentRock != null)
                {
                    currentRock.OnOreMined();
                }

                if (skillSystem != null)
                {
                    skillSystem.OnOreMined(ore.experience);
                }
            }
            else
            {
                Debug.Log($"<color=yellow>Failed to mine {ore.name}...</color>");
            }
        }
    }

    private float CalculateSuccessChance(OreData ore)
    {
        if (skillSystem == null)
            return baseSuccessChance;

        int playerLevel = skillSystem.MiningSkill.Level;
        int levelDifference = playerLevel - ore.requiredLevel;

        float chance = baseSuccessChance + (levelDifference * 0.01f);

        return Mathf.Clamp(chance, 0.01f, 0.99f);
    }

    private void AddOreToInventory(OreData ore)
    {
        if (playerInventory != null)
        {
            bool added = playerInventory.AddItem(ore.itemId, 1);
            if (!added)
            {
                Debug.LogWarning("<color=red>Could not add ore to inventory - inventory full!</color>");
                StopMining();
            }
        }
        else
        {
            Debug.Log($"[Would add to inventory: {ore.name}]");
        }
    }

    public OreData[] GetAllOreData()
    {
        return oreData;
    }

    public class OreData
    {
        public string name;
        public int requiredLevel;
        public string itemId;
        public int experience;

        public OreData(string name, int requiredLevel, string itemId, int experience)
        {
            this.name = name;
            this.requiredLevel = requiredLevel;
            this.itemId = itemId;
            this.experience = experience;
        }
    }
}
