using UnityEngine;

[ExecuteInEditMode]
public class EnableDepthBuffer : MonoBehaviour
{
    void OnEnable()
    {
        var cam = GetComponent<Camera>();
        if (!cam) return;

        // Preserve any existing depth modes while adding the depth texture required by the shield shader.
        cam.depthTextureMode |= DepthTextureMode.Depth;
    }
}
