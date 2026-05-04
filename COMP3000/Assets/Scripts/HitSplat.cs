using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class HitSplat : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI damageText;
    [SerializeField] private Image background;
    [SerializeField] private float floatDuration = 1.5f;
    [SerializeField] private float floatHeight = 2f;
    [SerializeField] private float fadeDelay = 0.5f;

    private Vector3 startPosition;
    private CanvasGroup canvasGroup;
    private Transform targetTransform;

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

    void Update()
    {
        if (targetTransform != null)
        {
            transform.position = targetTransform.position;
        }
    }
    
    public void Initialize(int damage, Vector3 worldPosition, Transform target = null)
    {
        transform.position = worldPosition;
        startPosition = worldPosition;
        targetTransform = target;

        if (damageText != null)
        {
            damageText.text = damage.ToString();
            damageText.color = Color.white;
        }

        if (background != null)
        {
            if (damage == 0)
            {
                background.color = Color.blue;
            }
            else
            {
                background.color = Color.red;
            }
        }

        Debug.Log($"HitSplat created: {damage} damage at {worldPosition}");
    }

    private IEnumerator FloatAndFade()
    {
        yield return new WaitForSeconds(floatDuration);
        Destroy(gameObject);
    }
}
