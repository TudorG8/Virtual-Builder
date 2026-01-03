using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class TriggerEvent : UnityEvent<Collider> {
}

[System.Serializable]
public class CollisionEvent : UnityEvent<Collision> {
}

/// <summary>
/// Allows an object to trigger different events on collisions with different layers.
/// </summary>
public class CollisionTrigger : MonoBehaviour{
	public enum TriggerType {
		OnTriggerEnter, 
		OnCollisionEnter, 
		OnTriggerExit,
		OnCollisionExit
	}

	[Header("Events")]
	[Tooltip("For trigger enter and trigger exit")]
	[SerializeField] TriggerEvent triggerEvents;
	[Tooltip("For collision enter and collision exit")]
	[SerializeField] CollisionEvent collisionEvents;

	[Header("Settings")]
	[Tooltip("Only collisions of this type will happen")]
	[SerializeField] TriggerType triggerType;
	[Tooltip("Select which layer this even trigger will happen on")]
    [SerializeField] LayerMask allowedCollisions;

    void OnTriggerEnter(Collider other) {
		if (triggerType == TriggerType.OnTriggerEnter && IsCollisionAllowed (other.gameObject)) {
			triggerEvents.Invoke (other);
		}
    }

	void OnTriggerExit(Collider other) {
		if(triggerType == TriggerType.OnTriggerExit && IsCollisionAllowed(other.gameObject))
			triggerEvents.Invoke(other);
	}

	void OnCollisionEnter(Collision other) {
		if (triggerType == TriggerType.OnCollisionEnter && IsCollisionAllowed (other.gameObject)) {
			collisionEvents.Invoke (other);
		}
    }

	void OnCollisionExit(Collision other) {
		if (triggerType == TriggerType.OnCollisionExit && IsCollisionAllowed(other.gameObject))
			collisionEvents.Invoke(other);
	}
		
    private bool IsCollisionAllowed(GameObject other) {
        return (allowedCollisions.value & 1 << other.gameObject.layer) == 1 << other.gameObject.layer;
    }
}
