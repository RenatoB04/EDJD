using UnityEngine;

public class ShieldGeneratorController : MonoBehaviour
{
    private Material generatorMaterial;
    private float currentActiveState = 0f;

    [Header("Generator Settings")]
    [Tooltip("How fast the physical displacement deforms when activating/deactivating.")]
    public float transitionSpeed = 2.0f;

    [Tooltip("Is the shield currently powered on?")]
    public bool isShieldActive = true;

    void Start()
    {
        Renderer rend = GetComponent<Renderer>();
        if (rend != null)
        {
            // Use an instanced material so each generator can animate independently.
            generatorMaterial = rend.material;
        }
    }

    void Update()
    {
        if (generatorMaterial == null) return;

        float target = isShieldActive ? 1.0f : 0.0f;
        currentActiveState = Mathf.MoveTowards(currentActiveState, target, Time.deltaTime * transitionSpeed);

        generatorMaterial.SetFloat("_ActiveState", currentActiveState);
    }

    public void SetShieldActive(bool active)
    {
        isShieldActive = active;
    }
}
