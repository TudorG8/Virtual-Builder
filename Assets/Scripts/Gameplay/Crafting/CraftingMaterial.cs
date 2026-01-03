using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary
/// A crafting material is used in the crafting of components, mainly in <see cref="CraftingStation.MaterialAdded"> Crafting Stations </see>.
/// </summary>
public class CraftingMaterial : MonoBehaviour {
    [Header("Settings")]
    [Tooltip("The unique identifier/name of this material.")]
	[SerializeField] string type;
	
	[Header("Object References")]
	[Tooltip("Reference to the CustomGrabbable component")]
	[SerializeField] protected CustomGrabbable customGrabbable;
	[Tooltip("Reference to the canvas that will display the name when the object is close.")]
	[SerializeField] FloatingText ui;
	[Tooltip("Reference to the text that displays the type field. Will be capitablized")]
	[SerializeField] TextMeshProUGUI titleText;

	[Header("Scriptable Data")]
	[Tooltip("Scriptable Data used to display this component in the inventory")]
	[SerializeField] InventoryItemData inventoryItemData;
	[Tooltip("Scriptable Data holding information about the material")]
	[SerializeField] CraftingMaterialData materialData;

	public string Type {
		get {
			return this.type;
		}
	}

	public InventoryItemData InventoryItemData {
		get {
			return this.inventoryItemData;
		}
		set {
			inventoryItemData = value;
		}
	}

	public CraftingMaterialData MaterialData {
		get {
			return this.materialData;
		}
		set {
			materialData = value;
		}
	}

	/// <summary>
	/// Should be called whenever this material becomes targetted by a hand
	/// </summary>
	public void OnBecomeTargeted() {
		if (customGrabbable.grabbedBy == null) {
			ui.gameObject.SetActive (true);
			ui.Allign ();
			titleText.text = char.ToUpper (type [0]) + type.Substring (1);
		}
	}

	/// <summary>
	/// Should be called whenever this material becomes untargetted by a hand
	/// </summary>
	public void OnBecomeUntargeted() {
		ui.gameObject.SetActive (false);
	}
}
