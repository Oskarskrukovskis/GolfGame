using UnityEngine;

public class CameraAdjusterManager : MonoBehaviour
{
    public Camera mainCamera;
    public float referenceWidth = 1080f;  
    public float referenceHeight = 1920f; 
    public float referenceOrthoSize = 5f; 

    private void Start()
    {
        AdjustCameraSize();
    }

    //Adjusts the camera size based on the screen.
    private void AdjustCameraSize()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        float screenAspect = (float)Screen.width / Screen.height;
        float referenceAspect = referenceWidth / referenceHeight;

        if (screenAspect >= referenceAspect)
        {
            mainCamera.orthographicSize = referenceOrthoSize;
        }
        else
        {
            float scaleFactor = referenceAspect / screenAspect;
            mainCamera.orthographicSize = referenceOrthoSize * scaleFactor;
        }
    }

}