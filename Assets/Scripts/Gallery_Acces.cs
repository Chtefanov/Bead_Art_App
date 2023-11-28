using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gallery_Acces : MonoBehaviour
{
    //Display of picture
    public RawImage Gallery_Display;

    private AspectRatioFitter aspectRatioFitter;
    private Texture2D Some_Picture_From_Gallery;
    //Initial UI buttons for "Pixelator panel" in context of picture displayed/ not displayed
    public GameObject Select_A_Picture_Button;
    public GameObject Rotate_The_Picture_Button;

    public void PickImage(int maxSize)
    {
        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
        {
            Debug.Log("Image path: " + path);
            if (path != null)
            {
                // Creating Texture from selected image
                Some_Picture_From_Gallery = NativeGallery.LoadImageAtPath(path, maxSize, false);
                if (Some_Picture_From_Gallery == null)
                {
                    Debug.Log("Couldn't load texture from " + path);
                    return;
                }

                // Adjusting the aspect ratio
                if (aspectRatioFitter != null)
                {
                    aspectRatioFitter.aspectRatio = (float)Some_Picture_From_Gallery.width / Some_Picture_From_Gallery.height;
                }

                // Assigingn texture to the RawImage component
                if (Gallery_Display != null)
                {
                    Gallery_Display.texture = Some_Picture_From_Gallery;
                }
                //If picture is contain wihtin "RawImage" GameComponent.

                if (Some_Picture_From_Gallery != null)
                {
                    Select_A_Picture_Button.SetActive(false);
                    Rotate_The_Picture_Button.SetActive(true);
                }
                else
                {
                    Select_A_Picture_Button.SetActive(true);
                    Rotate_The_Picture_Button.SetActive(false);
                }
            }
        });
    }
}

