using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script that will make the waist script point in the general direction of the user.
/// </summary>
public class WaistScript : MonoBehaviour {
	[Header("Prefabs")]
	[Tooltip("Will replace the material of the chest")]
	[SerializeField] Material chestMaterial;
	[Tooltip("Will replace the material of the headset")]
	[SerializeField] Material headsetMaterial;

	[Header("Scene Objects")]
	[Tooltip("The player object, which should have a forward facing the way the player is looking")]
	[SerializeField] Transform player;
	[Tooltip("The main camera which shows the direction of the headset")]
	[SerializeField] Transform mainCamera;
	[Tooltip("The body transform that contains the skinned mesh renderers")]
	[SerializeField] Transform bodyParent;

	[Header("Read Only")]
	[Tooltip("Will be equal to the chest mesh at runtime")]
	[SerializeField] SkinnedMeshRendererMiddle chest;
	[Tooltip("Will be equal to the headset mesh at runtime")]
	[SerializeField] SkinnedMeshRendererMiddle headset;

	[Header("Settings")]
	[Tooltip("The waist will be offset from the chest by this amount")]
	[SerializeField] Vector3 offset;
	[Tooltip("Fast speed for smoothing the waist.")]
	[SerializeField] float fastSpeed = 3f;
	[Tooltip("Slow speed for smoothing the waist.")]
	[SerializeField] float slowSpeed = 1f;
	[Tooltip("Minimum angles facing up before the second check starts.")]
	[SerializeField] float angles = 67;
	[Tooltip("Maximum angles facing up before the second check ends.")]
	[SerializeField] float angleOffset = 90;
	[Tooltip("Whether to enable the draw gizmos setting.")]
	[SerializeField] bool drawGizmos = true;

	void Update() {
		if (chest == null && bodyParent.childCount > 0) {
			Transform chestObj = bodyParent.GetChild (1);
			Transform headObj  = bodyParent.GetChild (2);

			chest = chestObj.gameObject.AddComponent<SkinnedMeshRendererMiddle> ();
			headset = headObj.gameObject.AddComponent<SkinnedMeshRendererMiddle> ();

			chest.Material = chestMaterial;
			headset.Material = headsetMaterial;

			// Wait for next frame for position to have been calculated
			return;
		}

		if (headset == null && chest == null) {
			return;
		}

		Vector3 chestDirection = chest.Position - player.position;
		Vector3 headDirection = headset.Position - player.position;

		bool useSmoothing = false;
		Vector3 direction;
		// Head is pointing backwards, but the chest isnt
		if (Vector3.Angle(player.up, mainCamera.forward) < angles) {
			float upAngles = Vector3.Angle (player.up, mainCamera.up);

			if (upAngles > angles && upAngles < angleOffset) {
				direction = chestDirection;
			} 
			else if (upAngles > angles) {
				direction = -headDirection;
			}
			else {
				direction = headDirection;
			}

			useSmoothing = true;
		}
		else {
			direction = headset.Position - chest.Position;
		}

		Vector3 right = Vector3.Cross (transform.up, direction);
		Vector3 newForward = Vector3.Cross (right, transform.up);

		transform.position = chest.Position + offset;
			
		if (useSmoothing) {
			transform.forward = Vector3.Slerp (transform.forward, newForward, Time.deltaTime * (useSmoothing ? slowSpeed : fastSpeed));
		} 
		else {
			transform.forward = newForward;
		}
	}

	void OnDrawGizmos() {
		if (headset == null && chest == null) {
			return;
		}
		if (!drawGizmos) {
			return;
		}

		Vector3 chestDirection = chest.Position - player.position;
		Vector3 headDirection = headset.Position - player.position;

		Gizmos.color = Color.red;
		Gizmos.DrawLine (player.position, player.position + chestDirection.normalized * 5);

		Gizmos.color = Color.green;
		Gizmos.DrawLine (player.position, player.position + headDirection.normalized * 5);

		Gizmos.color = Color.grey;
		Gizmos.DrawLine (player.position, player.position + -headDirection.normalized * 5);

		Gizmos.color = Color.cyan;
		Gizmos.DrawLine (transform.position, transform.position + transform.forward);
		Gizmos.color = Color.white;
	}
}
