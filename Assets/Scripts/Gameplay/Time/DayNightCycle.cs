using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides a day night cycle based on having two directional lights.
/// </summary>
public class DayNightCycle : Singleton<DayNightCycle> {
	[Header("Scene References")]
	[Tooltip("Reference to the sun directional light.")]
	[SerializeField] Light sun;
	[Tooltip("Reference to the moon directional light.")]
	[SerializeField] Light moon;

	[Header("Settings")]
	[Tooltip("Seconds in a full cycle (day and night)")]
	[SerializeField] float secondsInCycle = 120;
	[Tooltip("Initial rotation of the sun.")]
	[SerializeField] Vector3 initialRotation;
	[Tooltip("Base intensity value.")]
	[SerializeField] float baseValue = 3.5f;

	[Header("Read Only")]
	[Tooltip("How much the cycle progressed.")]
	[Range(0, 1)] [SerializeField] float currentTime = 0;

	public float CurrentTime {
		get {
			return this.currentTime;
		}
		set {
			currentTime = value;
		}
	}

	void Awake() {
		InitiateSingleton ();
	}

	void Update() {
		UpdateSun();

		currentTime = (currentTime + Time.deltaTime / secondsInCycle) % 1;
	}

	/// <summary>
	/// Updates the sun.
	/// The cycle can be split into 4 quadrants.
	/// Quadrant 1 : (0.00 -> 0.25)
	/// Sun intensity goes from 0% to 100%
	/// Quadrant 2 : (0.00 -> 0.25)
	/// Sun intensity goes from 100% to 0%
	/// Quadrant 3 : (0.00 -> 0.25)
	/// Moon intensity goes from 0% to 100%
	/// Quadrant 4 : (0.00 -> 0.25)
	/// Moon intensity goes from 100% to 0%
	/// </summary>
	void UpdateSun() {
		sun.transform.localRotation = Quaternion.Euler((currentTime * 360f) + initialRotation.x, initialRotation.y, initialRotation.z);

		if (currentTime <= 0.25f) {
			sun.intensity = baseValue * (currentTime / 0.25f);
			moon.intensity = 0f;
		} 
		else if (currentTime <= 0.50f) {
			sun.intensity = baseValue * ((0.50f - currentTime) / 0.25f);
			moon.intensity = 0f;
		}
		else if(currentTime <= 0.75f) {
			sun.intensity = 0f;
			moon.intensity = baseValue * ((currentTime - 0.50f) / 0.25f);
		}
		else {
			sun.intensity = 0f;
			moon.intensity = baseValue * ((1.00f - currentTime) / 0.25f);
		}
	}
}