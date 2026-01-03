using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;

/// <summary>
/// An inventory holds a list of items, up to a maximum. Some items are stackable, in which case they will stack up to a limit.
/// <see cref="InventoryItemData.cs"/>
/// Items can be added (after being processed) and removed.
/// </summary>
public class Inventory : MonoBehaviour {
	[Header("Settings")]
	[Tooltip("How many stacks a stackable item can have")]
	[SerializeField] int maxStacks;
	[Tooltip("How many items this inventory can hold. Should be equal to the UI slots")]
	[SerializeField] int maxItems;

	[Header("Read Only")]
	[Tooltip("List of the current items")]
	[SerializeField] List<InventoryItem> inventoryItems;

	[Header("Events")]
	[SerializeField] UnityEvent onInventoryChanged;

	public int MaxItems {
		get {
			return this.maxItems;
		}
		set {
			maxItems = value;
		}
	}

	public List<InventoryItem> InventoryItems {
		get {
			return this.inventoryItems;
		}
		set {
			inventoryItems = value;
		}
	}
		
	/// <summary>
	/// Process a gameobject, checing for the available types (item components and crafting materials as of now).
	/// </summary>
	/// <returns><c>true</c>, if item was processed, <c>false</c> otherwise.</returns>
	/// <param name="obj">Object.</param>
	public bool ProcessItem(GameObject obj) {
		GameObject prefab = null;
		InventoryItemData newItem = null;
		string type = "";

		bool found = false;

		if (!found) {
			ItemComponent itemComponent = obj.GetComponent<ItemComponent> ();
			if (itemComponent != null && itemComponent.Item == null) {
				type = "Component";
				newItem = itemComponent.InventoryItemData;
				prefab = itemComponent.RecipeData.ComponentResult;
				found = true;
			}
		}

		if (!found) {
			CraftingMaterial craftingMaterial = obj.GetComponent<CraftingMaterial> ();
			if (craftingMaterial) {
				type = "Material";
				newItem = craftingMaterial.InventoryItemData;
				prefab = craftingMaterial.MaterialData.Material;
				found = true;
			}
		}

		if (found) {
			return AddNewItem (newItem, type, prefab);
		}

		return false;
	}

	/// <summary>
	/// An item is successfully added if there is a spot for it.
	/// Even if all spots are taken, one of them may be the same type and stackable.
	/// </summary>
	/// <returns><c>true</c>, if new item was added, <c>false</c> otherwise.</returns>
	/// <param name="itemData">Item data.</param>
	/// <param name="type">Type.</param>
	/// <param name="prefab">Prefab.</param>
	public bool AddNewItem(InventoryItemData itemData, string type, GameObject prefab) {
		bool addedItem = false;
		if (!itemData.Stackable && inventoryItems.Count < maxItems) {
			inventoryItems.Add (new InventoryItem (itemData, type, prefab));
			addedItem = true;
		} 
		else {
			int i = 0;
			for (i = 0; i < inventoryItems.Count; i++) {
				InventoryItem item = inventoryItems [i];
				if (item.ItemData.DisplayName.Equals (itemData.DisplayName) && item.Stacks < maxStacks) {
					item.Stacks++;
					addedItem = true;
					break;
				}
			}

			if (i == inventoryItems.Count && i < maxItems) {
				inventoryItems.Add (new InventoryItem (itemData, type, prefab));
				addedItem = true;
			}
		}

		if (addedItem) {
			onInventoryChanged.Invoke ();
			return true;
		}

		return false;
	}

	/// <summary>
	/// Removes an item at a given index, creates the object behind it and returns it.
	/// </summary>
	/// <returns>The prefab related to the item.</returns>
	/// <param name="index">Index.</param>
	public GameObject RemoveItem(InventoryItem item) {
		if (item.ItemData.Stackable && item.Stacks > 1) {
			item.Stacks--;
		} 
		else {
			inventoryItems.Remove (item);
		}

		GameObject newObj = Instantiate (item.Prefab, new Vector3 (), Quaternion.identity);

		onInventoryChanged.Invoke ();

		return newObj;
	}
}
