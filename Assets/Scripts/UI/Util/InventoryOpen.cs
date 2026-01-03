using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Quick tool to switch open the inventory screen.
/// </summary>
public class InventoryOpen : MonoBehaviour {
	[Header("Scene References")]
	[Tooltip("Reference to the inventory")]
	[SerializeField] FloatingText inventory;
	[Tooltip("Reference to the canvas containing the InventoryUI")]
	[SerializeField] GameObject settingsCanvas;
	[Tooltip("Reference to the canvas containing the actual inventory")]
	[SerializeField] GameObject canvas;
	[Tooltip("Reference to the player transform, which will be used to orient the canvas.")]
	[SerializeField] Player player;

	[Header("Settings")]
	[Tooltip("Added to the position without taking into account direction.")]
	[SerializeField] Vector3 offset;
	[Tooltip("Canvas will be placed this amount in front of the player.")]
	[SerializeField] float distance = 2f;

	void Update () {
		if (player.Dead) {
			return;
		}

		bool shouldSwitch = OVRInput.GetDown (OVRInput.Button.Start);

		if (shouldSwitch) {
			canvas.SetActive (!canvas.activeSelf);

			if (canvas.activeInHierarchy) {
				canvas.gameObject.SetActive (true);
				canvas.transform.position = player.transform.position + player.transform.forward * distance + offset;
				inventory.Allign ();
			}
		}
	}
}
