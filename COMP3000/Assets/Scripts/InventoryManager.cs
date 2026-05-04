using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInventory playerInventory;

    public PlayerInventory PlayerInventory => playerInventory;

    void Start()
    {
        if (playerInventory == null)
        {
            playerInventory = FindFirstObjectByType<PlayerInventory>();
        }

        if (playerInventory == null)
        {
            Debug.LogError("InventoryManager: PlayerInventory not found!");
        }
    }
}
