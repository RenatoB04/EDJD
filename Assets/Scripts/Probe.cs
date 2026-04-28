using UnityEngine;

public class Probe : MonoBehaviour
{
    // Reference to the Reflection Probe located on the player capsule
    public ReflectionProbe probe;
    private Material sphereMaterial;

    void Start()
    {
        // Cache the material of the sphere during initialization
        Renderer rend = GetComponent<Renderer>();
        if (rend != null)
        {
            sphereMaterial = rend.material;
        }
    }

    void Update()
    {
        // Ensure both the probe and the material are properly assigned
        if (probe != null && sphereMaterial != null)
        {
            // Assign the real-time generated texture from the probe 
            // to the _Cube property of our WorldReflex shader
            sphereMaterial.SetTexture("_Cube", probe.realtimeTexture);
        }
    }
}