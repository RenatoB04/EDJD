using UnityEngine;

// Enables depth texture generation for this camera.
// Useful for screen-space effects that rely on scene depth.
[ExecuteInEditMode]
public class EnableDepthBuffer : MonoBehaviour
{
    void OnEnable()
    {
        var cam = GetComponent<Camera>();
        if (!cam) return;

        // Ensure depth texture is available for shaders
        cam.depthTextureMode |= DepthTextureMode.Depth;
    }
}