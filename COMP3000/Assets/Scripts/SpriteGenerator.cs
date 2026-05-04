using UnityEngine;

public static class SpriteGenerator
{
    public static Sprite CreateWhiteSquare(int size = 32)
    {
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);

        Color[] pixels = new Color[size * size];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.white;
        }

        texture.SetPixels(pixels);
        texture.Apply();
        texture.filterMode = FilterMode.Point;

        Sprite sprite = Sprite.Create(
            texture,
            new Rect(0, 0, size, size),
            new Vector2(0.5f, 0.5f),
            100f
        );
        
        return sprite;
    }

    public static Sprite CreateColoredSquare(Color color, int size = 32)
    {
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        
        Color[] pixels = new Color[size * size];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = color;
        }
        
        texture.SetPixels(pixels);
        texture.Apply();
        texture.filterMode = FilterMode.Point;
        
        Sprite sprite = Sprite.Create(
            texture,
            new Rect(0, 0, size, size),
            new Vector2(0.5f, 0.5f),
            100f
        );
        
        return sprite;
    }

    public static Sprite CreateBorderedSquare(Color fillColor, Color borderColor, int size = 32, int borderWidth = 2)
    {
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        
        Color[] pixels = new Color[size * size];

        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = fillColor;
        }

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                bool isOnBorder = (x < borderWidth || x >= size - borderWidth || 
                                   y < borderWidth || y >= size - borderWidth);
                
                if (isOnBorder)
                {
                    pixels[y * size + x] = borderColor;
                }
            }
        }
        
        texture.SetPixels(pixels);
        texture.Apply();
        texture.filterMode = FilterMode.Point;
        
        Sprite sprite = Sprite.Create(
            texture,
            new Rect(0, 0, size, size),
            new Vector2(0.5f, 0.5f),
            100f
        );
        
        return sprite;
    }
}
