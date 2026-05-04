using UnityEngine;
using TMPro;
using System.Collections;

public class XPDrop : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI xpText;
    [SerializeField] private float floatDuration = 2f;
    [SerializeField] private float floatHeight = 100f;
    [SerializeField] private float fadeDelay = 1.5f;

    private Vector3 startPosition;
    private CanvasGroup canvasGroup;

    void Start()
    {
        startPosition = transform.position;
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        Canvas canvas = GetComponent<Canvas>();
        if (canvas != null)
        {
            canvas.sortingOrder = 10000;
        }

        StartCoroutine(FloatAndFade());
    }

    public void Initialize(int xpAmount, string skillName = "")
    {
        if (xpText != null)
        {
            xpText.text = "+" + xpAmount.ToString() + " XP (" + skillName + ")";
            xpText.color = Color.yellow;
        }

        Debug.Log($"XPDrop created: {xpAmount} XP ({skillName}) at {transform.position}");
    }

    private IEnumerator FloatAndFade()
    {
        float elapsedTime = 0f;

        while (elapsedTime < floatDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / floatDuration;

            Vector3 newPos = startPosition;
            newPos.y += floatHeight * progress;
            transform.position = newPos;

            if (elapsedTime > fadeDelay)
            {
                float fadeProgress = (elapsedTime - fadeDelay) / (floatDuration - fadeDelay);
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, fadeProgress);
            }

            yield return null;
        }

        Destroy(gameObject);
    }
}
