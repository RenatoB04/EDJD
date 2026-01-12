using UnityEngine;
using System.Collections;
public class ImpactScript : MonoBehaviour {
	[Header("Impact Despawn Timer")]
	public float despawnTimer = 10.0f;
	[Header("Audio")]
	public AudioClip[] impactSounds;
	public AudioSource audioSource;
	private void Start () {
		StartCoroutine (DespawnTimer ());
		audioSource.clip = impactSounds
			[Random.Range(0, impactSounds.Length)];
		audioSource.Play();
	}
	private IEnumerator DespawnTimer() {
		yield return new WaitForSeconds (despawnTimer);
		Destroy (gameObject);
	}
}