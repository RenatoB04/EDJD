using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class EmergencyPostProcessController : MonoBehaviour
{
    [Header("Shader Reference")]
    [Tooltip("The EmergencyPostProcessing shader file.")]
    public Shader postProcessShader;

    private Material postProcessMaterial;

    [Header("Emergency Settings")]
    [Range(0.0f, 1.0f)]
    [Tooltip("0 = disabled, 1 = full screen glitch and vignette effect.")]
    public float emergencyIntensity = 0.0f;

    [Range(0.0f, 0.1f)]
    [Tooltip("Maximum scale of horizontal noise screen displacement.")]
    public float glitchScale = 0.03f;

    [Range(0.0f, 0.05f)]
    [Tooltip("Offset distance for RGB split chromatic aberration.")]
    public float chromaticAberration = 0.015f;

    [Tooltip("Warning border glow color.")]
    public Color emergencyColor = new Color(0.8f, 0.0f, 0.0f, 1.0f);

    void Start()
    {
        if (postProcessShader == null)
            postProcessShader = Shader.Find("Custom/EmergencyPostProcessing");
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (postProcessShader == null || emergencyIntensity <= 0.0f)
        {
            Graphics.Blit(source, destination);
            return;
        }

        if (postProcessMaterial == null || postProcessMaterial.shader != postProcessShader)
        {
            postProcessMaterial = new Material(postProcessShader);
            postProcessMaterial.hideFlags = HideFlags.DontSave;
        }

        postProcessMaterial.SetFloat("_EmergencyIntensity", emergencyIntensity);
        postProcessMaterial.SetFloat("_GlitchScale", glitchScale);
        postProcessMaterial.SetFloat("_ChromaticAberration", chromaticAberration);
        postProcessMaterial.SetColor("_EmergencyColor", emergencyColor);

        Graphics.Blit(source, destination, postProcessMaterial);
    }

    void OnDisable()
    {
        if (postProcessMaterial != null)
        {
            DestroyImmediate(postProcessMaterial);
        }
    }
}
