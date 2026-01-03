using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A quiver holds arrows and allows a player to grab them out of it by holding their controller trigger.
/// The motion should resemble grabbing an arrow like out of a real bow.
/// </summary>
public class Quiver : MonoBehaviour {
	[Header("Settings")]
	[Tooltip("The layer the hand is on")]
	[SerializeField] LayerMask handLayer;

	[Header("Prefabs")]
	[Tooltip("The arrow prefab to spawn when one is drawn")]
	[SerializeField] GameObject arrowPrefab;

	void OnTriggerStay(Collider other) {
		if(other.attachedRigidbody == null) {
			return;
		}

		CustomGrabber grabber = other.attachedRigidbody.GetComponent<CustomGrabber> ();

		if (grabber && IsCollisionAllowed(other.gameObject)) {
			if (grabber.GrabbedObject != null) {
				return;
			} 

			float strength = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, grabber.Controller);

			if (strength > 0.5f) {
				GameObject arrowObj = Instantiate (arrowPrefab, grabber.transform.position, Quaternion.identity);
				arrowObj.transform.forward = grabber.transform.forward;

				grabber.M_target = arrowObj.GetComponent<CustomGrabbable> ();
				grabber.M_targetCollider = arrowObj.GetComponent<CustomGrabbable> ().grabPoints [0];

				grabber.ForceGrabBegin ();
			}
		}
	}

	private bool IsCollisionAllowed(GameObject other) {
		return (handLayer.value & 1 << other.gameObject.layer) == 1 << other.gameObject.layer;
	}
}
