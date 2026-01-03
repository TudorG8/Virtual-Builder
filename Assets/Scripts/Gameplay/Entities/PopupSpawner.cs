using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spawner for different game objects.
/// </summary>
public class PopupSpawner : Singleton<PopupSpawner> {
	[Header("Prefabs")]
	[Tooltip("Damage prefab used for popups")]
	[SerializeField] GameObject damagePopup;
	[Tooltip("Item base used for component connection.")]
	[SerializeField] GameObject itemBase;

	public GameObject DamagePopup {
		get {
			return this.damagePopup;
		}
		set {
			damagePopup = value;
		}
	}

	public GameObject ItemBase {
		get {
			return this.itemBase;
		}
		set {
			itemBase = value;
		}
	}

	void Awake() {
		InitiateSingleton ();
	}
}
