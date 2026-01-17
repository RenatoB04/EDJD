using UnityEngine;
using System.Collections;
public class ProjectileScript : MonoBehaviour {
	private bool explodeSelf;
	[Tooltip("Enable to use constant force, instead of force at launch only")]
	public bool useConstantForce;
	[Tooltip("How fast the projectile moves")]
	public float constantForceSpeed;
	[Tooltip("How long after spawning that the projectile self destructs")]
	public float explodeAfter;
	private bool hasStartedExplode;
	private bool hasCollided;
	[Header("Explosion Prefabs")]
	public Transform explosionPrefab;
	[Header("Customizable Options")]
	[Tooltip("Initial launch force")]
	public float force = 5000f;
	[Tooltip("How long after spawning should the projectile object destroy")]
	public float despawnTime = 30f;
	[Header("Explosion Options")]
	[Tooltip("Explosion radius")]
	public float radius = 50.0F;
	[Tooltip("Explosion intensity")]
	public float power = 250.0F;
	[Header("Rocket Launcher Projectile")]
	[Tooltip("Enabled if the projectile has particle effects")]
	public bool usesParticles;
	public ParticleSystem smokeParticles;
	public ParticleSystem flameParticles;
	[Tooltip("Added delay to let particle effects finish playing, " +
		"before destroying object")]
	public float destroyDelay;
	private void Start () 
	{
		if (!useConstantForce) 
		{
			GetComponent<Rigidbody> ().AddForce 
				(gameObject.transform.forward * force);
		}
		StartCoroutine (DestroyTimer ());
	}
	private void FixedUpdate()
	{
		if(GetComponent<Rigidbody>().linearVelocity != Vector3.zero)
			GetComponent<Rigidbody>().rotation = 
				Quaternion.LookRotation(GetComponent<Rigidbody>().linearVelocity);  
		if (useConstantForce == true && !hasStartedExplode) {
			GetComponent<Rigidbody>().AddForce 
				(gameObject.transform.forward * constantForceSpeed);
			StartCoroutine (ExplodeSelf ());
			hasStartedExplode = true;
		}
	}
	private IEnumerator ExplodeSelf () 
	{
		yield return new WaitForSeconds (explodeAfter);
		if (!hasCollided) {
			Instantiate (explosionPrefab, transform.position, transform.rotation);
		}
		gameObject.GetComponent<MeshRenderer> ().enabled = false;
		gameObject.GetComponent<Rigidbody> ().isKinematic = true;
		gameObject.GetComponent<BoxCollider>().isTrigger = true;
		if (usesParticles == true) {
			flameParticles.GetComponent <ParticleSystem> ().Stop ();
			smokeParticles.GetComponent<ParticleSystem> ().Stop ();
		}
		yield return new WaitForSeconds (destroyDelay);
		Destroy (gameObject);
	}
	private IEnumerator DestroyTimer () 
	{
		yield return new WaitForSeconds (despawnTime);
		Destroy (gameObject);
	}
	private IEnumerator DestroyTimerAfterCollision () 
	{
		yield return new WaitForSeconds (destroyDelay);
		Destroy (gameObject);
	}
	private void OnCollisionEnter (Collision collision)
	{
		if (collision.transform.CompareTag("Player"))
			return;
		hasCollided = true;
		gameObject.GetComponent<MeshRenderer> ().enabled = false;
		gameObject.GetComponent<Rigidbody> ().isKinematic = true;
		gameObject.GetComponent<BoxCollider>().isTrigger = true;
		if (usesParticles == true) {
			flameParticles.GetComponent <ParticleSystem> ().Stop ();
			smokeParticles.GetComponent<ParticleSystem> ().Stop ();
		}
		StartCoroutine (DestroyTimerAfterCollision ());
		Instantiate(explosionPrefab,collision.contacts[0].point,
			Quaternion.LookRotation(collision.contacts[0].normal));
		if (collision.gameObject.tag == "Target" && 
		    	collision.gameObject.GetComponent<TargetScript>().isHit == false) {
			Instantiate(explosionPrefab,collision.contacts[0].point,
			            Quaternion.LookRotation(collision.contacts[0].normal));
			collision.gameObject.transform.gameObject.GetComponent
				<TargetScript>().isHit = true;
		}
		Vector3 explosionPos = transform.position;
		Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);
		foreach (Collider hit in colliders)
		{
			if (hit.CompareTag("Player"))
				continue;
			Rigidbody rb = hit.GetComponent<Rigidbody> ();
			if (rb != null)
				rb.AddExplosionForce (power * 50, explosionPos, radius, 3.0F);
			if (hit.GetComponent<Collider>().tag == "Target" && 
			    	hit.GetComponent<TargetScript>().isHit == false) {
				hit.gameObject.GetComponent<TargetScript>().isHit = true;
			}
			if (hit.transform.tag == "ExplosiveBarrel") {
				hit.transform.gameObject.GetComponent<ExplosiveBarrelScript>().explode = true;
			}
			if (hit.GetComponent<Collider>().tag == "GasTank") 
			{
				hit.gameObject.GetComponent<GasTankScript> ().isHit = true;
				hit.gameObject.GetComponent<GasTankScript> ().explosionTimer = 0.05f;
			}
		}
	}
}