using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerPersistence : MonoBehaviour
{
    private static PlayerPersistence instance;
    private PlayerController playerController;
    private Camera playerCamera;

    private void Awake()
    {
        // Check if this is the first instance
        if (instance == null)
        {
            instance = this;
            
            // CRITICAL: Mark as DontDestroyOnLoad IMMEDIATELY in Awake
            DontDestroyOnLoad(gameObject);
            Debug.Log($"[PlayerPersistence] Player GameObject '{gameObject.name}' marked as DontDestroyOnLoad in Awake()");

            // Find and preserve camera
            playerCamera = GetComponentInChildren<Camera>();
            if (playerCamera == null)
            {
                playerCamera = FindFirstObjectByType<Camera>();
            }

            if (playerCamera != null && playerCamera.gameObject != gameObject)
            {
                DontDestroyOnLoad(playerCamera.gameObject);
                Debug.Log($"[PlayerPersistence] Camera '{playerCamera.gameObject.name}' marked as DontDestroyOnLoad");
            }

            // Find player controller
            playerController = GetComponent<PlayerController>();
            if (playerController == null)
            {
                playerController = GetComponentInChildren<PlayerController>();
            }

            Debug.Log("[PlayerPersistence] Initialized and preserved");

            // Subscribe to scene changes
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            // Destroy duplicate
            Debug.Log("[PlayerPersistence] Duplicate found, destroying");
            Destroy(gameObject);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"[PlayerPersistence] Scene loaded: {scene.name}");

        // Find spawn point in the new scene
        PlayerSpawnPoint spawnPoint = FindFirstObjectByType<PlayerSpawnPoint>();
        
        if (spawnPoint != null)
        {
            Vector3 spawnPos = spawnPoint.GetSpawnPosition();
            transform.position = spawnPos;
            Debug.Log($"[PlayerPersistence] Player repositioned to spawn point at {spawnPos}");
        }

        // Reset player state
        if (playerController != null)
        {
            playerController.ResetMovement();
            Debug.Log("[PlayerPersistence] Player movement reset");
        }
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}
