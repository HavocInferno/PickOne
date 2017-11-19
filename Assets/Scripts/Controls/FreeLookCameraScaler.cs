using UnityEngine;
using UnityStandardAssets.Cameras;

[AddComponentMenu("Controls/FreeCameraLookScaler")]
/// <summary>
/// Scales a view from FreeLookCam using mouse wheel input.
/// </summary>
public class FreeLookCameraScaler : MonoBehaviour
{
    [SerializeField]
    private FreeLookCam freeLookCamera;
    [SerializeField]
    private float sensivity = 1.0f;
    [SerializeField]
    private float updateSpeed = 0.1f;
    [SerializeField]
    private float minScale = 0.1f;
    [SerializeField]
    private float maxScale = 10.0f;
    [SerializeField]
    private float maxCameraMoveSpeed = 10.0f;

    private Vector3 originalScale;
    private float originalCameraMoveSpeed;
    private float targetScale = 1.0f;
    private float currentScale = 1.0f;

    private void Start()
    {
        originalScale = transform.localScale;
        originalCameraMoveSpeed = freeLookCamera.m_MoveSpeed;
    }

    private void Update()
    {
        float value = Input.GetAxis("Mouse ScrollWheel");
        targetScale = Mathf.Clamp(targetScale + value * sensivity, minScale, maxScale);
        currentScale = Mathf.Lerp(currentScale, targetScale, updateSpeed);
        transform.localScale = Mathf.Pow(2.0f, currentScale) * 0.5f * originalScale;
        freeLookCamera.m_MoveSpeed = Mathf.Min(originalCameraMoveSpeed / currentScale, maxCameraMoveSpeed);
    }
}
