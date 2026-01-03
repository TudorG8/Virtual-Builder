using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

/// <summary>
/// Arrows are projectiles are projectiles that are shot and face in the direction they are heading in.
/// </summary>
public class Arrow : MonoBehaviour {
	[Header("Object References")]
	[Tooltip("Reference to the grabbable script")]
	[SerializeField] CustomGrabbable grabbable;
	[Tooltip("Reference to the attached rigidbody")]
	[SerializeField] Rigidbody rigidBody;
	[Tooltip("Reference to the attached collider")]
	[SerializeField] Collider attachedCollider;
	[Tooltip("Reference to the canvas that will display the name when the object is close. ")]
	[SerializeField] GameObject ui;
	[Tooltip("Reference to the text that displays the type field. Will be capitablized")]
	[SerializeField] TextMeshProUGUI titleText;
	[Tooltip("Reference to the offensive entity script attached to this object.")]
	[SerializeField] OffensiveEntity offensiveEntity;

	[Header("Settings")]
	[Tooltip("Size(length) of the arrow, or how much it can be drawn back.")]
	[SerializeField] float size;

	[Header("Read Only")]
	[Tooltip("This will be true once an arrow has been shot")]
	[SerializeField] bool hasBeenShot;
	[Tooltip("Will be set when the arrow is armed")]
	[SerializeField] BowItemBase bow;
	[Tooltip("Will be set to true for one fixed update frame once it hit a collider.")]
	[SerializeField] bool hasHit = false;
	[Tooltip("Location of the gameobject on the last fixed step.")]
	[SerializeField] Vector3 location;
	[Tooltip("Rotation of the gameobject on the last fixed step.")]
	[SerializeField] Quaternion rotation;

	[Header("Events")]
	[Tooltip("Triggered whenever the arrow has been shot by a controller.")]
	[SerializeField] UnityEvent onShoot;

	public bool HasBeenShot {
		get {
			return this.hasBeenShot;
		}
		set {
			hasBeenShot = value;
		}
	}

	public float Size {
		get {
			return this.size;
		}
		set {
			size = value;
		}
	}

	public CustomGrabbable Grabbable {
		get {
			return this.grabbable;
		}
		set {
			grabbable = value;
		}
	}

	public BowItemBase Bow {
		get {
			return this.bow;
		}
		set {
			bow = value;
		}
	}

	public Collider Collider {
		get {
			return this.attachedCollider;
		}
		set {
			attachedCollider = value;
		}
	}

	public Rigidbody RigidBody {
		get {
			return this.rigidBody;
		}
		set {
			rigidBody = value;
		}
	}

	void Update() {
		if (hasBeenShot) {
			// Orientate the arrow in the direction of its velocity.
			transform.rotation = Quaternion.LookRotation (rigidBody.velocity);
		}
	}

	/// <summary>
	/// Keep track of the position on the last fixed update.
	/// </summary>
	void FixedUpdate() {
		if (!hasHit) {
			location = transform.position;
			rotation = transform.rotation;
		} 
		else {
			hasHit = false;
		}
	}

	public void OnGrabBegin() {
		hasBeenShot = false;
		if (bow == null) {
			transform.parent = null;
		}
		rigidBody.constraints = RigidbodyConstraints.None;
		offensiveEntity.HasDealtDamage = false;
	} 

	public void OnBecomeUngrabbed() {
		if (bow) {
			bow.OnArrowUngrabbed ();
		}
	}

	void OnCollisionEnter(Collision collision) {
		if (hasBeenShot) {
			transform.position = location;
			transform.rotation = rotation;

			GameObject target = collision.collider.attachedRigidbody != null ? collision.collider.attachedRigidbody.gameObject : collision.collider.gameObject;
			BecomeStuckIn (target);
		}
	}

	/// <summary>
	/// Allows the arrow to become stuck in an object. 
	/// Stops all physics, destroys the rigidbody and sets the parent of the arrow to that object.
	/// </summary>
	/// <param name="obj">Object.</param>
	public void BecomeStuckIn(GameObject obj) {
		if (hasBeenShot) {
			hasBeenShot = false;
			rigidBody.isKinematic = true;
			rigidBody.velocity = Vector3.zero;
			rigidBody.angularVelocity = Vector3.zero;
			rigidBody.constraints = RigidbodyConstraints.FreezeAll;
			hasHit = true;

			Destroy(rigidBody);

			grabbable.CanBeGrabbed = false;

			transform.SetParent (obj.transform, true);
			transform.position = location;
			transform.rotation = rotation;
		}
	}

	/// <summary>
	/// Called whenever this arrow is shot.
	/// </summary>
	public void OnShoot() {
		StartCoroutine (ShootCoroutine ());
		onShoot.Invoke ();
	}

	/// <summary>
	/// A routine that turns the layer of the arrow to void and then to projectiles after a small delay.
	/// Done so that it doesn't collide with the bow.
	/// </summary>
	IEnumerator ShootCoroutine() {
		gameObject.layer = LayerMask.NameToLayer ("Void");
		attachedCollider.gameObject.layer = LayerMask.NameToLayer ("Void");
		yield return new WaitForSeconds (0.1f);
		yield return new WaitForFixedUpdate ();
		gameObject.layer = LayerMask.NameToLayer ("Projectiles");
		attachedCollider.gameObject.layer = LayerMask.NameToLayer ("Projectiles");
	}

	/// <summary>
	/// Should be called whenever the player becomes in range of this material.
	/// </summary>
	public void OnBecomeTargeted() {
		if (grabbable.grabbedBy == null) {
			ui.gameObject.SetActive (true);
			titleText.text = "Arrow";
		}
	}

	/// <summary>
	/// Should be called whenever the player leaves the range of this material
	/// </summary>
	public void OnBecomeUntargeted() {
		ui.gameObject.SetActive (false);
	}
}
