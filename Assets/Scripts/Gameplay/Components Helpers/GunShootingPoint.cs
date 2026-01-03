using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An item component that acts as a point from where projectiles can be shot forward.
/// </summary>
public class GunShootingPoint : MonoBehaviour {
	[Header("Object References")]
	[Tooltip("Projectiles will be shot from this point")]
	[SerializeField] Transform shootingPoint;

	public Transform ShootingPoint {
		get {
			return this.shootingPoint;
		}
		set {
			shootingPoint = value;
		}
	}
}
