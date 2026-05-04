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

    public void Show(string skillName, int level, float currentXp, float xpForNextLevel, float remainingXp, Vector3 slotPosition)
    {
        if (skillNameText != null)
            skillNameText.text = $"{skillName} - Level {level}";
        
        if (xpInfoText != null)
            xpInfoText.text = $"XP: {currentXp:F0} / {xpForNextLevel:F0}\nRemaining: {remainingXp:F0}";

        PositionTooltip(slotPosition);

        gameObject.SetActive(true);
        canvasGroup.alpha = 1f;
    }

    private void PositionTooltip(Vector3 slotPosition)
    {
        Vector3 leftPos = slotPosition + new Vector3(-rectTransform.rect.width - tooltipOffset + 45, -tooltipOffset, 0);
        rectTransform.position = leftPos;

        Canvas canvas = GetComponentInParent<Canvas>();
        
        if (canvas != null)
        {
            Vector3[] tooltipCorners = new Vector3[4];
            rectTransform.GetWorldCorners(tooltipCorners);
            
            Vector3[] canvasCorners = new Vector3[4];
            canvas.GetComponent<RectTransform>().GetWorldCorners(canvasCorners);
            
            float canvasMinX = Mathf.Min(canvasCorners[0].x, canvasCorners[1].x, canvasCorners[2].x, canvasCorners[3].x);
            float canvasMaxX = Mathf.Max(canvasCorners[0].x, canvasCorners[1].x, canvasCorners[2].x, canvasCorners[3].x);
            
            if (tooltipCorners[0].x < canvasMinX)
            {
                Vector3 rightPos = slotPosition + new Vector3(rectTransform.rect.width + tooltipOffset, -tooltipOffset, 0);
                rectTransform.position = rightPos;
            }
        }
    }

    public void Hide()
    {
        canvasGroup.alpha = 0f;
        gameObject.SetActive(false);
    }
}
