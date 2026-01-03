using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A Offensive entity that can deal damage to DamageableEntities.
/// </summary>
public class OffensiveEntity : MonoBehaviour {
	[Header("Settings")]
	[Tooltip("Whether to use velocity when calculating damage.")]
	[SerializeField] bool useVelocity;
	[Tooltip("Minimum velocity required for minimum damage.")]
	[SerializeField] float minVelocity;
	[Tooltip("Maximum velocity required for maximum damage.")]
	[SerializeField] float maxVelocity;
	[Tooltip("A list of current stats. Should contain atleast damage and the what damage resistances this can hit.")]
	[SerializeField] List<FloatStat> stats;
	[Tooltip("If true, this can only deal damage once (example bullets)")]
	[SerializeField] bool dealDamageOnceOnly = false;

	[Header("Read Only")]
	[Tooltip("Current velocity of the object")]
	[SerializeField] float velocity;
	[Tooltip("Position on the last frame, used to calculate velocity.")]
	[SerializeField] Vector3 lastPosition;
	[Tooltip("Will be true if <dealDamageOnceOnly> is on and this has already dealt damage.")]
	[SerializeField] bool hasDealtDamage = false;

	public bool HasDealtDamage {
		get {
			return this.hasDealtDamage;
		}
		set {
			hasDealtDamage = value;
		}
	}

	public FloatStat GetStat(ModifierName name) {
		return stats.Find(stat => stat.Name == name);
	}

	void Start() {
		lastPosition = transform.position;
	}
    
	void Update() {
		velocity = ((transform.position - lastPosition) / Time.deltaTime).magnitude;
		lastPosition = transform.position;
    }

	/// <summary>
	/// Calculates the damage against a given resistance, taking into account velocity if enabled.
	/// </summary>
	public float CalculateCurrentDamage(ModifierName targetResistance) {
		if (dealDamageOnceOnly && hasDealtDamage) {
			return 0;
		}
			
		FloatStat damage = stats.Find(stat => stat.Name == ModifierName.Damage);
		FloatStat damageModifier = stats.Find(stat => stat.Name == targetResistance);

		if (dealDamageOnceOnly) {
			hasDealtDamage = true;
		}

		float damageToDeal = damageModifier != null ? damage.Current * (damageModifier.Current / 100f) : 0;

		if (!useVelocity) {
			return damageToDeal;
		}

		if (velocity < minVelocity) {
			return 0;
		}

		float currentVelocity = Mathf.Clamp (velocity, minVelocity, maxVelocity);

		float percentageOfSpeed = Mathf.Clamp01((currentVelocity - minVelocity) / (maxVelocity - minVelocity));

		float damageDealt = percentageOfSpeed * damageToDeal;

		return damageDealt;
	}
}
