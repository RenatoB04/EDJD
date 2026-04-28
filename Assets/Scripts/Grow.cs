using UnityEngine;

public class Grow : MonoBehaviour
{
    public MeshRenderer targetRenderer;
    public Transform playerFPS;

    [Header("Growth Settings")]
    public float maxDistance = 15f; // Distance at which growth begins
    public float minDistance = 3f;  // Distance for maximum growth (G = 1)

    void Update()
    {
        if (targetRenderer != null && playerFPS != null)
        {
            // 1. Calculate the spatial distance between the tree and the player
            float dist = Vector3.Distance(transform.position, playerFPS.position);

            // 2. Convert the distance into a value between 0 and 1 (Inverted: closer = higher G)
            // Mathf.Clamp01 ensures the value never drops below 0 or exceeds 1
            float gValue = 1.0f - Mathf.Clamp01((dist - minDistance) / (maxDistance - minDistance));

            // 3. Send the calculated value to the Shader
            targetRenderer.material.SetFloat("_G", gValue);
        }
    }
}