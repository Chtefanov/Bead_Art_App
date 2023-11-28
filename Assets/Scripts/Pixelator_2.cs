using UnityEngine.UI;
using UnityEngine;

public class Pixelator_2 : MonoBehaviour
{
    public RawImage Gallery_Displayer;
    public AspectRatioFitter Fit_image;
    //Buttoons under "initial buttons" as individual gameobjects, for 
    //making changes to UI regarding pixelator containing picture or not
    public GameObject Rotate_Picture_Button;
    public GameObject Select_A_Picture_Button;
    //The complete set of buttons as gameobject for deactivationo purposes 
    //when pixelation of picture is going on.

    private RectTransform Dimensions_Fitting;
    private const float DefaultAspectRatio = 0.64f;
    // Flag to track panel activation


    private void OnEnable()
    {
        // When the pixelator panel is enabled, reset it if it's active
        ResetPixelatorPanel();
    }
    public void ResetPixelatorPanel()
    {
        if (Gallery_Displayer != null)
        {
            Gallery_Displayer.texture = null;
            if (Fit_image != null)
            {
                Fit_image.aspectRatio = DefaultAspectRatio; // Default aspect ratio
                Fit_image.enabled = false;
            }
        }
        UpdateButtonVisibility();
    }
    public void LoadImage(Texture2D image)
    {
        if (Gallery_Displayer != null)
        {
            Gallery_Displayer.texture = image;
            if (Fit_image != null)
            {
                float imageAspectRatio = (float)image.width / image.height;
                Fit_image.aspectRatio = Mathf.Min(imageAspectRatio, DefaultAspectRatio);
                Fit_image.enabled = true;
            }
        }
        UpdateButtonVisibility();
    }
    public void UpdateButtonVisibility()
    {
        bool isImagePresent = Gallery_Displayer != null && Gallery_Displayer.texture != null;
        Rotate_Picture_Button.SetActive(isImagePresent);
        Select_A_Picture_Button.SetActive(!isImagePresent);
    }

    public void Rotate_By_90Degrees()
    {
        if (Gallery_Displayer != null && Gallery_Displayer.texture != null)
        {
            Gallery_Displayer.rectTransform.Rotate(0, 0, -90);
            if (Fit_image != null)
            {
                Fit_image.aspectRatio = 1f / Fit_image.aspectRatio;
            }
        }
    }
}