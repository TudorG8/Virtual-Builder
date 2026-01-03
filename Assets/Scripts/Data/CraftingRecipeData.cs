using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Scriptable Data for Item Components
/// </summary>
[System.Serializable]
public class CraftingRecipeData : ScriptableObject {
	#if UNITY_EDITOR
	[MenuItem("Assets/Level Design/Create/Crafting Recipe")]
	public static void CreateMyAsset() {
		CraftingRecipeData asset = ScriptableObject.CreateInstance<CraftingRecipeData>();

		AssetDatabase.CreateAsset(asset, "Assets/Data/Components/crafting_recipe.asset");
		AssetDatabase.SaveAssets();

		EditorUtility.FocusProjectWindow();

		Selection.activeObject = asset;
	}
	#endif

	[System.Serializable]
	public class Pair {
		[SerializeField] string type;
		[SerializeField] int amount;
		public string Type {
			get {
				return this.type;
			}
		}

		public int Amount {
			get {
				return this.amount;
			}
		}
	}

	[Tooltip("List of materials used to create this component")]
	[SerializeField] List<Pair> materials;
	[Tooltip("Unique name to identify a component")]
	[SerializeField] string componentName;
	[Tooltip("Prefab that symbolizes the gameobject of this component.")]
	[SerializeField] GameObject componentResult;
	[Tooltip("Reference to the inventory item data.")]
	[SerializeField] InventoryItemData itemData;

	public List<Pair> Materials {
		get {
			return this.materials;
		}
	}

	public string Name {
		get {
			return this.componentName;
		}
	}

	public GameObject ComponentResult {
		get {
			return this.componentResult;
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
