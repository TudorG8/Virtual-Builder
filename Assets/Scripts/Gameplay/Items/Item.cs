using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Items are groups of components, which can have bases that give them custom functionality.
/// Items can only be formed by combining item components.
/// </summary>
[System.Serializable]
public class Item : MonoBehaviour {
	[Header("Read Only")]
	[Tooltip("Items can have a maximum of one base component, which will be populated here")]
	[SerializeField] ItemComponent baseComponent;
	[Tooltip("List of components this item is made of")]
	[SerializeField] List<ItemComponent> components;
	[Tooltip("If this item is being held, this will be equal to the specific component being held")]
	[SerializeField] CustomGrabbable currentlyGrabbedComponent;
	[Tooltip("Rigidbody of the item")]
	[SerializeField] Rigidbody rigidBody;

	public ItemComponent BaseComponent {
		get {
			return this.baseComponent;
		}
		set {
			baseComponent = value;
		}
	}

	public CustomGrabbable CurrentlyGrabbedComponent {
		get {
			return this.currentlyGrabbedComponent;
		}
		set {
			currentlyGrabbedComponent = value;
		}
	}

	public List<ItemComponent> Components {
		get { return components; }
		set { components = value; }
	}

	public Rigidbody RigidBody {
		get {
			if (rigidBody == null) {
				rigidBody = GetComponent<Rigidbody> ();
			}
			return this.rigidBody;
		}
		set {
			rigidBody = value;
		}
	}

	/// <summary>
	/// Forces the grabber holding this item to instantly release the item.
	/// </summary>
	public void ForceRelease() {
		if (currentlyGrabbedComponent != null) {
			CustomGrabber grabbable = currentlyGrabbedComponent.grabbedBy as CustomGrabber;
			grabbable.ForceRelease (currentlyGrabbedComponent, false);
		}
	}

	/// <summary>
	/// Called wen the item has been grabbed on one of its components.
	/// </summary>
	/// <param name="grabbedComponent">The grabbed component.</param>
	public void OnComponentGrab(ItemComponent grabbedComponent) {
		if (baseComponent) {
			baseComponent.CustomGrabbable.CanBeGrabbed = false;
		}
		else {
			foreach(ItemComponent component in components) {
				component.CustomGrabbable.CanBeGrabbed = false;
			}
		}
	}

	/// <summary>
	/// Called when this item is released from being held.
	/// </summary>
	public void OnRelease() {
		if (baseComponent) {
			baseComponent.CustomGrabbable.CanBeGrabbed = true;
		} 
		else {
			foreach(ItemComponent component in components) {
				component.CustomGrabbable.CanBeGrabbed = true;
			}
		}
	}

	/// <summary>
	/// Should be called when the item assembly is done.
	/// Will call component.OnComponentConnection for each component, allowing every component to react to new ones.
	/// If there is a base component already, it will switch off all other components.
	/// </summary>
	public void ItemFinished(ConnectorInput input, ConnectorOutput output) {
		ConnectorInformation connectorInformation = new ConnectorInformation (input, output);
		for (int i = 0; i < components.Count; i++) {
			ItemComponent component = components [i];

			component.OnItemFinished.Invoke (connectorInformation);

			if (baseComponent != null && component != baseComponent) {
				component.CustomGrabbable.CanBeGrabbed = false;
			}
		}
	}

	/// <summary>
	/// Returns a list with all modifiers present on all components
	/// </summary>
	public List<Modifier> GetComponentsModifiers() {
		List<Modifier> modifiers = new List<Modifier> ();
		for (int i = 0; i < components.Count; i++) {
			ItemComponent component = components [i];

			modifiers.AddRange (component.ModifierData.Modifiers);
		}

		return modifiers;
	}
}