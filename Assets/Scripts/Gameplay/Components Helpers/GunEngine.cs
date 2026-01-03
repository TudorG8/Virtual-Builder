using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides some important details for guns. 
/// All guns should have one and only one engine.
/// </summary>
public class GunEngine : MonoBehaviour {
	[Header("Object References")]
	[Tooltip("The source that will play sound effects.")]
	[SerializeField] AudioSource audioSource;

	[Header("Prefabs")]
	[Tooltip("The bullet the gun will shoot.")]
	[SerializeField] GameObject bulletPrefab;
	[Tooltip("The clip that should be played when fired.")]
	[SerializeField] VolumeClip shootingSound;
	[Tooltip("The clip that should be played when the magazine is empty.")]
	[SerializeField] VolumeClip emptySound;
	[Tooltip("The clip that should be played when the gun is reloaded.")]
	[SerializeField] VolumeClip reloadSound;

	[Header("Settings")]
	[Tooltip("Whether the resulting gun will be automatic or not (when holding the fire button)")]
	[SerializeField] bool isAutomatic;

	public GameObject BulletPrefab {
		get {
			return this.bulletPrefab;
		}
		set {
			bulletPrefab = value;
		}
	}

	public bool IsAutomatic {
		get {
			return this.isAutomatic;
		}
		set {
			isAutomatic = value;
		}
	}

	public AudioSource AudioSource {
		get {
			return this.audioSource;
		}
		set {
			audioSource = value;
		}
	}

	public VolumeClip ShootingSound {
		get {
			return this.shootingSound;
		}
		set {
			shootingSound = value;
		}
	}

	public VolumeClip EmptySound {
		get {
			return this.emptySound;
		}
		set {
			emptySound = value;
		}
	} 

	public VolumeClip ReloadSound {
		get {
			return this.reloadSound;
		}
		set {
			reloadSound = value;
		}
	}
}
