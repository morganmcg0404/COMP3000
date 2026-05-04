using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Button))]
public class MovementModeButton : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerController playerController;

    [Header("UI Settings")]
    [SerializeField] private TextMeshProUGUI buttonText;
    [SerializeField] private Text legacyButtonText;

    [Header("Display Text")]
    [SerializeField] private string runningText = "Running";
    [SerializeField] private string walkingText = "Walking";

    [Header("Optional Colors")]
    [SerializeField] private bool useColorTinting = true;
    [SerializeField] private Color runningColor = new Color(0.2f, 1f, 0.2f); // Green
    [SerializeField] private Color walkingColor = new Color(1f, 1f, 0.2f); // Yellow

    private Button button;
    private Image buttonImage;

    void Start()
    {
        // Get button component
        button = GetComponent<Button>();
        buttonImage = GetComponent<Image>();

        // Find PlayerController if not assigned
        if (playerController == null)
        {
            playerController = FindFirstObjectByType<PlayerController>();
            if (playerController == null)
            {
                Debug.LogError("MovementModeButton: PlayerController not found!");
                return;
            }
        }

        // Add click listener
        button.onClick.AddListener(OnButtonClicked);

        // Subscribe to movement mode changes
        playerController.OnMovementModeChanged += UpdateButtonDisplay;

        // Initialize display
        UpdateButtonDisplay(playerController.IsRunning);

        Debug.Log("MovementModeButton initialized!");
    }

    void OnDestroy()
    {
        // Unsubscribe from events
        if (playerController != null)
        {
            playerController.OnMovementModeChanged -= UpdateButtonDisplay;
        }

        // Remove listener
        if (button != null)
        {
            button.onClick.RemoveListener(OnButtonClicked);
        }
    }

    private void OnButtonClicked()
    {
        if (playerController != null)
        {
            playerController.ToggleMovementMode();
        }
    }

    private void UpdateButtonDisplay(bool isRunning)
    {
        string displayText = isRunning ? runningText : walkingText;

        // Update TextMeshPro text if available
        if (buttonText != null)
        {
            buttonText.text = displayText;
        }

        // Update legacy Text if available
        if (legacyButtonText != null)
        {
            legacyButtonText.text = displayText;
        }

        // Update button color if enabled
        if (useColorTinting && buttonImage != null)
        {
            buttonImage.color = isRunning ? runningColor : walkingColor;
        }

        Debug.Log($"Movement mode button updated: {displayText}");
    }
}
