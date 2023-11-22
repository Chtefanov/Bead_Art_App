using UnityEngine;
using Wilberforce;

public class Color_Blind_Controller : MonoBehaviour
{
    private Colorblind colorblindEffect;
    void Start()
    {
        // Get the Colorblind component from the main camera
        colorblindEffect = Camera.main.GetComponent<Colorblind>();
        if (colorblindEffect == null)
        {
            Debug.LogError("Colorblind component not found on the main camera!");
        }
    }

    public void ChangeColorblindMode(int mode)
    {
        if (colorblindEffect != null)
        {
            colorblindEffect.Type = mode;
        }
    }
}
