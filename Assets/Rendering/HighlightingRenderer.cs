using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// When this component is attached to the camera, it can highlight object.
/// For specifying which objects should be highlighted, use Highlighter component.
/// </summary>
[AddComponentMenu("Rendering/Highlighting Renderer")]
[RequireComponent(typeof(Camera))]
public class HighlightingRenderer : MonoBehaviour
{
    [Tooltip("Shader that is used to render highlights")]
    public Shader highlightShader = null;
    [Tooltip("Shader that is used to blend highlight and the original render from camera")]
    public Shader blendShader = null;
    [Tooltip("Tag to determine that objects is highlighted")]
    public string replacementTag = "Highlight";
    [Tooltip("Which layers participate in rendering")]
    public LayerMask cullingMask;

    private Material material = null;
    private RenderTexture renderTexture = null;
    private Camera highlightCamera = null;
    private GameObject highlightCameraObject = null;
    private int currentResolutionX = 0;
    private int currentResolutionY = 0;

    void Start()
    {
        if (highlightShader && blendShader)
        {
            Camera camera = GetComponent<Camera>();

            // Create new material for image effects
            material = new Material(blendShader);
            material.name = "HighlightMaterial";
            material.hideFlags = HideFlags.HideAndDontSave;

            // Create additional camera to render highlighting
            highlightCameraObject = new GameObject("_Camera");
            highlightCameraObject.transform.SetParent(transform, false);
            highlightCamera = highlightCameraObject.AddComponent<Camera>();
            highlightCameraObject.hideFlags = HideFlags.HideAndDontSave;

            highlightCamera.CopyFrom(camera);
            highlightCamera.targetTexture = renderTexture;
            highlightCamera.SetReplacementShader(highlightShader, replacementTag);
            highlightCamera.enabled = false;
            highlightCamera.cullingMask = cullingMask;

            RefreshRenderTexture();
        }
        else
        {
            Debug.LogError("HighlightingRenderer | " +
                "Shader is not assigned. Disabling image effect.", gameObject);
            enabled = false;
        }
    }


    void RefreshHighlightCamera()
    {
        if (highlightCamera)
        {
            Camera camera = GetComponent<Camera>();

            // Update orthographic size
            highlightCamera.orthographicSize = camera.orthographicSize;
            // Position is updated automatically, since this camera
            // is attached to the same transform
        }
        else
        {
            Debug.LogWarning("HighlightingRenderer | " +
                "Trying to refresh highlight camera, but it does not exist", gameObject);
        }
    }


    void RefreshRenderTexture()
    {
        Camera camera = GetComponent<Camera>();

        if (camera.pixelWidth == currentResolutionX &&
            camera.pixelHeight == currentResolutionY)
        {
            return;
        }
        currentResolutionX = camera.pixelWidth;
        currentResolutionY = camera.pixelHeight;

        if (renderTexture)
            renderTexture.Release();
        renderTexture = new RenderTexture(currentResolutionX, currentResolutionY, 24);

        if (highlightCamera)
        {
            highlightCamera.targetTexture = renderTexture;
        }

        if (material)
        {
            material.SetTexture("_AdditionalTex", renderTexture);
            material.SetVector("_AdditionalTex_TexelSize", renderTexture.texelSize);
        }
        else
        {
            Debug.LogWarning("HighlightingRenderer | " +
                "Trying to setup the material uniforms, but the material does not exist", gameObject);
        }
    }


    private void Update()
    {
        RefreshRenderTexture();
        RefreshHighlightCamera();
    }


    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (highlightShader != null &&
            material != null &&
            renderTexture != null)
        {
            // Render highlights to texture
            RenderTexture.active = highlightCamera.targetTexture;
            highlightCamera.enabled = true;
            highlightCamera.Render();
            highlightCamera.enabled = false;
            // Merge this texture with the default render
            Graphics.Blit(source, destination, material);
        }
        else
        {
            Graphics.Blit(source, destination);
            Debug.LogWarning("HighlightingRenderer | " +
                "Shader is not assigned. Disabling image effect.", gameObject);
            enabled = false;
        }
    }


    void OnDisable()
    {
        if (material)
        {
            DestroyImmediate(material);
            DestroyImmediate(highlightCameraObject);
            DestroyImmediate(renderTexture);
        }
    }
}
