using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlotUI : MonoBehaviour
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI quantityText;
    [SerializeField] private Image slotBackground;
    [SerializeField] private Color emptySlotColor = Color.gray;
    [SerializeField] private Color filledSlotColor = Color.white;
    [SerializeField] private TextMeshProUGUI itemNameText;

    private InventorySlot slot;
    private int slotIndex;
    private Button slotButton;

    void Awake()
{
    slotButton = GetComponent<Button>();

    // 👇 Auto-create TMP text if not assigned
    if (itemNameText == null)
    {
        GameObject textObj = new GameObject("ItemNameText");
        textObj.transform.SetParent(transform);

        itemNameText = textObj.AddComponent<TextMeshProUGUI>();

        // Stretch to fit the slot
        RectTransform rect = itemNameText.rectTransform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        // Text settings
        itemNameText.alignment = TextAlignmentOptions.Center;
        itemNameText.enableAutoSizing = true;
        itemNameText.fontSizeMin = 8;
        itemNameText.fontSizeMax = 24;
        itemNameText.text = "";
        itemNameText.raycastTarget = false;
    }
}

    public void Initialize(InventorySlot inventorySlot, int index)
    {
        slot = inventorySlot;
        slotIndex = index;

        if (slot != null)
        {
            slot.OnSlotChanged += Refresh;
        }

        Refresh();
    }

    public void Refresh()
{
    if (slot == null)
        return;

    if (slot.IsEmpty)
    {
        itemIcon.sprite = null;
        itemIcon.enabled = false;

        quantityText.text = "";
        itemNameText.text = ""; // 👈 clear name

        slotBackground.color = emptySlotColor;
    }
    else
    {
        itemIcon.sprite = slot.Item.Icon;
        itemIcon.enabled = true;

        // 👇 SHOW ITEM NAME
        itemNameText.text = slot.Item.ItemName;

        if (slot.Item.IsStackable && slot.Quantity > 1)
        {
            quantityText.text = slot.Quantity.ToString();
        }
        else
        {
            quantityText.text = "";
        }

        slotBackground.color = filledSlotColor;
    }
}

    public void OnSlotClicked()
    {
        if (slot != null && !slot.IsEmpty)
        {
            Debug.Log($"<color=cyan>Clicked: {slot.Item.ItemName} x{slot.Quantity}</color>");
        }
    }

    void OnDestroy()
    {
        if (slot != null)
        {
            slot.OnSlotChanged -= Refresh;
        }
    }
}
