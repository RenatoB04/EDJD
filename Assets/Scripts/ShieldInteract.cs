using UnityEngine;
using System.Collections;

public class ShieldInteract : MonoBehaviour
{
    private Material shieldMaterial;
    private Coroutine rippleCoroutine;

    [Header("Ripple Settings")]
    public float maxRadius = 3.0f;
    public float rippleSpeed = 5.0f;

    void Start()
    {
        shieldMaterial = GetComponent<Renderer>().material;

        // Initialise ripple state as inactive
        shieldMaterial.SetFloat("_HitRadius", -1000f);
        shieldMaterial.SetFloat("_MaxRadius", maxRadius);
    }

    void Update()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.transform == transform)
            {
                TriggerHit(hit.point);
            }
        }
    }

    public void TriggerHit(Vector3 hitPoint)
    {
        shieldMaterial.SetVector("_HitPos", hitPoint);

        // Restart ripple if already active
        if (rippleCoroutine != null)
            StopCoroutine(rippleCoroutine);

        rippleCoroutine = StartCoroutine(AnimateRipple());
    }

    IEnumerator AnimateRipple()
    {
        float currentRadius = 0f;

        while (currentRadius < maxRadius)
        {
            currentRadius += Time.deltaTime * rippleSpeed;
            shieldMaterial.SetFloat("_HitRadius", currentRadius);
            yield return null;
        }

        // Reset state (disable ripple effect in shader)
        shieldMaterial.SetFloat("_HitRadius", -1000f);
    }
}