using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Allows the grabbing of items and storing them in the inventory.
/// </summary>
public class InventoryStorer : MonoBehaviour {
	[Header("Scene References")]
	[SerializeField] Inventory inventory;

	[Header("Object References")]
	[SerializeField] CustomGrabber grabber;

	void Update() {
		HandleStoring ();
	}

	/// <summary>
	/// Checks for the user pressing the store button and processes items.
	/// </summary>
	void HandleStoring() {
		if (grabber.M_target == null) {
			return;
		}

		bool storeItem = OVRInput.GetDown (OVRInput.Button.One, grabber.Controller);

		if (storeItem) {
			if (inventory.ProcessItem (grabber.M_target.gameObject)) {

				GameObject obj = grabber.M_target.gameObject;

				if (grabber.grabbedObject != null) {
					grabber.ForceRelease (grabber.grabbedObject);
				}

				Destroy (obj);
			}
		}
	}
}
