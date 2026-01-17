using UnityEngine;

public class SimpleProjectile : MonoBehaviour
{
    public float lifeTime = 5f;
    public GameObject hitVfx;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (hitVfx)
        {
            var v = Instantiate(hitVfx, collision.GetContact(0).point, Quaternion.identity);
            Destroy(v, 2f);
        }

        Destroy(gameObject);
    }
}