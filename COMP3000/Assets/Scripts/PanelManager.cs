using UnityEngine;
using UnityEngine.UI;

public class PanelManager : MonoBehaviour
{
    [SerializeField] private Button inventoryButton;
    [SerializeField] private Button skillsButton;
    [SerializeField] private Button equipmentButton;
    [SerializeField] private Button settingsButton;

    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private GameObject skillsPanel;
    [SerializeField] private GameObject equipmentPanel;
    [SerializeField] private GameObject settingsPanel;

    [SerializeField] private Color activeButtonColor = Color.white;
    [SerializeField] private Color inactiveButtonColor = new Color(0.7f, 0.7f, 0.7f);

    void Start()
    {
        Debug.Log("PanelManager Start() called!");

        if (inventoryButton == null)
        {
            Debug.LogError("PanelManager: Inventory Button is NULL!");
            return;
        }
        if (skillsButton == null)
        {
            Debug.LogError("PanelManager: Skills Button is NULL!");
            return;
        }
        if (equipmentButton == null)
        {
            Debug.LogError("PanelManager: Equipment Button is NULL!");
            return;
        }
        if (settingsButton == null)
        {
            Debug.LogError("PanelManager: Settings Button is NULL!");
            return;
        }
        if (inventoryPanel == null || skillsPanel == null || equipmentPanel == null || settingsPanel == null)
        {
            Debug.LogError("PanelManager: Not all panels assigned in Inspector!");
            return;
        }

        Debug.Log("All references verified successfully!");

        inventoryButton.onClick.AddListener(() => {
            Debug.Log("Inventory button clicked!");
            ShowPanel(inventoryPanel, inventoryButton);
        });
        skillsButton.onClick.AddListener(() => {
            Debug.Log("Skills button clicked!");
            ShowPanel(skillsPanel, skillsButton);
        });
        equipmentButton.onClick.AddListener(() => {
            Debug.Log("Equipment button clicked!");
            ShowPanel(equipmentPanel, equipmentButton);
        });
        settingsButton.onClick.AddListener(() => {
            Debug.Log("Settings button clicked!");
            ShowPanel(settingsPanel, settingsButton);
        });

        Debug.Log("PanelManager: All listeners registered successfully");

        ShowPanel(inventoryPanel, inventoryButton);
    }

    private void ShowPanel(GameObject panelToShow, Button activeButton)
    {
        inventoryPanel.SetActive(false);
        skillsPanel.SetActive(false);
        equipmentPanel.SetActive(false);
        settingsPanel.SetActive(false);

        panelToShow.SetActive(true);

        UpdateButtonColors(activeButton);

        Debug.Log($"Showing panel: {panelToShow.name}");
    }

    private void UpdateButtonColors(Button activeButton)
    {
        inventoryButton.image.color = inactiveButtonColor;
        skillsButton.image.color = inactiveButtonColor;
        equipmentButton.image.color = inactiveButtonColor;
        settingsButton.image.color = inactiveButtonColor;

        activeButton.image.color = activeButtonColor;
    }
}
