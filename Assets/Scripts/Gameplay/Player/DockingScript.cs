using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Allows items to be docked on a waist side of the player.
/// </summary>
public class DockingScript : MonoBehaviour {
	[Header("Object References")]
	[Tooltip("The exact position that the items will be positioned on")]
	[SerializeField] Transform dockingPosition;

	[Header("Read Only")]
	[Tooltip("The currently docked item on this side.")]
	[SerializeField] CustomGrabbable currentlyDockedItem;

	[Header("Settings")]
	[Tooltip("The layer the hand is on")]
	[SerializeField] LayerMask handLayer;


	/// <summary>
	/// Check if a hand is currently sitting in the grabbing range
	/// When it detects a grabber, it will make it so that when it drops an item, it will be automatically docked.
	/// </summary>
	void OnTriggerEnter(Collider other) {
		if(other.attachedRigidbody == null || other.tag != "Hand") {
			return;
		}

		CustomGrabber grabber = other.attachedRigidbody.GetComponent<CustomGrabber> ();

		if (grabber && IsCollisionAllowed(other.gameObject)) {
			grabber.OnGrabEnd.RemoveListener (DockItem);
			grabber.OnGrabEnd.AddListener (DockItem);
		}
	}

	/// <summary>
	/// Check if a hand just left the grabbing range.
	/// Removes the docking event from the grabber
	/// </summary>
	void OnTriggerExit(Collider other) {
		
		if(other.attachedRigidbody == null || other.tag != "Hand") {
			return;
		}

		CustomGrabber grabber = other.attachedRigidbody.GetComponent<CustomGrabber> ();

		if (grabber && IsCollisionAllowed(other.gameObject)) {
			grabber.OnGrabEnd.RemoveListener (DockItem);
		}
	}

	/// <summary>
	/// Action to take to dock an item, which just sets it as kinematic and makes it follow the player.
	/// Only item bases can be docked.
	/// </summary>
	void DockItem(OVRGrabbable grabbable) {
		if (grabbable == null || currentlyDockedItem != null) {
			return; 
		}

		CustomGrabbable grabbableScript = grabbable as CustomGrabbable;

		ItemBase itemBase = grabbableScript.GetComponent<ItemBase> ();

		if (itemBase != null && itemBase.ItemComponent.AttachedRigidbody != null) {
			currentlyDockedItem = grabbableScript;
			itemBase.ItemComponent.AttachedRigidbody.isKinematic = true;
		}
	}

	private bool IsCollisionAllowed(GameObject other) {
		return (handLayer.value & 1 << other.gameObject.layer) == 1 << other.gameObject.layer;
	}

	void LateUpdate () {
		if (currentlyDockedItem == null) {
			return;
		}
		// If a docked item has just been grabbed
		else if (currentlyDockedItem.grabbedBy != null) { 
			currentlyDockedItem = null;
		}
		else {
			//If an item is docked, its parent needs to be offset
			if (currentlyDockedItem.Item != null) {
				currentlyDockedItem.transform.parent.position = dockingPosition.position + (currentlyDockedItem.transform.parent.position - currentlyDockedItem.transform.position);
				currentlyDockedItem.transform.parent.rotation = dockingPosition.rotation;
			} 
			else {
				currentlyDockedItem.transform.position = dockingPosition.position;
				currentlyDockedItem.transform.rotation = dockingPosition.rotation;
			}
		}
	}
}
