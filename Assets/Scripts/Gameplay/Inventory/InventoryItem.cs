using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Format for holding items in an inventory.
/// </summary>
[System.Serializable]
public class InventoryItem {
	[Tooltip("The type of the item ex(materials/components).")]
	[SerializeField] string type;
	[Tooltip("The maximum stacks this item can have, leave at 1 for unstackable items.")]
	[SerializeField] int stacks;
	[Tooltip("Reference to the item data scriptable object.")]
	[SerializeField] InventoryItemData itemData;
	[Tooltip("The object this recipe belongs to.")]
	[SerializeField] GameObject prefab;

	public string Type {
		get {
			return this.type;
		}
		set {
			type = value;
		}
	}

	public int Stacks {
		get {
			return this.stacks;
		}
		set {
			stacks = value;
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

	public GameObject Prefab {
		get {
			return this.prefab;
		}
		set {
			prefab = value;
		}
	}

	public InventoryItem (InventoryItemData itemData, string type, GameObject prefab) {
		this.itemData = itemData;
		this.type = type;
		this.stacks = 1;
		this.prefab = prefab;
	}
}
