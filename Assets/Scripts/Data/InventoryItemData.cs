using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Scriptable Data for an item that can be stored in an inventory.
/// </summary>
public class InventoryItemData : ScriptableObject {
	#if UNITY_EDITOR
	[MenuItem("Assets/Level Design/Create/Inventory Item")]
	public static void CreateMyAsset() {
		InventoryItemData asset = ScriptableObject.CreateInstance<InventoryItemData>(); 

		AssetDatabase.CreateAsset(asset, "Assets/Data/inventory_item.asset");
		AssetDatabase.SaveAssets();

		EditorUtility.FocusProjectWindow();

		Selection.activeObject = asset;
	}
	#endif

	[Tooltip("Name that will be displayed in the UI")]
	[SerializeField] string displayName;
	[Tooltip("Whether this inventory item can stack (example: for materials)")]
	[SerializeField] bool stackable;
	[Tooltip("An image/icon to display in the inventory format")]
	[SerializeField] Sprite displayImage;

	public string DisplayName {
		get {
			return this.displayName;
		}
		set {
			displayName = value;
		}
	}

	public bool Stackable {
		get {
			return this.stackable;
		}
		set {
			stackable = value;
		}
	}

	public Sprite DisplayImage {
		get {
			return this.displayImage;
		}
		set {
			displayImage = value;
		}
	}
}
