using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Make an element float on top of an element at a given offset
/// </summary>
public class FloatingText : MonoBehaviour {
	[Header("Object References")]
	[Tooltip("The object that will object will float around")]
	[SerializeField] Transform obj; 
	[Tooltip("An object whose local position will be saved to be used as an offset at awake time")]
	[SerializeField] Transform offset;

	[Header("Read Only")]
	[Tooltip("This will be populated at runtime to be equal to the main camera")]
	[SerializeField] Transform target;

	[Header("Settings")]
	[SerializeField] bool sameYCoordinate = false;
	[Tooltip("If this is true, this object will turn to face the target")]
	[SerializeField] bool lookAtTarget = true;
	[Tooltip("If this is true, this object will update its position in regards to obj")]
	[SerializeField] bool updatePosition = true;
	[Tooltip("If this is true, it will run allign on the Update step.")]
	[SerializeField] bool runOnUpdate = true;

	[Header("Read Only")]
	[Tooltip("This will be equal to the offset local position")]
	[SerializeField] Vector3 savedOffset;

	void Awake() {
		GameObject mainEye = GameObject.FindGameObjectWithTag ("MainCamera");
		if(mainEye != null)
			target = mainEye.transform;
		savedOffset = offset.localPosition;
	}

	/// <summary>
	/// Gets the position relative to the object.
	/// </summary>
	Vector3 GetPosition() {
		return  obj.position + savedOffset;
	}

	void Update () {
		if (target != null && runOnUpdate) {
			Allign ();
		}
	}

	/// <summary>
	/// Alligns the attached object to match the camera.
	/// </summary>
	public void Allign() {
		if (lookAtTarget) {
			Vector3 targetActualPosition = target.transform.position;
			Vector3 currentPosition = transform.position;

			if (sameYCoordinate) {
				targetActualPosition.y = currentPosition.y;
			}

			Vector3 dir = currentPosition - targetActualPosition;
			transform.rotation = Quaternion.LookRotation (dir);
		}

		if (updatePosition) {
			transform.position = GetPosition ();
		}
	}
}
