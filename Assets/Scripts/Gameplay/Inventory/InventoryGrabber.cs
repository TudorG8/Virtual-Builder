using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// For now, simply sets the laser pointer to OnWhenHitTarget, as this is private in the library.
/// </summary>
public class InventoryGrabber : MonoBehaviour {
	[Header("Object References")]
	[Tooltip("Reference to the laser pointer.")]
	[SerializeField] LaserPointer pointer;

	void Start() {
		pointer.laserBeamBehavior = LaserPointer.LaserBeamBehavior.OnWhenHitTarget;
	}
}
