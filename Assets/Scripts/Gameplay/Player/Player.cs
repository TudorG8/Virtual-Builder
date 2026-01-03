using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Represents the stats of the player.
/// </summary>
public class Player : MonoBehaviour {
	[Header("Scene References")]
	[Tooltip("Where the player will respawn when dead.")]
	[SerializeField] Transform startingPoint;
	[Tooltip("Where the player will be moved to on death.")]
	[SerializeField] Transform deadPoint;
	[Tooltip("The image that shows how wounded the player is.")]
	[SerializeField] Image damageImage;

	[Header("Settings")]
	[Tooltip("How quick the player regenerates when out of combat.")]
	[SerializeField] float regenerationPerSecond = 4f;
	[Tooltip("How much health the player has.")]
	[SerializeField] FloatStat health;
	[Tooltip("How long the player will last in combat after being hit.")]
	[SerializeField] float timeInCombat = 20f;
	[Tooltip("How much hp the player has on start or respawn.")]
	[SerializeField] float startingHitpoints = 50f;

	[Header("Read Only")]
	[Tooltip("Will be true when the player runs out of hitpoints.")]
	[SerializeField] bool dead = false;
	[Tooltip("Will be true when the player gets hit by an enemy. Lasts until timeInCombat.")]
	[SerializeField] bool inCombat = false;
	[Tooltip("Amount of time left until the player is out of combat.")]
	[SerializeField] float timeLeftInCombat;

	public FloatStat Health {
		get {
			return this.health;
		}
		set {
			health = value;
		}
	}

	public bool Dead {
		get {
			return this.dead;
		}
		set {
			dead = value;
		}
	}

	void Start() {
		health.Current = startingHitpoints;
	}

	void Update() {
		if (dead) {
			return;
		}

		if (inCombat) {
			timeLeftInCombat -= Time.deltaTime;

			if (timeLeftInCombat < 0) {
				timeLeftInCombat = 0;
				inCombat = false;
			}
		} 
		else {
			health.Current += regenerationPerSecond * Time.deltaTime;
		}

		Color currentColor = damageImage.color;
		currentColor.a = (1 - health.Current / startingHitpoints) * 0.50f;
		damageImage.color = currentColor;
	}

	public void OnDamageTaken(float current, float maximum, float difference) {
		if (difference <= 0) {
			return;
		}

		if (dead) {
			return;
		}

		if (Mathf.Approximately (current, 0)) {
			dead = true;
			transform.position = deadPoint.position;
			transform.forward = deadPoint.forward;

			Color currentColor = damageImage.color;
			currentColor.a = 0;
			damageImage.color = currentColor;
			return;
		}

		inCombat = true;
		timeLeftInCombat = timeInCombat;
	}

	/// <summary>
	/// Resets the players stats and places them at the starting point.
	/// </summary>
	public void Reset() {
		health.Current = startingHitpoints;
		dead = false;
		inCombat = false;
		timeLeftInCombat = 0f;
		transform.position = startingPoint.position;
		transform.forward = startingPoint.forward;
	}
}
