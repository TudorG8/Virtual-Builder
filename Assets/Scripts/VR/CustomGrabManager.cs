using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A trigger area that sets the in range attributes of grabbables.
/// </summary>
public class CustomGrabManager : MonoBehaviour {
	[SerializeField] Color outlineColorInRange;
	[SerializeField] Color outlineColorHighlighted;

	public Color OutlineColorInRange {
		get {
			return this.outlineColorInRange;
		}
		set {
			outlineColorInRange = value;
		}
	}

	public Color OutlineColorHighlighted {
		get {
			return this.outlineColorHighlighted;
		}
		set {
			outlineColorHighlighted = value;
		}
	}

	void OnTriggerEnter(Collider otherCollider) {
		CustomGrabbable dg    = otherCollider.GetComponentInChildren<CustomGrabbable>();
		WhenInRange     range = otherCollider.GetComponentInChildren<WhenInRange>();
		if(dg) {
			dg.InRange = true;
		}
		if(range) {
			range.InRange = true;
		}
	}

	void OnTriggerExit(Collider otherCollider) {
		CustomGrabbable dg    = otherCollider.GetComponentInChildren<CustomGrabbable>();
		WhenInRange     range = otherCollider.GetComponentInChildren<WhenInRange>();
		if(dg) {
			dg.InRange = false;
		}
		if(range) {
			range.InRange = false;
		}
	}
}
