using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInventory playerInventory;
    [SerializeField] private Transform slotContainer;
    [SerializeField] private GameObject slotPrefab;

    [Header("UI Settings")]
    [SerializeField] private bool showOnStart = true;

    private InventorySlotUI[] slotUIs;
    private CanvasGroup canvasGroup;
    private bool slotsCreated = false;

    void Awake()
    {
        // Don't auto-find PlayerInventory here - let GameManager set it
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    // Public method for GameManager to set the persistent PlayerInventory
    public void SetPlayerInventory(PlayerInventory inventory)
    {
        playerInventory = inventory;
        Debug.Log("InventoryUI received PlayerInventory from GameManager");

        // Now that we have the inventory, try to create slots
        if (playerInventory != null && playerInventory.Slots != null && playerInventory.Slots.Count > 0)
        {
            CreateInventorySlots();
        }
    }

    void Start()
    {
        Debug.Log("<color=cyan>[InventoryUI] Start() called</color>");

        // Only try to find PlayerInventory if not already set by GameManager
        if (playerInventory == null)
        {
            playerInventory = FindFirstObjectByType<PlayerInventory>();
            Debug.Log("<color=cyan>[InventoryUI] Found PlayerInventory via FindFirstObjectByType</color>");
        }

        // Create slots in Start() after all Awake() calls are done
        if (!slotsCreated)
        {
            Debug.Log("<color=cyan>[InventoryUI] Creating slots in Start()</color>");
            CreateInventorySlots();
            slotsCreated = true;
        }

        // Subscribe to inventory events
        if (playerInventory != null)
        {
            Debug.Log("<color=cyan>[InventoryUI] Subscribing to inventory events</color>");
            playerInventory.OnSlotUpdated += UpdateSlot;
            RefreshAllSlots();
        }
        else
        {
            Debug.LogError("<color=red>[InventoryUI] PlayerInventory is NULL in Start()!</color>");
        }

        if (!showOnStart)
            Hide();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
            Toggle();

        // Check if we need to create slots (PlayerInventory became ready after OnEnable)
        if (!slotsCreated && playerInventory != null && playerInventory.Slots != null && playerInventory.Slots.Count > 0)
        {
            Debug.Log("<color=cyan>[InventoryUI] PlayerInventory became ready in Update(), creating slots now</color>");
            CreateInventorySlots();
            slotsCreated = true;
            RefreshAllSlots();
        }
    }

    void OnEnable()
    {
        // Check if PlayerInventory is ready (has slots initialized)
        if (playerInventory != null && playerInventory.Slots != null && playerInventory.Slots.Count > 0)
        {
            // Ensure slots are created if they haven't been yet
            if (!slotsCreated)
            {
                CreateInventorySlots();
                slotsCreated = true;
            }

            // Refresh UI when inventory panel is opened
            RefreshAllSlots();
        }
        else
        {
            // Don't create slots yet - wait for PlayerInventory to initialize
        }
    }

    void OnDisable()
    {
        if (playerInventory != null)
        {
            playerInventory.OnSlotUpdated -= UpdateSlot;
        }
    }

    private void CreateInventorySlots()
    {
        if (playerInventory == null)
        {
            Debug.LogError("PlayerInventory not found!");
            return;
        }

        foreach (Transform child in slotContainer)
        {
            Destroy(child.gameObject);
        }

        slotUIs = new InventorySlotUI[playerInventory.MaxSlots];

        for (int i = 0; i < playerInventory.MaxSlots; i++)
        {
            GameObject slotObj = Instantiate(slotPrefab, slotContainer);

            InventorySlotUI slotUI = slotObj.GetComponent<InventorySlotUI>();

            if (slotUI == null)
            {
                slotUI = slotObj.AddComponent<InventorySlotUI>();
            }

            slotUI.Initialize(playerInventory.GetSlot(i), i);
            slotUIs[i] = slotUI;
        }

        RefreshAllSlots();
    }

    private void UpdateSlot(int slotIndex, InventorySlot slot)
    {
        if (slotIndex >= 0 && slotIndex < slotUIs.Length)
        {
            slotUIs[slotIndex].Refresh();
        }
    }

    public void RefreshAllSlots()
    {
        if (slotUIs != null)
        {
            foreach (InventorySlotUI slotUI in slotUIs)
            {
                if (slotUI != null)
                    slotUI.Refresh();
            }
        }
    }

    public void Show()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
    }

    public void Hide()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }

    public void Toggle()
    {
        if (canvasGroup != null && canvasGroup.alpha > 0.5f)
            Hide();
        else
            Show();
    }
}
