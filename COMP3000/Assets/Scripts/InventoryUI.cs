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

    void Awake()
    {
        if (playerInventory == null)
            playerInventory = FindFirstObjectByType<PlayerInventory>();

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        CreateInventorySlots();
    }

    void Start()
    {
        if (!showOnStart)
            Hide();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
            Toggle();
    }

    void OnEnable()
    {
        if (playerInventory != null)
        {
            playerInventory.OnSlotUpdated += UpdateSlot;
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

    // 👇 Try get component
    InventorySlotUI slotUI = slotObj.GetComponent<InventorySlotUI>();

    // 👇 If missing, add it automatically
    if (slotUI == null)
    {
        slotUI = slotObj.AddComponent<InventorySlotUI>();
    }

    // 👇 Now it's guaranteed to exist
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
