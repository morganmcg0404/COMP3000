using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalManager : MonoBehaviour
{
    [System.Serializable]
    public class PortalData
    {
        public string portalName;
        public Vector3 position;
        public string targetScene;
        public Sprite portalSprite;
    }

    [Header("Portal Configuration")]
    [SerializeField] private List<PortalData> portals = new List<PortalData>();

    void Start()
    {
        CreatePortals();
    }

    private void CreatePortals()
    {
        foreach (PortalData portalData in portals)
        {
            GameObject portalObj = new GameObject(portalData.portalName);
            portalObj.transform.position = portalData.position;
            portalObj.transform.SetParent(transform);

            // Add SpriteRenderer
            SpriteRenderer spriteRenderer = portalObj.AddComponent<SpriteRenderer>();
            if (portalData.portalSprite != null)
            {
                spriteRenderer.sprite = portalData.portalSprite;
            }
            else
            {
                // Create a simple colored square as placeholder
                Texture2D texture = new Texture2D(32, 32);
                Color[] colors = new Color[32 * 32];
                for (int i = 0; i < colors.Length; i++)
                {
                    colors[i] = new Color(0.2f, 0.8f, 1.0f, 0.8f); // Light blue with transparency
                }
                texture.SetPixels(colors);
                texture.Apply();

                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f));
                spriteRenderer.sprite = sprite;
            }

            // Add Portal script
            Portal portal = portalObj.AddComponent<Portal>();
            portal.GetType().GetField("targetSceneName", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(portal, portalData.targetScene);

            Debug.Log($"Created portal '{portalData.portalName}' at {portalData.position} targeting scene '{portalData.targetScene}'");
        }
    }
}