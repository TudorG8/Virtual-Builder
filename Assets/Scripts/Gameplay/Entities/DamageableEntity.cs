using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A damageable entity can be damaged by OffensiveEntities on either collision or trigger.
/// </summary>
public class DamageableEntity : MonoBehaviour {
	[Header("Settings")]
	[Tooltip("The damage type required to damage this entity.")]
	[SerializeField] ModifierName resistanceType;
	[Tooltip("How much health this entity has.")]
	[SerializeField] FloatStat health;
	[Tooltip("Colliders need to have one of these valid tags")]
	[SerializeField] List<string> validTags;

	public FloatStat Health {
		get {
			return this.health;
		}
		set {
			health = value;
		}
	}

	public List<string> ValidTags {
		get {
			return this.validTags;
		}
		set {
			validTags = value;
		}
	}

	public void OnHit(Collision collision) {
		if (!validTags.Contains(collision.collider.attachedRigidbody.tag)) {
			return;
		}

		OffensiveEntity offensiveEntity = collision.collider.attachedRigidbody.GetComponent<OffensiveEntity> ();

		if (offensiveEntity) {

			float damage = offensiveEntity.CalculateCurrentDamage (resistanceType);

			if (!Mathf.Approximately (damage, 0) && !Mathf.Approximately(health.Current, 0)) {
				health.Current -= damage;

				GameObject popup = Instantiate (PopupSpawner.Instance.DamagePopup, collision.GetContact (0).point, Quaternion.identity);
				PopupUI popupScript = popup.GetComponent<PopupUI> ();
				popupScript.TitleText.text = (Mathf.Ceil (damage * 10)).ToString ();

				GameObject mainEye = GameObject.FindGameObjectWithTag ("MainCamera");
				Vector3 direction = (mainEye.transform.position - popup.transform.position).normalized;
				popup.transform.position += direction * 0.2f;
			}
		}
	}

	public void OnHit(Collider collider) {
		if (!validTags.Contains(collider.tag)) {
			return;
		}

		OffensiveEntity offensiveEntity = collider.GetComponent<OffensiveEntity> ();

		if (offensiveEntity) {

			float damage = offensiveEntity.CalculateCurrentDamage (resistanceType);

			if (!Mathf.Approximately (damage, 0) && !Mathf.Approximately(health.Current, 0)) {
				health.Current -= damage;

				GameObject popup = Instantiate (PopupSpawner.Instance.DamagePopup, collider.transform.position, Quaternion.identity);
				PopupUI popupScript = popup.GetComponent<PopupUI> ();
				popupScript.TitleText.text = ((int)(damage * 10)).ToString ();

				GameObject mainEye = GameObject.FindGameObjectWithTag ("MainCamera");
				Vector3 direction = (mainEye.transform.position - popup.transform.position).normalized;
				popup.transform.position += direction * 0.2f;
			}
		}
	}
}
