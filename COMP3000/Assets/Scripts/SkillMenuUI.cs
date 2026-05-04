using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillMenuUI : MonoBehaviour
{
    [SerializeField] private SkillSystem skillSystem;
    [SerializeField] private Transform skillGrid;
    [SerializeField] private GameObject tooltipPrefab;

    void Start()
    {
        if (skillSystem == null)
            skillSystem = FindFirstObjectByType<SkillSystem>();

        if (skillSystem == null)
        {
            Debug.LogError("SkillMenuUI: SkillSystem not found!");
            return;
        }

        if (skillGrid == null)
            skillGrid = GetComponent<Transform>();

        SetupGridLayout();

        SkillTooltip tooltip = null;
        if (tooltipPrefab != null)
        {
            Canvas mainCanvas = FindFirstObjectByType<Canvas>();
            if (mainCanvas != null)
            {
                GameObject tooltipObj = Instantiate(tooltipPrefab, mainCanvas.transform);
                tooltip = tooltipObj.GetComponent<SkillTooltip>();
                Debug.Log("Tooltip prefab instantiated in canvas");
            }
            else
            {
                Debug.LogError("SkillMenuUI: No Canvas found for tooltip!");
            }
        }
        else
        {
            Debug.LogWarning("SkillMenuUI: No tooltip prefab assigned!");
        }

        CreateSkillSlots(tooltip);

        Debug.Log("SkillMenuUI initialized successfully!");
    }

    private void SetupGridLayout()
    {
        GridLayoutGroup gridLayout = skillGrid.GetComponent<GridLayoutGroup>();
        if (gridLayout == null)
            gridLayout = skillGrid.gameObject.AddComponent<GridLayoutGroup>();

        gridLayout.constraintCount = 2;
        gridLayout.cellSize = new Vector2(115f, 60f);
        gridLayout.spacing = new Vector2(5f, 5f);
        gridLayout.padding = new RectOffset(10, 10, 10, 10);

        Debug.Log("Grid layout configured");
    }

    private void CreateSkillSlots(SkillTooltip tooltip)
    {
        Skill[] skillsToDisplay = new Skill[]
        {
            skillSystem.CombatSkill,
            skillSystem.HealthSkill,
            skillSystem.MiningSkill,
            skillSystem.FishingSkill,
            skillSystem.SmithingSkill,
            skillSystem.CookingSkill
        };

        foreach (Skill skill in skillsToDisplay)
        {
            GameObject slotObj = new GameObject($"{skill.SkillName} Slot");
            slotObj.transform.SetParent(skillGrid, false);

            RectTransform slotRect = slotObj.AddComponent<RectTransform>();
            slotRect.sizeDelta = new Vector2(115f, 60f);

            LayoutElement layoutElement = slotObj.AddComponent<LayoutElement>();
            layoutElement.preferredWidth = 115f;
            layoutElement.preferredHeight = 60f;

            Image bgImage = slotObj.AddComponent<Image>();
            bgImage.color = new Color(0x59 / 255f, 0x42 / 255f, 0x39 / 255f, 1f);

            SkillSlotUI slotUI = slotObj.AddComponent<SkillSlotUI>();

            GameObject nameTextObj = new GameObject("SkillName");
            nameTextObj.transform.SetParent(slotObj.transform, false);
            RectTransform nameRect = nameTextObj.AddComponent<RectTransform>();
            nameRect.anchorMin = Vector2.zero;
            nameRect.anchorMax = new Vector2(1f, 0.6f);
            nameRect.offsetMin = new Vector2(0, 0);
            nameRect.offsetMax = new Vector2(0, 25);

            TextMeshProUGUI nameText = nameTextObj.AddComponent<TextMeshProUGUI>();
            nameText.text = skill.SkillName;
            nameText.alignment = TextAlignmentOptions.Center;
            nameText.fontSize = 20;

            GameObject levelTextObj = new GameObject("SkillLevel");
            levelTextObj.transform.SetParent(slotObj.transform, false);
            RectTransform levelRect = levelTextObj.AddComponent<RectTransform>();
            levelRect.anchorMin = new Vector2(0f, 0f);
            levelRect.anchorMax = new Vector2(1f, 0.4f);
            levelRect.offsetMin = Vector2.zero;
            levelRect.offsetMax = Vector2.zero;

            TextMeshProUGUI levelText = levelTextObj.AddComponent<TextMeshProUGUI>();
            levelText.text = $"Lvl {skill.Level}";
            levelText.alignment = TextAlignmentOptions.Center;
            levelText.fontSize = 16;
            levelText.color = new Color(1f, 1f, 0.5f);

            slotUI.Initialize(skill, tooltip);

            Debug.Log($"Created skill slot for {skill.SkillName}");
        }
    }
}
