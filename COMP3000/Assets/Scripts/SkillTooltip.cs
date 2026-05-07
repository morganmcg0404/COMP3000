using UnityEngine;
using TMPro;

public class SkillTooltip : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI skillNameText;
    [SerializeField] private TextMeshProUGUI xpInfoText;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private float tooltipOffset = 10f;

    void Awake()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();
        
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();

        canvasGroup.alpha = 0f;
        gameObject.SetActive(false);
    }

    public void SetTextElements(TextMeshProUGUI skillName, TextMeshProUGUI xpInfo)
    {
        skillNameText = skillName;
        xpInfoText = xpInfo;
    }

    public void Show(string skillName, int level, float currentXp, float xpForNextLevel, float remainingXp, RectTransform slotRect)
    {
        if (skillNameText != null)
            skillNameText.text = $"{skillName} - Level {level}";
        
        if (xpInfoText != null)
            xpInfoText.text = $"XP: {currentXp:F0} / {xpForNextLevel:F0}\nRemaining: {remainingXp:F0}";

        PositionTooltip(slotRect);

        gameObject.SetActive(true);
        canvasGroup.alpha = 1f;
    }

    private void PositionTooltip(RectTransform slotRect)
    {
        if (slotRect == null) return;

        // Position tooltip at fixed position
        rectTransform.anchoredPosition = new Vector2(288.5f, -137f);
    }

    public void Hide()
    {
        canvasGroup.alpha = 0f;
        gameObject.SetActive(false);
    }
}
