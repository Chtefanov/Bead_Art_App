using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Handles pixelation effects and CVD modes on images.
/// </summary>
public class Pixelator_2 : MonoBehaviour
{
    #region public UI Compoenents
    [Header("Gallery Display")]
    public RawImage Gallery_Displayer;

    [Header("Initial UI Buttons")]
    public GameObject Select_Picture_Button;
    public GameObject Back_To_Main_Menu;

    [Header("Other UI Buttons")]
    public GameObject Pixelate_Picture_Button;
    public GameObject Save_Pixelation_Button;
    //Saved Pixelation UI-Buttons
    public GameObject CVD_Mode_No_Button;
    public GameObject CVD_Mode_Yes_Button;

    [Header("UI text")]
    public TextMeshProUGUI QuestionText;
    public TextMeshProUGUI Template_CVD_Save;
    public TextMeshProUGUI Template_Saved;
    public GameObject QuestionTextGameObject; //For Reference to the GameObject containing TextMeshProUGUI component

    public Texture2D[] Color_CVD_Icons;

    #endregion

    #region Private Fields - aspect ratio for picture/UI, colors, UI state and CVD mode icons
    private const float DefaultAspectRatio = 0.64f;
    
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

    private UIState currentUIState = UIState.Initial;

    private Dictionary<Color, Texture2D> InitializeColorToIconMap()
    {
        var colorToIconMap = new Dictionary<Color, Texture2D>();
        for (int i = 0; i < palette.Length; i++)
        {
            colorToIconMap.Add(palette[i], Color_CVD_Icons[i]); // Make sure this matches with your color array
        }
        return colorToIconMap;
    }

    //... other private fields
    #endregion

    #region Life Cycles Pixelator and UI states
    private void OnEnable()
    {
        ResetPixelatorPanel();
    }
    public void ImageSelected(Texture2D image)
    {
        UpdateUI(UIState.PictureSelected);
    }
    public void ResetPixelatorPanel()
    {
        if (Gallery_Displayer != null)
        {
            Gallery_Displayer.texture = null;
            UpdateUI(UIState.Initial);
        }
    }
    public enum UIState
    {
        Initial,
        PictureSelected,
        AfterPixelation,
        AcceptedPixelation,
        SavePixelationMode
    }
    private void UpdateUI(UIState newState)
    {
        currentUIState = newState;
        Debug.Log("Updating UI to state: " + newState);

        // Handle the visibility of buttons based on the state
        Select_Picture_Button.SetActive(newState == UIState.Initial || newState == UIState.PictureSelected || newState == UIState.AfterPixelation);
        Pixelate_Picture_Button.SetActive(newState == UIState.PictureSelected);
        Save_Pixelation_Button.SetActive(newState == UIState.AfterPixelation);
        Back_To_Main_Menu.SetActive(newState != UIState.SavePixelationMode);

        // Handle the visibility of CVD mode buttons
        CVD_Mode_Yes_Button.SetActive(newState == UIState.SavePixelationMode);
        CVD_Mode_No_Button.SetActive(newState == UIState.SavePixelationMode);

        // Handle the visibility of TextMeshProUGUI GameObject
        QuestionTextGameObject.SetActive(newState == UIState.SavePixelationMode);

        // Optionally update the text if needed
        if (newState == UIState.SavePixelationMode && QuestionTextGameObject.TryGetComponent(out TextMeshProUGUI textComponent))
        {
            textComponent.text = "Would you like to apply CVD mode to your bead art?";
        }
    }
    public void ShowYesCVD()
    {
        StartCoroutine(ShowTemplateMessageCorotine(Template_CVD_Save, 3f));
    }
    public void ShowNoCVD()
    {
        StartCoroutine(ShowTemplateMessageCorotine(Template_CVD_Save, 3f));
    }
    private IEnumerator ShowTemplateMessageCorotine(TextMeshProUGUI whereSaved, float duration)
    {
        whereSaved.gameObject.SetActive(true);
        yield return new WaitForSeconds(duration);
        whereSaved.gameObject.SetActive(false);
    }

