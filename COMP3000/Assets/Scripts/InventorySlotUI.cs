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

    private InventorySlot slot;
    private int slotIndex;
    private Button slotButton;

    void Awake()
    {
        slotButton = GetComponent<Button>();
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
            slotBackground.color = emptySlotColor;
        }
        else
        {
            itemIcon.sprite = slot.Item.Icon;
            itemIcon.enabled = true;

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
