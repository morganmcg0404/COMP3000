using UnityEngine;
using UnityEngine.UI;

public class InventoryPanelSetup : MonoBehaviour
{
    [SerializeField] private float padding = 10f;
    [SerializeField] private float spacing = 5f;
    [SerializeField] private int columns = 4;
    [SerializeField] private int rows = 7;

    void Start()
    {
        CreateInventorySlots();
        SetupGridLayout();
    }

    void OnEnable()
    {
        Invoke("SetupGridLayout", 0.1f);
    }

    private void CreateInventorySlots()
    {
        if (transform.childCount > 0)
        {
            Debug.Log("Inventory slots already exist, skipping creation");
            return;
        }

        for (int i = 0; i < 28; i++)
        {
            GameObject slotObj = new GameObject($"InventorySlot_{i + 1}");
            slotObj.transform.SetParent(transform, false);

            Image slotImage = slotObj.AddComponent<Image>();
            slotImage.color = new Color(0.8f, 0.8f, 0.8f, 1f);

            LayoutElement layoutElement = slotObj.AddComponent<LayoutElement>();

            Debug.Log($"Created inventory slot: {slotObj.name}");
        }
    }

    private void SetupGridLayout()
    {
        GridLayoutGroup gridLayout = GetComponent<GridLayoutGroup>();
        if (gridLayout == null)
        {
            gridLayout = gameObject.AddComponent<GridLayoutGroup>();
        }

        gridLayout.padding = new RectOffset((int)padding, (int)padding, (int)padding, (int)padding);

        gridLayout.spacing = new Vector2(spacing, spacing);

        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = columns;

        RectTransform panelRect = GetComponent<RectTransform>();
        if (panelRect != null)
        {
            float availableWidth = panelRect.rect.width - (padding * 2) - (spacing * (columns - 1));
            float cellSize = availableWidth / columns;

            float requiredHeight = (cellSize * rows) + (padding * 2) + (spacing * (rows - 1));

            panelRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, requiredHeight);

            gridLayout.cellSize = new Vector2(cellSize, cellSize);

            Debug.Log($"Inventory grid configured: {columns}x{rows}, panel width: {panelRect.rect.width}, required height: {requiredHeight}, cell size: {cellSize}");
            
            foreach (Transform child in transform)
            {
                LayoutElement layoutElement = child.GetComponent<LayoutElement>();
                if (layoutElement == null)
                {
                    layoutElement = child.gameObject.AddComponent<LayoutElement>();
                }
                layoutElement.preferredWidth = cellSize;
                layoutElement.preferredHeight = cellSize;
            }
        }
        else
        {
            Debug.LogError("InventoryPanelSetup: Panel RectTransform not found!");
        }
    }
}
