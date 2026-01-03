using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A resource is an interactable object that creates materials when hit.
/// </summary>
public class Resource : MonoBehaviour {
	[Header("Prefabs")]
	[Tooltip("Reference to the object that this resource will spawn")]
	[SerializeField] GameObject materialPrefab;

	[Header("Object References")]
	[Tooltip("Reference to the damageable entity.")]
	[SerializeField] DamageableEntity damageableEntity;
	[Tooltip("Reference to the UI canvas.")]
	[SerializeField] GameObject ui;
	[Tooltip("Reference to hit points bar.")]
	[SerializeField] ProgressBarUI progressBar;
	[Tooltip("Reference to rigidbody of the resource.")]
	[SerializeField] Rigidbody resourceRigidbody;
	[Tooltip("Reference to the list of all colliders of the resource.")]
	[SerializeField] List<Collider> resourceColliders;
	[Tooltip("Reference a transform that indicates where materials will be spawned at.")]
	[SerializeField] Transform releasePoint;

	[Header("Settings")]
	[Tooltip("Starting hitpoints of this resource.")]
	[SerializeField] int startingHitPoints = 100;
	[Tooltip("How much damage needs to be dealt for a material to be spawned")]
	[SerializeField] int damageNeededForSpawn = 20;
	[Tooltip("Whether physics should be added to the object when it dies.")]
	[SerializeField] bool addPhysicsOnEnd;
	[Tooltip("Whether to release all materials at the end.")]
	[SerializeField] bool releaseAllOnEnd;
	[Tooltip("How long until the object fully dissapears.")]
	[SerializeField] float timeToDie;

	[Header("Read Only")]
	[Tooltip("Current damage dealt to this object.")]
	[SerializeField] float damageThreshold = 0;
	[Tooltip("Whether this resource ran out of hitpoints.")]
	[SerializeField][ReadOnly] bool dead;

	void Start() {
		damageableEntity.Health.Current = damageableEntity.Health.CurrentMaximum = startingHitPoints;
		damageThreshold = 0;

		progressBar.Fill.fillAmount = 1;
	}

	/// <summary>
	/// Handles what happens when damage is taken.
	/// Sets the progress bar to the right fill.
	/// Checks for death, in which case it will add physics and spawn materials.
	/// </summary>
	public void OnDamageTaken(float current, float maximum, float difference) {
		if (dead) {
			return;
		}

		progressBar.Fill.fillAmount = current / maximum;

		damageThreshold += difference;

		if (Mathf.Approximately(current, 0)) {
			dead = true;

			ui.gameObject.SetActive (false);

			Destroy (gameObject, timeToDie);

			gameObject.layer = LayerMask.NameToLayer ("Void");
			for (int i = 0; i < resourceColliders.Count; i++) {
				resourceColliders[i].gameObject.layer = LayerMask.NameToLayer ("Void");
			}
				
			if (addPhysicsOnEnd) {
				resourceRigidbody.isKinematic = false;
				resourceRigidbody.useGravity = true;

				GameObject player = GameObject.FindGameObjectWithTag ("Player");
				Vector3 direction = transform.position - player.transform.position;
				direction.Normalize ();

				resourceRigidbody.velocity = direction * 3;
			}

			if (releaseAllOnEnd) {
				while (damageThreshold - damageNeededForSpawn >= 0) {
					damageThreshold -= damageNeededForSpawn;

					GameObject mat = Instantiate (materialPrefab, releasePoint.position + Random.insideUnitSphere * 1f, Quaternion.identity);
					mat.transform.forward = mat.transform.position - releasePoint.position;
					Rigidbody rigidbody = mat.GetComponent<Rigidbody> ();
					rigidbody.velocity = (mat.transform.position - releasePoint.position).normalized * 4f;
				}
			}
		}
	}

	/// <summary>
	/// Should be called whenever a component hits this resource. Only specialized components can hit different resources.
	/// </summary>
	public void OnHit(Collision collision) {
		if (damageableEntity.ValidTags.Contains(collision.collider.tag) && !releaseAllOnEnd) {
			while (damageThreshold - damageNeededForSpawn >= 0) {
				damageThreshold -= damageNeededForSpawn;

				Vector3 direction = collision.collider.gameObject.transform.position - transform.position;
				Instantiate (materialPrefab, transform.position + direction * 1.1f, Quaternion.identity);
			}
		}
	}

	/// <summary>
	/// Should be called whenever a component hits this resource. Only specialized components can hit different resources.
	/// </summary>
	public void OnHit(Collider collider) {
		if (damageableEntity.ValidTags.Contains(collider.tag) && !releaseAllOnEnd) {
			while (damageThreshold - damageNeededForSpawn >= 0) {
				damageThreshold -= damageNeededForSpawn;

				Vector3 direction = collider.gameObject.transform.position - transform.position;
				Instantiate (materialPrefab, transform.position + direction * 1.1f, Quaternion.identity);
			}
		}
	}

	/// <summary>
	/// Should be called whenever this resource enters the range of the player
	/// </summary>
	public void OnBecomeInRange() {
		if (!dead) {
			ui.gameObject.SetActive (true);
		}
	}

	/// <summary>
	/// Should be called whenever this resource exits the range of the player
	/// </summary>
	public void OnBecomeOutOfRange() {
		ui.gameObject.SetActive (false);
	}
}
