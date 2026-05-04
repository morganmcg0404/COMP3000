using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class SkillUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("References")]
    [SerializeField] private Skill skill;
    [SerializeField] private SkillSystem skillSystem;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI skillNameText;
    [SerializeField] private TextMeshProUGUI skillLevelText;
    [SerializeField] private Image expBarFill;
    [SerializeField] private GameObject tooltipPanel;
    [SerializeField] private TextMeshProUGUI tooltipText;

    [Header("Settings")]
    [SerializeField] private string skillName = "Combat"; // "Combat" or "Health"
    [SerializeField] private Color expBarColor = Color.yellow;

    private bool isHovering = false;

    void Start()
    {
        // Find the SkillSystem in the scene if not assigned
        if (skillSystem == null)
        {
            skillSystem = FindFirstObjectByType<SkillSystem>();
        }

        if (skillSystem == null)
        {
            Debug.LogError("SkillUI: SkillSystem not found! Make sure there's a SkillSystem component in the scene.");
            return;
        }

        // Get the skill reference
        skill = skillSystem.GetSkill(skillName);

        if (skill == null)
        {
            Debug.LogError($"SkillUI: Could not find skill '{skillName}'");
            return;
        }

        // Subscribe to skill events
        skill.OnLevelUp += OnSkillLevelUp;
        skill.OnExpGained += OnSkillExpGained;

        // Set exp bar color
        if (expBarFill != null)
        {
            expBarFill.color = expBarColor;
        }

        // Hide tooltip initially
        if (tooltipPanel != null)
        {
            tooltipPanel.SetActive(false);
        }

        // Initial UI update
        UpdateUI();
    }

    void OnDestroy()
    {
        // Unsubscribe from events
        if (skill != null)
        {
            skill.OnLevelUp -= OnSkillLevelUp;
            skill.OnExpGained -= OnSkillExpGained;
        }
    }

    void Update()
    {
        // Update tooltip position if hovering
        if (isHovering && tooltipPanel != null && tooltipPanel.activeSelf)
        {
            UpdateTooltipPosition();
        }
    }

    private void UpdateUI()
    {
        if (skill == null) return;

        // Update skill name
        if (skillNameText != null)
        {
            skillNameText.text = skill.SkillName;
        }

        // Update skill level
        if (skillLevelText != null)
        {
            skillLevelText.text = $"Level {skill.Level}";
        }

        // Update exp bar
        if (expBarFill != null)
        {
            float expPercentage = skill.GetExpPercentage();
            expBarFill.fillAmount = expPercentage;
        }

        // Update tooltip if it's visible
        if (isHovering && tooltipPanel != null && tooltipPanel.activeSelf)
        {
            UpdateTooltipText();
        }
    }

    private void UpdateTooltipText()
    {
        if (tooltipText == null || skill == null) return;

        string expDisplay = skill.GetExpDisplayString();
        tooltipText.text = $"<b>{skill.SkillName}</b>\nLevel: {skill.Level}/{skill.MaxLevel}\n{expDisplay}";
    }

    private void UpdateTooltipPosition()
    {
        if (tooltipPanel == null) return;

        Vector2 mousePosition = Input.mousePosition;
        
        // Offset the tooltip so it doesn't overlap with cursor
        Vector2 offset = new Vector2(15f, -15f);
        
        tooltipPanel.transform.position = mousePosition + offset;
    }

    // Event handlers
    private void OnSkillLevelUp(int newLevel)
    {
        UpdateUI();
        // Add visual feedback here (particle effects, animations, etc.)
    }

    private void OnSkillExpGained(float expAmount)
    {
        UpdateUI();
    }

    // Pointer event handlers for hover functionality
    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        
        if (tooltipPanel != null)
        {
            tooltipPanel.SetActive(true);
            UpdateTooltipText();
            UpdateTooltipPosition();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        
        if (tooltipPanel != null)
        {
            tooltipPanel.SetActive(false);
        }
    }
}
