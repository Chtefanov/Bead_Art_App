using UnityEngine;
using UnityEngine.UI;

public class ImagePixelator : MonoBehaviour
{
    //Game objects 
    public GameObject Initial_Buttons;
    public GameObject After_Pixelation_Buttons;
    public GameObject CVD_Mode_Buttons;
    public RawImage Gallery_Displayer;
    //Exeption case Buttons
    public GameObject Select_Picture_Button;
    public GameObject Rotate_Picture_Button;

    // UI TEXTS
    public GameObject QuestionText;
    public GameObject CVDTempSaveText;
    public GameObject TempSaveText;

    /* Defining a fixed color pallette. Some color are luckelly defined in unity already     
       
       Unity uses float values ranging 0-1 for definition of colors. Using RGB color-wheel for selection,
       the corresponding color value needed for colors not inlcuded in unity, is simply a matter 
       of devision of 255 (the rgp max values) 

      Colors not included defined needs to have "1f" or alpha value 
      set to 1 so to not be opaque.   
     
     */
    private Color[] palette = new Color[] {

    Color.red, // Red
    Color.green, // Green
    Color.blue, // Blue
    Color.yellow, // Yellow
    Color.black, // Black
    Color.white, // White
    Color.magenta, // Pink or magenta, looks the same

    new Color(0.501f, 0.031f, 0.870f, 1f),  // Purple RGB original 128, 8, 222 - devided by 255 >
    new Color(0.937f, 0.643f, 0.129f, 1f), // Orange RGB original  239, 164, 33
    new Color(0.576f, 0.980f, 0.298f, 1f) // Light Green RGP  RGB original 147, 250, 76 
    };

    public enum UIState
    {
        Initial,
        AfterPixelation,
        CVDMode
    }
    public void UI_Change(UIState state)
    {
        bool showInitial = state == UIState.Initial;
        bool showAfterPixelation = state == UIState.AfterPixelation;
        bool showCVDMode = state == UIState.CVDMode;

        Initial_Buttons.SetActive(showInitial);
        After_Pixelation_Buttons.SetActive(showAfterPixelation);
        CVD_Mode_Buttons.SetActive(showCVDMode);

        // Assuming you want to hide all texts in the initial state and show specific ones in other states

        //CVDTempSaveText.SetActive(showCVDMode);
        //TempSaveText.SetActive(showAfterPixelation);
    }

    public void SetUIStateToInitial()
    {
        UI_Change(UIState.Initial);
        /*if (Gallery_Displayer.texture != null)
        {
            Select_Picture_Button.SetActive(false);
            Rotate_Picture_Button.SetActive(true);
        }
        else
        {
            Select_Picture_Button.SetActive(true);
            Rotate_Picture_Button.SetActive(false);
        }
        */
    }
    public void SetUIStateToAfterPixelation()
    {
        UI_Change(UIState.AfterPixelation);
        QuestionText.SetActive(true);
    }
    public void SetUIStateToCVDMode()
    {
        QuestionText.SetActive(false);
        UI_Change(UIState.CVDMode);

    }
}
    /*
    public Texture2D Pixelate(Texture2D originalTexture)
    {
        // Resize the image to a 29x29 grid
        Texture2D resizedImage = ResizeTexture(originalTexture, 29, 29);

        // Quantize the colors to the palette
        Texture2D quantizedImage = QuantizeColors(resizedImage);

        // Scale it back up to the original size (or desired display size)
        Texture2D pixelatedImage = ScaleTexture(quantizedImage, originalTexture.width, originalTexture.height);

        return pixelatedImage;
    }
}

    public void ApplyPixelation()
    {
        // Get the original sprite's texture.
        Sprite originalSprite = GetComponent<Image>().sprite;
        Texture2D originalTexture = originalSprite.texture;

        // Pixelate the texture.
        Texture2D pixelatedTexture = Pixelate(originalTexture);

        // Create a new sprite from the pixelated texture.
        Sprite newSprite = Sprite.Create(pixelatedTexture, new Rect(0.0f, 0.0f, pixelatedTexture.width, pixelatedTexture.height), new Vector2(0.5f, 0.5f));

        // Set the new sprite to the Image component.
        GetComponent<Image>().sprite = newSprite;
    }
    public void PixelateDisplayFieldImage()
    {
        if (displayFieldImage.texture is Texture2D originalTexture)
        {
            Texture2D pixelatedTexture = Pixelate(originalTexture);
            displayFieldImage.texture = pixelatedTexture;
        }
        else
        {
            Debug.LogError("The texture of displayFieldImage is not a Texture2D and cannot be pixelated.");
        }
    }












    private Texture2D ResizeTexture(Texture2D source, int width, int height)
    {
        // Create a RenderTexture with the desired width and height.
        RenderTexture rt = new RenderTexture(width, height, 24);
        RenderTexture.active = rt;

        // Copy the source texture to the RenderTexture, resizing it in the process.
        Graphics.Blit(source, rt);

        // Create a new Texture2D with the desired width and height.
        Texture2D result = new Texture2D(width, height, source.format, false);

        // Read the pixels from the RenderTexture into the Texture2D.
        result.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        result.Apply();

        // Clean up
        RenderTexture.active = null;
        rt.Release();

        return result;
    }
    private Texture2D QuantizeColors(Texture2D source)
    {
        Texture2D result = new Texture2D(source.width, source.height, source.format, false);
        for (int y = 0; y < source.height; y++)
        {
            for (int x = 0; x < source.width; x++)
            {
                Color originalColor = source.GetPixel(x, y);
                Color closestPaletteColor = FindClosestPaletteColor(originalColor);
                result.SetPixel(x, y, closestPaletteColor);
            }
        }
        result.Apply();
        return result;
    }
    private Texture2D ScaleTexture(Texture2D source, int newWidth, int newHeight)
    {
        // Create a new empty texture with the desired size and the same format as the source.
        Texture2D result = new Texture2D(newWidth, newHeight, source.format, false);

        // Scale factors for x and y axes.
        float scaleX = (float)source.width / newWidth;
        float scaleY = (float)source.height / newHeight;

        // Iterate over all pixels in the new texture.
        for (int y = 0; y < newHeight; y++)
        {
            for (int x = 0; x < newWidth; x++)
            {
                // Get the corresponding source pixel (using Mathf.Floor to ensure we don't access out-of-bounds index).
                int sourceX = Mathf.FloorToInt(x * scaleX);
                int sourceY = Mathf.FloorToInt(y * scaleY);

                // Ensure the source indices are within the bounds of the source texture.
                sourceX = Mathf.Clamp(sourceX, 0, source.width - 1);
                sourceY = Mathf.Clamp(sourceY, 0, source.height - 1);

                // Get the color from the source texture and set it to the result texture.
                Color color = source.GetPixel(sourceX, sourceY);
                result.SetPixel(x, y, color);
            }
        }

        // Apply all the SetPixel calls on the result texture.
        result.Apply();

        // Return the scaled texture.
        return result;
    }
    private Color FindClosestPaletteColor(Color original)
    {
        Color closestColor = palette[0];
        float closestDistance = Mathf.Infinity;

        foreach (Color paletteColor in palette)
        {
            float distance = DistanceSqr(original, paletteColor);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestColor = paletteColor;
            }
        }
        return closestColor;
    }
    private float DistanceSqr(Color c1, Color c2)
    {
        float rDiff = c1.r - c2.r;
        float gDiff = c1.g - c2.g;
        float bDiff = c1.b - c2.b;
        return rDiff * rDiff + gDiff * gDiff + bDiff * bDiff;
    }
}
    */