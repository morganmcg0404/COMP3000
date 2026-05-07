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
        if (slotButton == null)
            slotButton = gameObject.AddComponent<Button>();

        // Auto-create slot background if not assigned
        if (slotBackground == null)
        {
            slotBackground = GetComponent<Image>();
            if (slotBackground == null)
                slotBackground = gameObject.AddComponent<Image>();
            slotBackground.color = emptySlotColor;
        }

        // Auto-create item icon if not assigned
        if (itemIcon == null)
        {
            GameObject iconObj = new GameObject("ItemIcon");
            iconObj.transform.SetParent(transform, false);
            itemIcon = iconObj.AddComponent<Image>();

            RectTransform iconRect = itemIcon.rectTransform;
            iconRect.anchorMin = Vector2.zero;
            iconRect.anchorMax = Vector2.one;
            iconRect.offsetMin = Vector2.zero;
            iconRect.offsetMax = Vector2.zero;
        }

        // Auto-create quantity text if not assigned
        if (quantityText == null)
        {
            GameObject qtyObj = new GameObject("QuantityText");
            qtyObj.transform.SetParent(transform, false);
            quantityText = qtyObj.AddComponent<TextMeshProUGUI>();

            RectTransform qtyRect = quantityText.rectTransform;
            qtyRect.anchorMin = new Vector2(1, 1);
            qtyRect.anchorMax = new Vector2(1, 1);
            qtyRect.anchoredPosition = new Vector2(-2, -2);
            qtyRect.sizeDelta = new Vector2(20, 20);

            quantityText.alignment = TextAlignmentOptions.BottomRight;
            quantityText.enableAutoSizing = true;
            quantityText.fontSizeMin = 4;
            quantityText.fontSizeMax = 8;
            quantityText.color = Color.black;
            quantityText.fontStyle = FontStyles.Bold;
            quantityText.raycastTarget = false;
        }

        // Auto-create item name text if not assigned
        if (itemNameText == null)
        {
            GameObject textObj = new GameObject("ItemNameText");
            textObj.transform.SetParent(transform, false);

            itemNameText = textObj.AddComponent<TextMeshProUGUI>();

            // Stretch to fit the slot
            RectTransform rect = itemNameText.rectTransform;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = new Vector2(5, 5);
            rect.offsetMax = new Vector2(-5, -5);

            // Text settings for visibility
            itemNameText.alignment = TextAlignmentOptions.Bottom;
            itemNameText.enableAutoSizing = true;
            itemNameText.fontSizeMin = 6;
            itemNameText.fontSizeMax = 16;
            itemNameText.text = "";
            itemNameText.color = Color.black;
            itemNameText.fontStyle = FontStyles.Bold;
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

        if (itemNameText == null)
        {
            Debug.LogError($"Item name text is null on slot {slotIndex}!");
            return;
        }

        if (slot.IsEmpty)
        {
            itemIcon.sprite = null;
            itemIcon.enabled = false;
            quantityText.text = "";
            itemNameText.text = "";
            slotBackground.color = emptySlotColor;
        }
        else
        {
            if (slot.Item == null)
            {
                Debug.LogError($"Slot {slotIndex} has item but Item is NULL!");
                return;
            }

            itemIcon.sprite = slot.Item.Icon;
            itemIcon.enabled = true;
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
