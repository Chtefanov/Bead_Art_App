using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System;
using System.Collections;

public class Pixelator_2 : MonoBehaviour
{
    //private enum UISTate { Initial, PictureSelected, AfterPixelation, AcceptedPixelation, SavePixelationMode}
    #region UI Compoenents
    public RawImage Gallery_Displayer;
    //public AspectRatioFitter Fit_image;
    #endregion

    //Initial UI-Buttons
    public GameObject Select_Picture_Button;
    public GameObject Back_To_Main_Menu;
    //Picture Selected UI-Button
    public GameObject Pixelate_Picture_Button;
    //Picture Pixelated UI-Button
    public GameObject Save_Pixelation_Button;
    //Saved Pixelation UI-Buttons
    public GameObject CVD_Mode_No_Button;
    public GameObject CVD_Mode_Yes_Button;
    #region UI Text
    
    public TextMeshProUGUI QuestionText;
    public TextMeshProUGUI Template_CVD_Save;
    public TextMeshProUGUI Template_Saved;
    #endregion
    //Private float set to const due to UI current ui design. 
    private const float DefaultAspectRatio = 0.64f;
    //For pixelation
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
    // Flag to track panel activation
    private UIState currentUIState = UIState.Initial;
    private void OnEnable()
    {
        ResetPixelatorPanel();
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
    public GameObject QuestionTextGameObject; // Reference to the GameObject containing TextMeshProUGUI component
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
    public void Save_Pixelation_Pressed()
    {
        UpdateUI(UIState.SavePixelationMode);
    }
    public void SavePixelatedImage(bool isCVDMode)
    {
        if (Gallery_Displayer.texture is Texture2D pixelatedTexture)
        {
            string folderName = isCVDMode ? "My CVD Bead Art" : "My Bead Art";
            string filePrefix = isCVDMode ? "CVD_PixelatedImage" : "PixelatedImage";
            string fileName = $"{filePrefix}_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.png";

            // Save the texture to the gallery
            NativeGallery.Permission permission = NativeGallery.SaveImageToGallery(pixelatedTexture, folderName, fileName, (success, path) =>
            {
                Debug.Log("Image saved: " + success + " at path: " + path);
            });

            if (permission != NativeGallery.Permission.Granted)
            {
                Debug.Log("Permission to access gallery was not granted");
            }
        }
        else
        {
            Debug.Log("No pixelated image found to save.");
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
            float distance = Color_Rbg_Distance(picture_original, platetteColor);
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
    
    //Needed for native gallery plugin to work.
    public void ImageSelected(Texture2D image)
    {
        UpdateUI(UIState.PictureSelected);
    }
}