    #endregion

    #region Pixelation Methods
    public void Pixelatetion_Pressed()
    {
        if (Gallery_Displayer.texture is Texture2D originalTexture)
        {
            Texture2D pixelatedTexture = Pixelate(originalTexture);
            Gallery_Displayer.texture = pixelatedTexture;
            UpdateUI(UIState.AfterPixelation);
        }
    }
    public void Pixelate_Prosess()
    {
        if(Gallery_Displayer.texture is Texture2D PrePixelatedPicture) 
        {
            Texture2D PixelatedPicture = Pixelate(PrePixelatedPicture);
            Gallery_Displayer.texture = PixelatedPicture;
        }
    }

    private Texture2D Pixelate(Texture2D PrePixelatedPicture)
    {
        Texture2D Grid_Fitted_Image = Fit_Image_To_Beads(PrePixelatedPicture, 29, 29);
        Texture2D Ten_Pallet_Colors = Pallet_To_Picture(Grid_Fitted_Image, palette);
        return Ten_Pallet_Colors;
    }
    private Texture2D Fit_Image_To_Beads(Texture2D image, int width, int height)
    {
        RenderTexture rt = new RenderTexture(width, height, 29);
        RenderTexture.active = rt;
        Graphics.Blit(image, rt);
        //Created new texture for 29x29 grid
        Texture2D results = new Texture2D(width, height, image.format, false);
        //Reads the pixel from rt into texture2D
        results.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        results.Apply();
        RenderTexture.active = null;
        rt.Release();
        return results;
    }
    public void Save_Pixelation_Pressed()
    {
        UpdateUI(UIState.SavePixelationMode);
    }
    private Texture2D ResizeTexture(Texture2D source, int newWidth, int newHeight)
    {
        source.filterMode = FilterMode.Point;
        RenderTexture rt = RenderTexture.GetTemporary(newWidth, newHeight);
        rt.filterMode = FilterMode.Point;
        RenderTexture.active = rt;
        Graphics.Blit(source, rt);
        Texture2D newTexture = new Texture2D(newWidth, newHeight);
        newTexture.ReadPixels(new Rect(0, 0, newWidth, newHeight), 0, 0);
        newTexture.Apply();
        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(rt);
        return newTexture;
    }


    private Texture2D Fit_Image_To_Beads(Texture2D image)
    {
        // Define the target width and height (29x29 grid)
        int targetWidth = 29;
        int targetHeight = 29;

        // Create a new RenderTexture with the target dimensions
        RenderTexture rt = new RenderTexture(targetWidth, targetHeight, 24);
        RenderTexture.active = rt;

        // Blit the original image to the RenderTexture
        Graphics.Blit(image, rt);

        // Create a new Texture2D with the target dimensions
        Texture2D result = new Texture2D(targetWidth, targetHeight, TextureFormat.RGBA32, false);

        // Read pixels from RenderTexture and apply to the new Texture2D
        result.ReadPixels(new Rect(0, 0, targetWidth, targetHeight), 0, 0);
        result.Apply();

        // Clean up
        RenderTexture.active = null;
        rt.Release();

        return result;
    }
    #endregion

    #region Saving to gallery methods.
    public void StartSaveProcess(bool isCVDMode)
    {
        if (Gallery_Displayer.texture is Texture2D pixelatedTexture)
        {
            if (isCVDMode)
            {
                StartCoroutine(SavePixelatedImageForCVDModeCoroutine(pixelatedTexture));
            }
            else
            {
                StartCoroutine(SaveImageCoroutine(ResizeTexture(pixelatedTexture, 424, 424)));
            }
        }
        else
        {
            Debug.LogError("No pixelated image found to save.");
        }
    }
    private IEnumerator SavePixelatedImageForCVDModeCoroutine(Texture2D pixelatedImage)
    {
        Dictionary<Color, Texture2D> colorToIconMap = InitializeColorToIconMap();
        Texture2D finalImage = OverlayIcons(pixelatedImage, colorToIconMap);

        yield return new WaitForEndOfFrame(); // Wait for the image processing to complete

        // Save the final image
        yield return StartCoroutine(SaveImageCoroutine(finalImage));
    }

