using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OVRTouchSample;
using UnityEngine.Events;

/// <summary>
/// This is an override of DistanceGrabbable, with a few difference to better fit this game.
/// Rather than inherit/override, this changes some things that are not marked as virtual in the original.
/// In exchange, some of this code is the same as the original.
/// </summary>
public class CustomGrabbable : OVRGrabbable {
	[Header("Settings")]
	[Tooltip("Material block that can change colours.")]
	[SerializeField] string m_materialColorField = "_OutlineColor";

	[Header("Scene Objects")]

	[Tooltip("Reference to all renderers on this object")]
	[SerializeField] List<Renderer> renderers;

	[Header("Prefabs")]
	[Tooltip("Reference to the offhand snap offset")]
	[SerializeField] Transform m_snapOffset_offHand;

	[Header("Events")]
	[Tooltip("Triggered when this grabbable enters the range of the player")]
	[SerializeField] UnityEvent onBecomeInRange;
	[Tooltip("Triggered when this grabbable exits the range of the player")]
	[SerializeField] UnityEvent onBecomeOutOfRange;
	[Tooltip("Triggered when this object becomes grabbed")]
	[SerializeField] UnityEvent onBecomeGrabbed;
	[Tooltip("Triggered when this object becomes ungrabbed")]
	[SerializeField] UnityEvent onBecomeUngrabbed;
	[Tooltip("Triggered when this object becomes targeted by a grabber")]
	[SerializeField] UnityEvent onBecomeTargeted;
	[Tooltip("Triggered when this object becomes untargeted by a grabber")]
	[SerializeField] UnityEvent onBecomeUntargeted;

	[Header("Read Only")]
	[Tooltip("Reference to the grab manager, set at runtime")]
	[SerializeField] CustomGrabManager m_crosshairManager;
	[Tooltip("True when the player is close enough to this object")]
	[SerializeField] bool m_inRange;
	[Tooltip("True when the player has a CustomGrabber pointing at this object")]
	[SerializeField] bool m_targeted;
	[Tooltip("A value that can be changed at runtime to switch this behaviour on and off")]
	[SerializeField] bool canBeGrabbed = true;
	[Tooltip("If this grabbable is part of an item, this will be populated")]
	[SerializeField] Item item;
	[Tooltip("Will be created at runtime")]
	[SerializeField] MaterialPropertyBlock m_mpb;

	public bool CanBeGrabbed {
		get { return canBeGrabbed; }
		set {
			canBeGrabbed = value;
			Refresh ();
		}
	}

	public bool InRange {
		get { return m_inRange; }
		set {
			if (m_inRange != value) {
				if (value) {onBecomeInRange.Invoke ();} 
				else { onBecomeOutOfRange.Invoke ();}
			}
			m_inRange = value;
			Refresh();
		}
	}

	public bool Targeted {
		get { return m_targeted; }
		set {
			if (m_targeted != value) {
				if (value) {
					onBecomeTargeted.Invoke ();
				} 
				else {
					onBecomeUntargeted.Invoke ();
				}
			}

			m_targeted = value;
			Refresh ();
		}
	}

	public Item Item {
		get { return item; }
		set { item = value; }
	}

	public Transform snapOffsetOffhand {
		get { return m_snapOffset_offHand; }
	}

	public Collider GrabbedCollider {
		get { return m_grabbedCollider; }
	}

	void Awake() {
		m_crosshairManager = FindObjectOfType<CustomGrabManager>();
		m_mpb = new MaterialPropertyBlock();
	}

	protected override void Start() {
		m_crosshairManager = FindObjectOfType<CustomGrabManager>();
		m_mpb = new MaterialPropertyBlock();
		Refresh ();
	}

