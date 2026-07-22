using UnityEngine;

public class CameraAspectRatio : MonoBehaviour
{
    private int _screenSizeX = 0;
    private int _screenSizeY = 0;
    private Camera _camera;

    private void RescaleCamera()
    {
        if (Screen.width == _screenSizeX && Screen.height == _screenSizeY) return;

        float targetAspect = 16.0f / 9.0f;
        float windowAspect = Screen.width / (float)Screen.height;
        float scaleHeight = windowAspect / targetAspect;
        float scalewidth = 1.0f / scaleHeight;
        Rect rect = _camera.rect;

        if (scaleHeight < 1.0f) // Add letterbox (black bars on top/bottom)
        {
            rect.width = 1.0f;
            rect.height = scaleHeight;
            rect.x = 0;
            rect.y = (1.0f - scaleHeight) / 2.0f;
        }
        else // Add pillarbox (black bars on sides)
        {
            rect.width = scalewidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scalewidth) / 2.0f;
            rect.y = 0;

            _camera.rect = rect;
        }

        _camera.rect = rect;
        _screenSizeX = Screen.width;
        _screenSizeY = Screen.height;
    }

    private void Awake()
    {
        _camera = GetComponent<Camera>();
    }

    void Update()
    {
        RescaleCamera();
    }

    public void OnDisable()
    {
        Rect rect = _camera.rect;

        rect.width = 1;
        rect.height = 1;
        rect.x = 0;
        rect.y = 0;

        _camera.rect = rect;

        _screenSizeX = 0;
        _screenSizeY = 0;
    }
}