using UnityEngine.UI;
using UnityEngine;

public class Pixelator_2 : MonoBehaviour
{
    public RawImage Gallery_Displayer;
    public AspectRatioFitter Fit_image;
    public GameObject Rotate_Picture_Button;
    public GameObject Select_A_Picture_Button;

    private RectTransform Dimensions_Fitting;

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
        }
        if (Fit_image != null)
        {
            Fit_image.aspectRatio = 0.64f;
        }
        Fit_RawImage_To_ParentSize();
        UpdateButtonVisibility();
    }
    private void Fit_RawImage_To_ParentSize()
    {
        if (Gallery_Displayer != null && Dimensions_Fitting != null)
        {
            float parentHeight = Dimensions_Fitting.rect.height;
            float parentWidth = Dimensions_Fitting.rect.width;

            Gallery_Displayer.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, parentWidth);
            Gallery_Displayer.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, parentHeight);
        }
    }
    public void UpdateButtonVisibility()
    {
        if (Gallery_Displayer != null && Gallery_Displayer.texture != null &&
            Gallery_Displayer.texture.width > 0 && Gallery_Displayer.texture.height > 0)
        {
            // Picture is present, show the Rotate_Picture_Button
            Rotate_Picture_Button.SetActive(true);
            Select_A_Picture_Button.SetActive(false);
        }
        else
        {
            // No picture, show the Select_A_Picture_Button
            Rotate_Picture_Button.SetActive(false);
            Select_A_Picture_Button.SetActive(true);
        }
    }
}