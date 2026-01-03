using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A quick component that will push any colliders that have one of the valid tags.
/// </summary>
public class EnemyPusher : MonoBehaviour {
	[Header("Settings")]
	[Tooltip("Any colliders with these tags will be pushed.")]
	[SerializeField] List<string> validTags;
	[Tooltip("Force applied on collision.")]
	[SerializeField] float force = 5000;

	void OnCollisionEnter(Collision collision) {
		if (!validTags.Contains (collision.collider.tag)) {
			return;
		}

		if (collision.collider != null && collision.collider.attachedRigidbody != null) {
			collision.collider.attachedRigidbody.AddForce (transform.forward * force);
		}
	}
}
