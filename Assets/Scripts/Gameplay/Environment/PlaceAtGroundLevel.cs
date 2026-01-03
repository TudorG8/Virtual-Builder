#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Editor script for level design that will place attached objects on top of a given layer.
/// </summary>
[ExecuteInEditMode]
public class PlaceAtGroundLevel : MonoBehaviour {
	[Header("Settings")]
	[Tooltip("Allows manual stopping.")]
	[SerializeField] bool update;
	[Tooltip("Will place the object on top of this layer.")]
	[SerializeField] LayerMask layerMask;

	void Update() {
		if (!update || Application.isPlaying) {
			return;
		}

		RaycastHit hit;

		Ray ray = new Ray (transform.position + Vector3.up * 10000, Vector3.down);
		if (Physics.Raycast (ray, out hit, Mathf.Infinity, layerMask)) {
			transform.position = hit.point;
		}
	}
}
#endif