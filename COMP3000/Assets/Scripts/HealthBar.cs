using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [Header("Health Bar References")]
    [SerializeField] private Image healthBarFill;
    [SerializeField] private Image healthBarBackground;

    [Header("Bar Settings")]
    [SerializeField] private float updateSpeed = 5f;

    private float currentHealth;
    private float maxHealth;
    private float targetFillAmount;

    void Start()
    {
        if (healthBarBackground != null)
        {
            healthBarBackground.sprite = SpriteGenerator.CreateColoredSquare(Color.gray, 32);
        }

        if (healthBarFill != null)
        {
            healthBarFill.sprite = SpriteGenerator.CreateColoredSquare(Color.green, 32);
        }

        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            canvas.sortingOrder = 10000;
            Debug.Log("Health bar Canvas sort order set to 10000");
        }
        else
        {
            Canvas thisCanvas = GetComponent<Canvas>();
            if (thisCanvas != null)
            {
                thisCanvas.sortingOrder = 10000;
                Debug.Log("Health bar Canvas sort order set to 10000");
            }
        }
    }

    void Update()
    {
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = Mathf.Lerp(healthBarFill.fillAmount, targetFillAmount, Time.deltaTime * updateSpeed);
        }
    }

    public void SetHealth(float current, float max)
    {
        currentHealth = current;
        maxHealth = max;
        targetFillAmount = maxHealth > 0 ? currentHealth / maxHealth : 0;

        Debug.Log($"HealthBar SetHealth - Current: {currentHealth}, Max: {maxHealth}, Fill: {targetFillAmount}, Image: {(healthBarFill == null ? "NULL" : "SET")}");

        if (healthBarFill != null)
        {
            healthBarFill.sprite = SpriteGenerator.CreateColoredSquare(Color.green, 32);
        }
    }

    public void UpdateHealth(float amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        SetHealth(currentHealth, maxHealth);
    }
}
