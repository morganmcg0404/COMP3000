using UnityEngine;

public class PlayerSpawnPoint : MonoBehaviour
{
    [SerializeField] private Vector3 spawnOffset = Vector3.zero;

    private void OnDrawGizmosSelected()
    {
        // Draw spawn point in editor
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(GetSpawnPosition(), 0.5f);
    }

    public Vector3 GetSpawnPosition()
    {
        return transform.position + spawnOffset;
    }
}
