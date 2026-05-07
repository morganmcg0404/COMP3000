using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class EquipmentUI : MonoBehaviour
{
    [Header("Equipment Slots")]
    [SerializeField] private Transform slotsContainer;
    [SerializeField] private Button closeButton;

    [Header("Stats Display")]
    [SerializeField] private Transform statsContainer;

    [Header("Settings")]
    [SerializeField] private int slotsPerRow = 3;
    [SerializeField] private Vector2 slotSize = new Vector2(60f, 60f);

    private PlayerEquipment playerEquipment;
    private Dictionary<EquipmentSlot, EquipmentSlotUI> slotUIDictionary = new Dictionary<EquipmentSlot, EquipmentSlotUI>();
    private List<TextMeshProUGUI> statDisplays = new List<TextMeshProUGUI>();

    // Public method for GameManager to set the persistent PlayerEquipment
    public void SetPlayerEquipment(PlayerEquipment equipment)
    {
        playerEquipment = equipment;
        Debug.Log("EquipmentUI received PlayerEquipment from GameManager");

        // Initialize if we already have the equipment
        if (playerEquipment != null)
        {
            SetupEquipmentSlots();
            SetupStatsDisplay();

            playerEquipment.OnEquipmentChanged += HandleEquipmentChanged;
            playerEquipment.OnStatsChanged += HandleStatsChanged;
        }
    }

    void Start()
    {
        // Only find PlayerEquipment if not already set by GameManager
        if (playerEquipment == null)
        {
            playerEquipment = FindFirstObjectByType<PlayerEquipment>();
        }

        if (playerEquipment == null)
        {
            Debug.LogError("EquipmentUI: PlayerEquipment not found!");
            return;
        }

        SetupEquipmentSlots();
        SetupStatsDisplay();

        playerEquipment.OnEquipmentChanged += HandleEquipmentChanged;
        playerEquipment.OnStatsChanged += HandleStatsChanged;

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(() =>
            {
                Debug.Log("Equipment panel closed");
                gameObject.SetActive(false);
            });
        }

        HandleStatsChanged(playerEquipment.TotalStats);
    }

    private void SetupEquipmentSlots()
    {
        if (slotsContainer == null)
        {
            Debug.LogError("EquipmentUI: Slots container not assigned!");
            return;
        }

        GridLayoutGroup gridLayout = slotsContainer.GetComponent<GridLayoutGroup>();
        if (gridLayout == null)
            gridLayout = slotsContainer.gameObject.AddComponent<GridLayoutGroup>();

        gridLayout.constraintCount = slotsPerRow;
        gridLayout.cellSize = slotSize;
        gridLayout.spacing = new Vector2(5f, 5f);
        gridLayout.padding = new RectOffset(5, 5, 5, 5);

        CreateEquipmentSlot(EquipmentSlot.Helmet, "Helmet");
        CreateEquipmentSlot(EquipmentSlot.Cape, "Cape");
        CreateEquipmentSlot(EquipmentSlot.Necklace, "Necklace");
        CreateEquipmentSlot(EquipmentSlot.Body, "Body");
        CreateEquipmentSlot(EquipmentSlot.Ring, "Ring");
        CreateEquipmentSlot(EquipmentSlot.MainHand, "Main Hand");
        CreateEquipmentSlot(EquipmentSlot.Hands, "Hands");
        CreateEquipmentSlot(EquipmentSlot.OffHand, "Off Hand");
        CreateEquipmentSlot(EquipmentSlot.Legs, "Legs");
        CreateEquipmentSlot(EquipmentSlot.Feet, "Feet");

        Debug.Log("EquipmentUI: Equipment slots created");
    }

    private void CreateEquipmentSlot(EquipmentSlot slotType, string displayName)
    {
        GameObject slotObj = new GameObject($"{displayName}_Slot");
        slotObj.transform.SetParent(slotsContainer, false);

        RectTransform slotRect = slotObj.AddComponent<RectTransform>();
        slotRect.sizeDelta = slotSize;

        LayoutElement layoutElement = slotObj.AddComponent<LayoutElement>();
        layoutElement.preferredWidth = slotSize.x;
        layoutElement.preferredHeight = slotSize.y;

        Image bgImage = slotObj.AddComponent<Image>();
        bgImage.color = new Color(0.2f, 0.2f, 0.2f, 1f);

        Outline outline = slotObj.AddComponent<Outline>();
        outline.effectColor = new Color(0.4f, 0.4f, 0.4f, 1f);
        outline.effectDistance = new Vector2(2, 2);

        EquipmentSlotUI slotUI = slotObj.AddComponent<EquipmentSlotUI>();
        slotUI.Initialize(slotType, displayName, playerEquipment);

        slotUIDictionary[slotType] = slotUI;
    }

    private void SetupStatsDisplay()
    {
        if (statsContainer == null)
        {
            Debug.LogError("EquipmentUI: Stats container not assigned!");
            return;
        }

        string[] statNames = { "Accuracy", "Defence", "Strength", "Attack Speed" };

        for (int i = 0; i < statNames.Length; i++)
        {
            GameObject statObj = new GameObject($"{statNames[i]}_Display");
            statObj.transform.SetParent(statsContainer, false);

            RectTransform statRect = statObj.AddComponent<RectTransform>();
            statRect.sizeDelta = new Vector2(200f, 30f);
            statRect.anchoredPosition = new Vector2(0, -i * 35f);

            TextMeshProUGUI statText = statObj.AddComponent<TextMeshProUGUI>();
            statText.text = $"{statNames[i]}: +0";
            statText.fontSize = 24;
            statText.alignment = TextAlignmentOptions.Left;
            statText.color = Color.white;

            statDisplays.Add(statText);
        }
    }

    private void HandleEquipmentChanged(EquipmentSlot slot, EquipmentItem item)
    {
        if (slotUIDictionary.ContainsKey(slot))
        {
            slotUIDictionary[slot].UpdateDisplay(item);
        }
    }

    private void HandleStatsChanged(EquipmentStats stats)
    {
        if (statDisplays.Count >= 4)
        {
            statDisplays[0].text = $"Accuracy: +{stats.Accuracy}";
            statDisplays[1].text = $"Defence: +{stats.Defence}";
            statDisplays[2].text = $"Strength: +{stats.Strength}";
            statDisplays[3].text = $"Attack Speed: +{stats.AttackSpeed}";
        }
    }

    private void OnDestroy()
    {
        if (playerEquipment != null)
        {
            playerEquipment.OnEquipmentChanged -= HandleEquipmentChanged;
            playerEquipment.OnStatsChanged -= HandleStatsChanged;
        }
    }
}
