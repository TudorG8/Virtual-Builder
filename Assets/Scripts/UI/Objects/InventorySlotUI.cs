using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

/// <summary>
/// An UI element that represents an inventory slot
/// </summary>
public class InventorySlotUI : MonoBehaviour {
	[System.Serializable]
	public class ClickEvent : UnityEvent<InventoryItem> {}

	[Header("Object References.")]
	[Tooltip("Reference to the item name text.")]
	[SerializeField] TextMeshProUGUI itemName;
	[Tooltip("Reference to the background of the object.")]
	[SerializeField] Image iconBackground;
	[Tooltip("Reference to the main image that will be changes to display items.")]
	[SerializeField] Image icon;
	[Tooltip("Reference to a text element that shows how many stacks this item has.")]
	[SerializeField] TextMeshProUGUI stacks;
	[Tooltip("Reference to a text element that shows the type of this element (example material)")]
	[SerializeField] TextMeshProUGUI type;

	[Header("Read Only")]
	[Tooltip("The inventory item associated with this slot.")]
	[SerializeField] InventoryItem item;
	[Tooltip("Will be called whenever this element is clicked")]
	[SerializeField] ClickEvent onClick;

	public TextMeshProUGUI ItemName {
		get {
			return this.itemName;
		}
		set {
			itemName = value;
		}
	}

	public Image IconBackground {
		get {
			return this.iconBackground;
		}
		set {
			iconBackground = value;
		}
	}

	public Image Icon {
		get {
			return this.icon;
		}
		set {
			icon = value;
		}
	}

	public TextMeshProUGUI Stacks {
		get {
			return this.stacks;
		}
		set {
			stacks = value;
		}
	}

	public TextMeshProUGUI Type {
		get {
			return this.type;
		}
		set {
			type = value;
		}
	}

	public InventoryItem Item {
		get {
			return this.item;
		}
		set {
			item = value;
		}
	}

	public ClickEvent OnClick {
		get {
			return this.onClick;
		}
		set {
			onClick = value;
		}
	}
		
	public void OnClickEvent() {
		onClick.Invoke (item);
	}
}
