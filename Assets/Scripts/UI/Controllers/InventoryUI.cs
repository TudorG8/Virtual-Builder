using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UI Controller for inventories, that handles updating the inventory slots to reflect the state of the actual inventory.
/// </summary>
public class InventoryUI : MonoBehaviour {
	[Header("Scene References")]
	[Tooltip("Reference to the inventory script.")]
	[SerializeField] Inventory inventory;
	[Tooltip("Where all the slots are located under.")]
	[SerializeField] Transform slotParent;
	[Tooltip("A list of references to all inventory slots.")]
	[SerializeField] List<InventorySlotUI> inventorySlots;

	void Awake() {
		inventory.MaxItems = slotParent.childCount;
	}

	/// <summary>
	/// Updates the UI to reflect the state of the inventory.
	/// </summary>
	public void UpdateUI() {
		for (int i = 0; i < inventory.InventoryItems.Count; i++) {
			InventoryItem item = inventory.InventoryItems [i];
			InventorySlotUI slot = inventorySlots [i];

			UpdateSlotToItem (slot, item);
		}

		for (int i = inventory.InventoryItems.Count; i < slotParent.childCount; i++) {
			InventorySlotUI slot = inventorySlots [i];
			ClearSlot (slot);
		}
	}

	/// <summary>
	/// Makes a slot show an item.
	/// </summary>
	void UpdateSlotToItem(InventorySlotUI inventorySlot, InventoryItem item) {
		inventorySlot.Item = item;
		inventorySlot.Icon.gameObject.SetActive (true);
		if (item.ItemData.Stackable) {
			inventorySlot.Stacks.gameObject.SetActive (true);
		}
			
		inventorySlot.Icon.sprite = item.ItemData.DisplayImage;
		inventorySlot.Stacks.text = "x" + item.Stacks.ToString();

		inventorySlot.OnClick.RemoveAllListeners ();
		inventorySlot.OnClick.AddListener ((inventoryItem) => {
			RemoveItem(inventorySlot, inventoryItem);
		});
	}

	/// <summary>
	/// Clears a slot from having any item in them.
	/// </summary>
	void ClearSlot(InventorySlotUI inventorySlot) {
		inventorySlot.Item = null;
		inventorySlot.Icon.gameObject.SetActive (false);
		inventorySlot.Stacks.gameObject.SetActive (false);
		inventorySlot.OnClick.RemoveAllListeners ();
	}

	/// <summary>
	/// Should be called whenever an item is grabbed from the inventory.
	/// </summary>
	public void RemoveItem(InventorySlotUI inventorySlot, InventoryItem item) {
		GameObject removedItem = inventory.RemoveItem (item);

		if (removedItem) {
			removedItem.transform.position = inventorySlot.transform.position;

			CustomGrabbable grabbale = removedItem.GetComponent<CustomGrabbable> ();

			if (grabbale) {
				grabbale.MakeUngrabbableFor (0.5f);
			}
		}
	}
}
