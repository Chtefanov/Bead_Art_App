using UnityEngine;

public class Native_Camera_Script : MonoBehaviour
{
    private int cameraMaxSize = -1; // Set based on your requirement
    private bool shouldTakePicture = false;
    private string lastImagePath = null;

    void Update()
    {
        if (shouldTakePicture && !NativeCamera.IsCameraBusy())
        {
            shouldTakePicture = false;
            TakePicture();
        }

        if (lastImagePath != null)
        {
            // Process and save the image after the camera interface is closed
            ProcessAndSaveImage(lastImagePath);
            lastImagePath = null;
        }
    }
    private void TakePicture()
    {
        NativeCamera.TakePicture((path) =>
        {
            if (!string.IsNullOrEmpty(path))
            {
                lastImagePath = path; // Store the path for later processing
            }
            else
            {
                Debug.Log("No image was taken, or operation was canceled.");
            }
        }, cameraMaxSize);
    }
    private void ProcessAndSaveImage(string imagePath)
    {
        Texture2D texture = NativeCamera.LoadImageAtPath(imagePath, cameraMaxSize);
        NativeGallery.SaveImageToGallery(texture, "GalleryTest", "My img {0}.png");
        Debug.Log("Image saved to gallery: " + imagePath);
        // To avoid memory leaks
        Destroy(texture);
    }
    public void ActivateCamera()
    {
        shouldTakePicture = true;
    }
}
