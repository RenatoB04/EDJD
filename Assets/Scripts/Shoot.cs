using UnityEngine;
using UnityEngine.InputSystem;

public class Shooter : MonoBehaviour
{
    [Header("Refs")]
    public Camera cam;
    public Transform firePoint;
    public GameObject projectilePrefab;
    public GameObject muzzlePrefab;

    [Header("Disparo")]
    public InputActionReference fireAction;
    public float projectileSpeed = 30f;
    public float fireRate = 4f;
    public float spread = 1f;

    private float _nextFireTime;

    void OnEnable()
    {
        if (fireAction) fireAction.action.Enable();
        if (!cam) cam = Camera.main;
    }

    void OnDisable()
    {
        if (fireAction) fireAction.action.Disable();
    }

    void Update()
    {
        bool firing = fireAction && fireAction.action.IsPressed();
        if (firing && Time.time >= _nextFireTime)
        {
            _nextFireTime = Time.time + (1f / fireRate);
            Shoot();
        }
    }

    void Shoot()
    {
        if (!cam || !firePoint) return;
        
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        Vector3 targetPoint;

        if (Physics.Raycast(ray, out RaycastHit hit, 1000f))
            targetPoint = hit.point;
        else
            targetPoint = ray.GetPoint(1000f);
        
        Vector3 dir = (targetPoint - firePoint.position).normalized;
        if (spread > 0f)
        {
            dir = Quaternion.Euler(
                Random.Range(-spread, spread),
                Random.Range(-spread, spread),
                0f
            ) * dir;
        }
        
        GameObject proj = projectilePrefab ? 
            Instantiate(projectilePrefab, firePoint.position, Quaternion.LookRotation(dir)) :
            CreateRuntimeSphereProjectile(firePoint.position, dir);
        
        if (proj.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.linearVelocity = dir * projectileSpeed;
        }

        if (muzzlePrefab)
        {
            var m = Instantiate(muzzlePrefab, firePoint.position, firePoint.rotation);
            Destroy(m, 2f);
        }
    }
    
    GameObject CreateRuntimeSphereProjectile(Vector3 pos, Vector3 dir)
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = pos;
        sphere.transform.localScale = Vector3.one * 0.15f;

        var rb = sphere.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        var sp = sphere.AddComponent<SimpleProjectile>();
        sp.lifeTime = 5f;
        
        var rend = sphere.GetComponent<Renderer>();
        if (rend && rend.sharedMaterial)
        {
            rend.sharedMaterial.color = Color.cyan;
        }
        
        return sphere;
    }
}
