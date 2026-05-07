using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    [Header("Portal Settings")]
    [SerializeField] private string targetSceneName;
    [SerializeField] private Vector3Int portalGridPosition;
    [SerializeField] private float interactionRadius = 1.5f;

    private PlayerController playerController;
    private GridManager gridManager;
    private bool isPlayerMovingToPortal = false;
    private bool isPlayerAtPortal = false;

    void Start()
    {
        playerController = FindFirstObjectByType<PlayerController>();
        gridManager = FindFirstObjectByType<GridManager>();

        if (gridManager != null)
        {
            // Calculate grid position from world position if not set
            if (portalGridPosition == Vector3Int.zero)
            {
                portalGridPosition = gridManager.WorldToGrid(transform.position);
                Debug.Log($"Portal grid position calculated as: {portalGridPosition}");
            }
        }

        // Add a collider if not present
        if (GetComponent<Collider2D>() == null)
        {
            CircleCollider2D collider = gameObject.AddComponent<CircleCollider2D>();
            collider.radius = interactionRadius;
            collider.isTrigger = true;
        }
    }

    void OnMouseDown()
    {
        if (playerController != null && gridManager != null)
        {
            Debug.Log($"Portal clicked! Moving player to {portalGridPosition}");
            playerController.MoveTo(portalGridPosition);
            isPlayerMovingToPortal = true;
        }
        else
        {
            Debug.LogError("Portal: PlayerController or GridManager not found!");
        }
    }

    void Update()
    {
        // Check if player has reached the portal position
        if (isPlayerMovingToPortal && !isPlayerAtPortal && playerController != null)
        {
            Vector3Int playerPos = playerController.CurrentGridPosition;
            float distance = Vector3Int.Distance(playerPos, portalGridPosition);

            if (distance <= 1.0f) // Within 1 tile
            {
                Debug.Log($"Player reached portal area! Loading scene: {targetSceneName}");
                isPlayerAtPortal = true;
                LoadTargetScene();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && isPlayerMovingToPortal)
        {
            Debug.Log($"Player entered portal trigger! Loading scene: {targetSceneName}");
            isPlayerAtPortal = true;
            LoadTargetScene();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerMovingToPortal = false;
        }
    }

    private void LoadTargetScene()
    {
        if (!string.IsNullOrEmpty(targetSceneName))
        {
            StartCoroutine(LoadSceneAsync());
        }
        else
        {
            Debug.LogError("Portal: No target scene specified!");
        }
    }

    private IEnumerator LoadSceneAsync()
    {
        // Wait a frame to ensure player persists
        yield return null;

        Debug.Log($"Loading scene: {targetSceneName}");
        SceneManager.LoadScene(targetSceneName);

        // Wait for scene to load
        yield return null;

        // Reset player state in new scene
        if (playerController != null)
        {
            playerController.ResetMovement();
            Debug.Log("Player movement reset after scene load");
        }

        // Find and reconnect GridManager
        GridManager newGridManager = FindFirstObjectByType<GridManager>();
        if (newGridManager != null && playerController != null)
        {
            Debug.Log("New GridManager found in scene, player systems reconnected");
        }
    }
}