using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A component that allows the reloading of guns.
/// All guns should have atleast one to allow reloading.
/// </summary>
public class GunReloader : MonoBehaviour {
	[Header("Read Only")]
	[Tooltip("Should be set by the main component when this is added.")]
	[SerializeField] GunItemBase connectedGunBase;

	public GunItemBase ConnectedGunBase {
		get {
			return this.connectedGunBase;
		}
		set {
			connectedGunBase = value;
		}
	}

	/// <summary>
	/// Simply calls reload on the main gun piece.
	/// </summary>
	/// <param name="other">Other.</param>
	public void Reload(Collider other) {
		if (connectedGunBase != null) {
			connectedGunBase.Reload (other);
		}
	}
}
