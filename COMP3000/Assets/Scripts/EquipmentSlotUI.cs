using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EquipmentSlotUI : MonoBehaviour
{
    private EquipmentSlot slotType;
    private string displayName;
    private PlayerEquipment playerEquipment;
    private EquipmentItem currentItem;
    
    private Image backgroundImage;
    private TextMeshProUGUI itemNameText;
    private bool isHovering = false;

    private Color defaultColor = new Color(0.2f, 0.2f, 0.2f, 1f);
    private Color hoverColor = new Color(0.3f, 0.3f, 0.3f, 1f);

    public void Initialize(EquipmentSlot slot, string name, PlayerEquipment equipment)
    {
        slotType = slot;
        displayName = name;
        playerEquipment = equipment;

        backgroundImage = GetComponent<Image>();

        Transform textTransform = transform.Find("ItemNameText");
        if (textTransform == null)
        {
            GameObject textObj = new GameObject("ItemNameText");
            textObj.transform.SetParent(transform, false);
            
            RectTransform textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            itemNameText = textObj.AddComponent<TextMeshProUGUI>();
            itemNameText.text = displayName;
            itemNameText.fontSize = 14;
            itemNameText.alignment = TextAlignmentOptions.Center;
            itemNameText.color = Color.white;
        }
        else
        {
            itemNameText = textTransform.GetComponent<TextMeshProUGUI>();
        }

        Button button = gameObject.AddComponent<Button>();
        button.targetGraphic = backgroundImage;
        button.onClick.AddListener(OnSlotClicked);

        UpdateDisplay(GetCurrentItem());
    }

    private EquipmentItem GetCurrentItem()
    {
        if (playerEquipment == null)
            return null;

        return slotType switch
        {
            EquipmentSlot.Helmet => playerEquipment.HelmetSlot,
            EquipmentSlot.Cape => playerEquipment.CapeSlot,
            EquipmentSlot.Necklace => playerEquipment.NecklaceSlot,
            EquipmentSlot.Body => playerEquipment.BodySlot,
            EquipmentSlot.Ring => playerEquipment.RingSlot,
            EquipmentSlot.MainHand => playerEquipment.MainHandSlot,
            EquipmentSlot.Hands => playerEquipment.HandsSlot,
            EquipmentSlot.OffHand => playerEquipment.OffHandSlot,
            EquipmentSlot.Legs => playerEquipment.LegsSlot,
            EquipmentSlot.Feet => playerEquipment.FeetSlot,
            _ => null
        };
    }

    public void UpdateDisplay(EquipmentItem item)
    {
        currentItem = item;

        if (currentItem != null)
        {
            itemNameText.text = currentItem.ItemName;
            backgroundImage.color = new Color(0.25f, 0.35f, 0.25f, 1f);
        }
        else
        {
            itemNameText.text = displayName;
            backgroundImage.color = defaultColor;
        }
    }

    private void OnSlotClicked()
    {
        if (currentItem != null)
        {
            Debug.Log($"Unequipping {currentItem.ItemName} from {displayName}");
            playerEquipment.UnequipItem(slotType);
        }
        else
        {
            Debug.Log($"No item equipped in {displayName} slot");
        }
    }

    public void OnPointerEnter()
    {
        isHovering = true;
        if (backgroundImage != null)
        {
            backgroundImage.color = hoverColor;
        }
    }

    public void OnPointerExit()
    {
        isHovering = false;
        if (currentItem != null)
        {
            backgroundImage.color = new Color(0.25f, 0.35f, 0.25f, 1f);
        }
        else
        {
            backgroundImage.color = defaultColor;
        }
    }
}