	/// <summary>
	/// Returns the snap offset in relation to the grip transform.
	/// </summary>
	/// <returns>A rotated vector.</returns>
	/// <param name="offset">The local position of the grip</param>
	/// <param name="offhand">Whether the controller wanting to grab this is the offhand</param>
	public Vector3 GetSnapOffsetPosition(Vector3 offset, bool offhand) {
		if (item == null) {
			Transform offsetToUse = offhand ? snapOffsetOffhand : m_snapOffset;
			return offsetToUse.position + offset; 
		} 
		else {
			Vector3 diff = transform.localPosition * -1;
			Transform offsetToUse = offhand ? snapOffsetOffhand : m_snapOffset;
			return RotatePointAroundPivot (
				offsetToUse.position + offset + diff, 
				offsetToUse.position + offset, 
				offsetToUse.rotation.eulerAngles
			);
		}
	}

	/// <summary>
	/// Rotates a point around a pivot by angles.
	/// </summary>
	/// <returns>The rotated vector.</returns>
	public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles) {
		return Quaternion.Euler(angles) * (point - pivot) + pivot;
	}

	/// <summary>
	/// Returns true if both offsets are not null
	/// </summary>
	public bool HasSnapOffset() {
		return m_snapOffset != null && m_snapOffset_offHand != null;
	}

	/// <summary>
	/// Should be called when a hand grabs this object.
	/// </summary>
	override public void GrabBegin(OVRGrabber hand, Collider grabPoint) {
		m_grabbedBy = hand;
		m_grabbedCollider = grabPoint;

		// In parent due to the existance of items, which will put the rigidbody on themselves
		gameObject.GetComponentInParent<Rigidbody>().isKinematic = true;

		if (item != null) {
			item.CurrentlyGrabbedComponent = this;
		}

		onBecomeGrabbed.Invoke ();
	}

	/// <summary>
	/// Should be called whenever a hand drops this object
	/// </summary>
	override public void GrabEnd(Vector3 linearVelocity, Vector3 angularVelocity) {
		m_grabbedBy = null; 
		m_grabbedCollider = null;

		// In parent due to the existance of items, which will put the rigidbody on themselves
		Rigidbody rb = gameObject.GetComponentInParent<Rigidbody>();
		rb.isKinematic = m_grabbedKinematic;
		rb.velocity = linearVelocity;
		rb.angularVelocity = angularVelocity;

		if (item != null) {
			item.CurrentlyGrabbedComponent = null;
		}

		onBecomeUngrabbed.Invoke ();
	}

	/// <summary>
	/// Similar to the normal grab end, but will not apply forces at the end of the grab, essentially resetting physics
	/// </summary>
	public void GrabEnd(Vector3 linearVelocity, Vector3 angularVelocity, bool applyForces) {
		if (applyForces) {
			Rigidbody rb = gameObject.GetComponentInParent<Rigidbody> ();
			rb.isKinematic = m_grabbedKinematic;
			rb.velocity = linearVelocity;
			rb.angularVelocity = angularVelocity;
		}
		m_grabbedBy = null;
		m_grabbedCollider = null;

		if (item != null) {
			item.CurrentlyGrabbedComponent = null;
			onBecomeUngrabbed.Invoke ();
		}
	}

	/// <summary>
	/// Sets the colour of the material block depending on whether the player is near or targetting this object.
	/// </summary>
	void Refresh() {
		if(m_materialColorField != null) {
			Color colorToUse;
			if (isGrabbed || !InRange || !canBeGrabbed) {
				colorToUse = Color.clear;
			} 
			else if (Targeted) {
				colorToUse = m_crosshairManager.OutlineColorHighlighted;
			} 
			else {
				colorToUse = m_crosshairManager.OutlineColorInRange;
			}

			for (int i = 0; i < renderers.Count; i++) {
				Renderer renderer = renderers [i];

				renderer.GetPropertyBlock (m_mpb);
				m_mpb.SetColor (m_materialColorField, colorToUse);
				renderer.SetPropertyBlock(m_mpb);
			}
		}
	}

	public void MakeUngrabbableFor(float duration) { 
		StartCoroutine (UngrabbableRoutine (duration));
	}

	IEnumerator UngrabbableRoutine(float duration) {
		canBeGrabbed = false;
		yield return new WaitForSeconds (duration);
		canBeGrabbed = true;
	}
}
