using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class SkillSlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TextMeshProUGUI skillNameText;
    [SerializeField] private TextMeshProUGUI skillLevelText;
    [SerializeField] private Image skillBackground;
    [SerializeField] private LayoutElement layoutElement;
    
    private Skill skill;
    private SkillTooltip tooltip;
    private Image hoverHighlight;
    private bool isHovering = false;
    
    private Color normalColor = new Color(0x59 / 255f, 0x42 / 255f, 0x39 / 255f, 1f);
    private Color hoverColor = new Color(0x70 / 255f, 0x55 / 255f, 0x50 / 255f, 1f);

    public void Initialize(Skill skillData, SkillTooltip tooltipPrefab)
    {
        skill = skillData;
        
        tooltip = tooltipPrefab;
        
        if (tooltip == null)
        {
            Debug.LogWarning($"SkillSlotUI: No tooltip provided for {skillData.SkillName}!");
        }
        
        hoverHighlight = GetComponent<Image>();
        if (hoverHighlight == null)
        {
            hoverHighlight = gameObject.AddComponent<Image>();
            hoverHighlight.color = normalColor;
        }

        if (skillNameText == null)
        {
            skillNameText = GetComponentInChildren<TextMeshProUGUI>();
        }
        
        if (skillLevelText == null)
        {
            TextMeshProUGUI[] textArray = GetComponentsInChildren<TextMeshProUGUI>();
            if (textArray.Length > 1)
                skillLevelText = textArray[1];
        }

        if (layoutElement == null)
            layoutElement = GetComponent<LayoutElement>();

        UpdateDisplay();
        
        skill.OnLevelUp += OnSkillLevelUp;
        skill.OnExpGained += OnSkillExpGained;
    }

    private void UpdateDisplay()
    {
        if (skillNameText != null)
            skillNameText.text = skill.SkillName;
        
        if (skillLevelText != null)
            skillLevelText.text = $"Lvl {skill.Level}";
    }

    private void OnSkillLevelUp(int newLevel)
    {
        UpdateDisplay();
    }

    private void OnSkillExpGained(float expAmount)
    {
        if (isHovering)
        {
            UpdateTooltip();
        }
    }

    private void UpdateTooltip()
    {
        if (tooltip != null)
        {
            float currentXp = skill.CurrentExp;
            float xpForNextLevel = skill.GetExpForNextLevel();
            float remainingXp = xpForNextLevel - currentXp;
            
            tooltip.Show(
                skill.SkillName,
                skill.Level,
                currentXp,
                xpForNextLevel,
                remainingXp,
                GetComponent<RectTransform>().position
            );
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        
        if (hoverHighlight != null)
            hoverHighlight.color = hoverColor;
        
        UpdateTooltip();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        
        if (hoverHighlight != null)
            hoverHighlight.color = normalColor;
        
        if (tooltip != null)
            tooltip.Hide();
    }

    private void OnDestroy()
    {
        if (skill != null)
        {
            skill.OnLevelUp -= OnSkillLevelUp;
            skill.OnExpGained -= OnSkillExpGained;
        }
    }
}