    private IEnumerator SaveImageCoroutine(Texture2D image)
    {
        byte[] bytes = image.EncodeToPNG();

        string folderName = "My CVD Bead Art";
        string filePrefix = "CVD_PixelatedImage";
        string fileName = $"{filePrefix}_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.png";

        NativeGallery.Permission permission = NativeGallery.SaveImageToGallery(bytes, folderName, fileName, (success, path) =>
        {
            Debug.Log($"Image saved: {success} at path: {path}");
        });

        if (permission != NativeGallery.Permission.Granted)
        {
            Debug.LogError("Permission to access gallery was not granted.");
        }

        yield return null;
    }
    #endregion
    private Texture2D OverlayIcons(Texture2D pixelatedImage, Dictionary<Color, Texture2D> colorToIconMap)
    {
        int iconSize = 150; // Size of each icon
        int gridSize = 29;  // Size of the grid
        Texture2D finalImage = new Texture2D(gridSize * iconSize, gridSize * iconSize);

        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                Color pixelColor = pixelatedImage.GetPixel(x, y);
                Color closestColor = FindClosestColor(pixelColor, palette);
                if (!colorToIconMap.TryGetValue(closestColor, out Texture2D icon))
                {
                    Debug.LogWarning($"No icon found for color: {closestColor}. Skipping this pixel. Coordinates: ({x},{y})");
                    continue;
                }

                Color[] iconPixels = icon.GetPixels();
                finalImage.SetPixels(x * iconSize, y * iconSize, icon.width, icon.height, iconPixels);
            }
        }

        finalImage.Apply();
        return finalImage;
    }
    private Color FindClosestColor(Color target, Color[] palette)
    {
        Color closestColor = palette[0];
        float minDistance = Mathf.Infinity;

        foreach (Color color in palette)
        {
            float distance = ColorDistance(target, color);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestColor = color;
            }
        }

        return closestColor;
    }

    private float ColorDistance(Color c1, Color c2)
    {
        // Euclidean distance in RGB space
        return Mathf.Sqrt((c1.r - c2.r) * (c1.r - c2.r) +
                          (c1.g - c2.g) * (c1.g - c2.g) +
                          (c1.b - c2.b) * (c1.b - c2.b));
    }
    private Texture2D Pallet_To_Picture(Texture2D image, Color[] plaette)
    {
        Color[] pixels = image.GetPixels();
        for (int i = 0; i< pixels.Length; i++)
        {
            pixels[i] = Calculate_Closets_Colors(pixels[i], palette);
        }
        Texture2D Color_Calculated_Pixture = new Texture2D(image.width, image.height);
        Color_Calculated_Pixture.SetPixels(pixels);
        Color_Calculated_Pixture.Apply();
        return Color_Calculated_Pixture;
    }
    private Color Calculate_Closets_Colors(Color picture_original, Color[] palette)
    {
        Color closestColor = palette[0];
        float minDistance = Mathf.Infinity;
        foreach (Color platetteColor in palette)
        {
            float distance = ColorDistance(picture_original, platetteColor);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestColor = platetteColor;
            }
        }
        return closestColor;
    }
    private float Color_Rbg_Distance(Color c1, Color c2)
    {
        return Mathf.Sqrt((c1.r - c2.r) * (c1.r - c2.r)
                        + (c1.g - c2.g) * (c1.g - c2.g)
                        + (c1.b - c2.b) * (c1.b - c2.b));
    }

}