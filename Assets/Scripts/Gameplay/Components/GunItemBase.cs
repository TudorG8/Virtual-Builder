using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Guns allow the shooting of bullets when combined with gun shooting points.
/// </summary>
public class GunItemBase : ItemBase {
	[Header("Settings")]
	[Tooltip("The layer the hand is on.")]
	[SerializeField] LayerMask handLayer;

	[Header("Read Only")]
	[Tooltip("A list of all gun shooting points, populated when new components are added")]
	[SerializeField] List<GunShootingPoint> endPoints;
	[Tooltip("Will be true until the player releases the trigger")]
	[SerializeField] bool tooTight = false;
	[Tooltip("Whether firing should be continuous when held down. Should be set by the gun engine.")]
	[SerializeField] bool automatic;
	[Tooltip("Set to true if not on cooldown.")]
	[SerializeField] bool canShoot = true;
	[Tooltip("Current amount of projectiles left.")]
	[SerializeField] int currentMagazine;
	[Tooltip("A reference to the gun engine, which is attached at runtime")]
	[SerializeField] GunEngine gunEngine;

	void Start() {
		UpdateStats ();
	}

	void Update() {
		if (itemComponent.CustomGrabbable.grabbedBy == null) {
			return;
		}

		float strength = OVRInput.Get (OVRInput.Axis1D.PrimaryIndexTrigger, (itemComponent.CustomGrabbable.grabbedBy as CustomGrabber).Controller);

		if (!automatic) {
			if (strength > 0.6f && !tooTight) {
				ShootBullets ();
				tooTight = true;
			} else if (strength <= 0.3f && tooTight) {
				tooTight = false;
			}
		} 
		else {
			if (strength > 0.6f && canShoot) {
				ShootBullets ();
				StartCoroutine (ShootRoutine (1 / stats.Find(stat => stat.Name == ModifierName.FireRate).Current));
			}
		}
	}

	/// <summary>
	/// Reload the gun and sets the current magazine to the maximum size. 
	/// Destroys the ammo on use.
	/// </summary>
	/// <param name="other">Ammo used.</param>
	public void Reload(Collider other) {
		if (other.tag != "Ammo") {
			return;
		}

		int maxMagazine = (int)stats.Find(stat => stat.Name == ModifierName.MagazineSize).Current;

		if (currentMagazine == maxMagazine) {
			return;
		}

		gunEngine.ReloadSound.PlayFrom (gunEngine.AudioSource);

		currentMagazine = maxMagazine;
		CustomGrabbable grabbable = other.attachedRigidbody.GetComponent<CustomGrabbable> ();
		if (grabbable && grabbable.grabbedBy != null) {
			(grabbable.grabbedBy as CustomGrabber).ForceRelease (grabbable);
		}

		Destroy (other.gameObject);
	}

	/// <summary>
	/// Clamps the accuracy between 0 and 100, 
	/// after which it will return a value between 0(perfectly accurate) and 0.5(max miss chance)
	/// </summary>
	float GetAccuracyRange(float accuracy) {
		accuracy = Mathf.Clamp (accuracy, 0, 100);

		return ((100 - accuracy) / 100f) * 0.5f;
	}

	/// <summary>
	/// Disables shooting for the given parameter.
	/// </summary>
	IEnumerator ShootRoutine(float seconds) {
		canShoot = false;
		yield return new WaitForSeconds (seconds);
		canShoot = true;
	}

	/// <summary>
	/// Whenever a new component is added, check if it is a gun shooting point
	/// </summary>
	public override void OnComponentConnection(ConnectorInformation connectorInformation) {
		base.OnComponentConnection (connectorInformation);

		ItemComponent newComponent = connectorInformation.Output.Component;

		GunShootingPoint gunShootingPoint = newComponent.GetComponent<GunShootingPoint> ();
		if (gunShootingPoint) {
			endPoints.Add (gunShootingPoint);
			return;
		} 

		GunEngine gunEngine = newComponent.GetComponent<GunEngine> ();
		if (gunEngine) {
			this.gunEngine = gunEngine;
			automatic = gunEngine.IsAutomatic;
			return;
		}

		GunReloader gunReloader = newComponent.GetComponent<GunReloader> ();
		if (gunReloader) {
			gunReloader.ConnectedGunBase = this;
		}
	}

	/// <summary>
	/// Create bullets at each shooting point and make them move forward
	/// </summary>
	void ShootBullets() {
		if (gunEngine == null) {
			return;
		}

		if (currentMagazine == 0) {
			if (endPoints.Count > 0) {
				gunEngine.EmptySound.PlayFrom (gunEngine.AudioSource);
			}
			return;
		}

		for (int i = 0; i < endPoints.Count; i++) {
			GameObject bulletObj = Instantiate (gunEngine.BulletPrefab, endPoints [i].ShootingPoint.position, Quaternion.identity);
			bulletObj.transform.forward = endPoints [i].transform.forward;

			OffensiveEntity offensiveEntity = bulletObj.GetComponent<OffensiveEntity> ();
			offensiveEntity.GetStat(ModifierName.Damage).CurrentMaximum = stats.Find(stat => stat.Name == ModifierName.Damage).Current;

			Vector2 pointInCircle = Random.insideUnitCircle * GetAccuracyRange(stats.Find(stat => stat.Name == ModifierName.Accuracy).Current);
			Vector3 destination = bulletObj.transform.forward + bulletObj.transform.right * pointInCircle.x + bulletObj.transform.up * pointInCircle.y;

			bulletObj.transform.forward = destination.normalized;


			bulletObj.transform.position = bulletObj.transform.position - bulletObj.transform.forward * 0.1f;

			Rigidbody rigidBody = bulletObj.GetComponent<Rigidbody> ();

			rigidBody.AddForce  (-bulletObj.transform.forward * stats.Find(stat => stat.Name == ModifierName.Velocity).Current);
		}

		if (endPoints.Count > 0) {
			gunEngine.ShootingSound.PlayFrom (gunEngine.AudioSource);
		}

		currentMagazine = Mathf.Clamp (currentMagazine - endPoints.Count, 0, int.MaxValue);
	}
		
	private bool IsCollisionAllowed(GameObject other) {
		return (handLayer.value & 1 << other.gameObject.layer) == 1 << other.gameObject.layer;
	}
}
