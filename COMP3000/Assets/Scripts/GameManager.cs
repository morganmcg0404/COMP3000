using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Player Systems")]
    [SerializeField] private SkillSystem skillSystem;
    [SerializeField] private PlayerInventory playerInventory;
    [SerializeField] private PlayerEquipment playerEquipment;
    [SerializeField] private CombatSystem combatSystem;

    [Header("UI Systems")]
    [SerializeField] private InventoryUI inventoryUI;
    [SerializeField] private EquipmentUI equipmentUI;
    [SerializeField] private SkillMenuUI skillMenuUI;

    private void Awake()
    {
        // Singleton pattern - ensure only one GameManager exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("GameManager created and marked as DontDestroyOnLoad");

            // Create persistent player systems as children
            CreatePersistentSystems();
        }
        else
        {
            Debug.Log("Duplicate GameManager found, destroying it");
            Destroy(gameObject);
            return;
        }

        // Subscribe to scene loading events
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void CreatePersistentSystems()
    {
        // Create SkillSystem if it doesn't exist
        if (skillSystem == null)
        {
            GameObject skillSystemObj = new GameObject("SkillSystem");
            skillSystemObj.transform.SetParent(transform);
            skillSystem = skillSystemObj.AddComponent<SkillSystem>();
            Debug.Log("Created persistent SkillSystem");
        }

        // Create PlayerInventory if it doesn't exist
        if (playerInventory == null)
        {
            GameObject inventoryObj = new GameObject("PlayerInventory");
            inventoryObj.transform.SetParent(transform);
            playerInventory = inventoryObj.AddComponent<PlayerInventory>();
            Debug.Log("Created persistent PlayerInventory");
        }

        // Create PlayerEquipment if it doesn't exist
        if (playerEquipment == null)
        {
            GameObject equipmentObj = new GameObject("PlayerEquipment");
            equipmentObj.transform.SetParent(transform);
            playerEquipment = equipmentObj.AddComponent<PlayerEquipment>();
            Debug.Log("Created persistent PlayerEquipment");
        }

        // Create CombatSystem if it doesn't exist
        if (combatSystem == null)
        {
            GameObject combatObj = new GameObject("CombatSystem");
            combatObj.transform.SetParent(transform);
            combatSystem = combatObj.AddComponent<CombatSystem>();
            Debug.Log("Created persistent CombatSystem");
        }
    }

    private void FindPlayerSystems()
    {
        // Try to find existing systems in the scene (for backwards compatibility)
        if (skillSystem == null)
            skillSystem = FindFirstObjectByType<SkillSystem>();

        if (playerInventory == null)
            playerInventory = FindFirstObjectByType<PlayerInventory>();

        if (playerEquipment == null)
            playerEquipment = FindFirstObjectByType<PlayerEquipment>();

        if (combatSystem == null)
            combatSystem = FindFirstObjectByType<CombatSystem>();

        Debug.Log($"GameManager systems: SkillSystem={skillSystem != null}, Inventory={playerInventory != null}, Equipment={playerEquipment != null}, Combat={combatSystem != null}");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"Scene loaded: {scene.name}. Reconnecting player systems...");

        // Disable/destroy duplicate systems in the scene
        DisableDuplicateSystems();

        // Reconnect UI systems to persistent player systems
        ConnectUISystems();

        // Update GridManager references in persistent systems
        UpdateSystemReferences();

        // Ensure player systems are properly initialized
        InitializePlayerSystems();
    }

    private void UpdateSystemReferences()
    {
        // Update GridManager reference in CombatSystem
        if (combatSystem != null)
        {
            combatSystem.UpdateGridManager();
        }

        // Update GridManager reference in PlayerController
        PlayerController playerController = FindFirstObjectByType<PlayerController>();
        if (playerController != null)
        {
            playerController.UpdateGridManager();
            Debug.Log("PlayerController GridManager updated");
        }

        // Update GridManager reference in other systems
        FishingSystem fishingSystem = FindFirstObjectByType<FishingSystem>();
        if (fishingSystem != null)
        {
            fishingSystem.UpdateGridManager();
        }

        MiningSystem miningSystem = FindFirstObjectByType<MiningSystem>();
        if (miningSystem != null)
        {
            miningSystem.UpdateGridManager();
        }

        EnemyMovement[] enemyMovements = FindObjectsByType<EnemyMovement>(FindObjectsSortMode.None);
        foreach (EnemyMovement enemyMovement in enemyMovements)
        {
            enemyMovement.UpdateGridManager();
        }

        Debug.Log("All system GridManager references updated");
    }

    private void DisableDuplicateSystems()
    {
        // Find and disable any duplicate systems in the scene
        SkillSystem[] skillSystems = FindObjectsByType<SkillSystem>(FindObjectsSortMode.None);
        PlayerInventory[] inventories = FindObjectsByType<PlayerInventory>(FindObjectsSortMode.None);
        PlayerEquipment[] equipments = FindObjectsByType<PlayerEquipment>(FindObjectsSortMode.None);
        CombatSystem[] combatSystems = FindObjectsByType<CombatSystem>(FindObjectsSortMode.None);

        foreach (SkillSystem sys in skillSystems)
        {
            if (sys != skillSystem && sys.transform.parent != transform)
            {
                sys.gameObject.SetActive(false);
                Debug.Log("Disabled duplicate SkillSystem in scene");
            }
        }

        foreach (PlayerInventory inv in inventories)
        {
            if (inv != playerInventory && inv.transform.parent != transform)
            {
                inv.gameObject.SetActive(false);
                Debug.Log("Disabled duplicate PlayerInventory in scene");
            }
        }

        foreach (PlayerEquipment eq in equipments)
        {
            if (eq != playerEquipment && eq.transform.parent != transform)
            {
                eq.gameObject.SetActive(false);
                Debug.Log("Disabled duplicate PlayerEquipment in scene");
            }
        }

        foreach (CombatSystem cs in combatSystems)
        {
            if (cs != combatSystem && cs.transform.parent != transform)
            {
                cs.gameObject.SetActive(false);
                Debug.Log("Disabled duplicate CombatSystem in scene");
            }
        }
    }

    private void ConnectUISystems()
    {
        // Find UI systems in the new scene
        if (inventoryUI == null)
            inventoryUI = FindFirstObjectByType<InventoryUI>();

        if (equipmentUI == null)
            equipmentUI = FindFirstObjectByType<EquipmentUI>();

        if (skillMenuUI == null)
            skillMenuUI = FindFirstObjectByType<SkillMenuUI>();

        // Connect them to the persistent player systems
        if (inventoryUI != null && playerInventory != null)
        {
            inventoryUI.SetPlayerInventory(playerInventory);
            Debug.Log("Connected InventoryUI to persistent PlayerInventory");
        }

        if (equipmentUI != null && playerEquipment != null)
        {
            equipmentUI.SetPlayerEquipment(playerEquipment);
            Debug.Log("Connected EquipmentUI to persistent PlayerEquipment");
        }

        if (skillMenuUI != null && skillSystem != null)
        {
            skillMenuUI.SetSkillSystem(skillSystem);
            Debug.Log("Connected SkillMenuUI to persistent SkillSystem");
        }
    }

    private void InitializePlayerSystems()
    {
        // Ensure player systems are ready
        if (playerInventory != null)
        {
            playerInventory.InitializeInventory();
            Debug.Log("PlayerInventory initialized");
        }

        if (playerEquipment != null)
        {
            playerEquipment.InitializeEquipment();
            Debug.Log("PlayerEquipment initialized");
        }

        if (skillSystem != null)
        {
            skillSystem.InitializeSkills();
            Debug.Log("SkillSystem initialized");
        }

        if (combatSystem != null)
        {
            combatSystem.InitializeCombat();
            Debug.Log("CombatSystem initialized");
        }
    }

    // Public accessors for other scripts
    public SkillSystem SkillSystem => skillSystem;
    public PlayerInventory PlayerInventory => playerInventory;
    public PlayerEquipment PlayerEquipment => playerEquipment;
    public CombatSystem CombatSystem => combatSystem;

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}