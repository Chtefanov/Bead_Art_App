using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gallery_Acces : MonoBehaviour
{
    public Pixelator_2 pixelator;
    //Display of picture
    public RawImage Gallery_Display;

    //private AspectRatioFitter aspectRatioFitter;
    private Texture2D Some_Picture_From_Gallery;
    //Initial UI buttons for "Pixelator panel" in context of picture displayed/ not displayed
    //public GameObject Select_A_Picture_Button;
    //public GameObject Pixelator_Picture_Button;
    public void PickImage(int maxSize)
    {
        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
        {
            if (!string.IsNullOrEmpty(path)) // Check if the path is not null or empty
            {   
                Some_Picture_From_Gallery = NativeGallery.LoadImageAtPath(path, maxSize, false);
                if (Some_Picture_From_Gallery == null)
                {
                    return;
                }
                /*if (aspectRatioFitter != null)
                {
                    aspectRatioFitter.aspectRatio = (float)Some_Picture_From_Gallery.width / Some_Picture_From_Gallery.height;
                }*/
                if (Gallery_Display != null)
                {
                    Gallery_Display.texture = Some_Picture_From_Gallery;
                    if (pixelator != null)
                    {
                        pixelator.ImageSelected(Some_Picture_From_Gallery);
                        Debug.Log("Image ready for pixelation");
                    }
                }
            }
        });
    }
}

