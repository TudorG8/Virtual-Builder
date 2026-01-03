using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Scritable Data for Materials.
/// </summary>
public class CraftingMaterialData : ScriptableObject {
	#if UNITY_EDITOR
	[MenuItem("Assets/Level Design/Create/Material")]
	public static void CreateMyAsset() {
		CraftingMaterialData asset = ScriptableObject.CreateInstance<CraftingMaterialData>();

		AssetDatabase.CreateAsset(asset, "Assets/Data/Materials/material.asset");
		AssetDatabase.SaveAssets();

		EditorUtility.FocusProjectWindow();

		Selection.activeObject = asset;
	}
	#endif

	[Tooltip("The type of the material. Add a purpose later")]
	[SerializeField] string type;
	[Tooltip("Prefab that symbolizes the gameobject of this material.")]
	[SerializeField] GameObject material;
	[Tooltip("Reference to the inventory item data.")]
	[SerializeField] InventoryItemData itemData;

	public string Type {
		get {
			return this.type;
		}
		set {
			type = value;
		}
	}

	public GameObject Material {
		get {
			return this.material;
		}
		set {
			material = value;
		}
	}

	public InventoryItemData ItemData {
		get {
			return this.itemData;
		}
		set {
			itemData = value;
		}
	}
}
