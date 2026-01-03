using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// A heldable component that can be connected to other components.
/// Implementation also allows for groups of components (items) to be connected with either components or groups.
/// </summary>
[System.Serializable]
public class ItemComponent : MonoBehaviour {
	[Header("Settings")]
	[Tooltip("Whether more than one of this component can be attached to an item.")]
	[SerializeField] bool unique;
	[Tooltip("The type used to mark the uniqueness of this component")]
	[SerializeField] string type;

    [Header("Object References")]
    [Tooltip("Reference to the attached rigidbody. When this component becomes part of an item, this component will be destroyed.")]
    [SerializeField] Rigidbody _rigidBody;
	[Tooltip("Reference to the CustomGrabbable component")]
	[SerializeField] CustomGrabbable customGrabbable;
	[Tooltip("References to all inputs attached to this component")]
	[SerializeField] List<ConnectorInput> inputs;
	[Tooltip("References to all outputs attached to this component")]
	[SerializeField] List<ConnectorOutput> outputs;
	[Tooltip("Reference to the floating text that will display the name when the object is close. ")]
	[SerializeField] FloatingText ui;
	[Tooltip("Reference to the text that displays the type field. Will be capitablized")]
	[SerializeField] TextMeshProUGUI titleText;

	[Header("Scriptable Data")]
	[Tooltip("Scriptable Data used to display this component in the inventory")]
	[SerializeField] InventoryItemData inventoryItemData;
	[Tooltip("Scriptable Data used to craft this component")]
	[SerializeField] CraftingRecipeData recipeData;
	[Tooltip("Scriptable Data that has all the modifiers of the component")]
	[SerializeField] ComponentModifierData modifierData;

	[Header("Read only")]
	[Tooltip("Will be null unless this is part of an item.")]
	[SerializeField][ReadOnly] Item item;
	[SerializeField] List<ConnectorOutput> connectedPieces;

	[Header("Events")]
	[SerializeField] ConnectorInformationEvent onItemFinished;

	public bool Unique {
		get {
			return this.unique;
		}
		set {
			unique = value;
		}
	}

	public string Type {
		get {
			return this.type;
		}
		set {
			type = value;
		}
	}

	public List<ConnectorInput> Inputs {
		get {
			return this.inputs;
		}
		set {
			inputs = value;
		}
	}

	public List<ConnectorOutput> Outputs {
		get {
			return this.outputs;
		}
		set {
			outputs = value;
		}
	}

	public Item Item {
		get {
		    return item; 
		}
		set { 
		    item = value; 
		}
	}

	public CustomGrabbable CustomGrabbable {
		get {
			return this.customGrabbable;
		}
		set {
			customGrabbable = value;
		}
	}

	public Rigidbody AttachedRigidbody {
		get { 
			if (item != null) {
				return item.RigidBody;
			}
			return _rigidBody; 
		}
		set { 
		    _rigidBody = value; 
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

	public CraftingRecipeData RecipeData {
		get {
			return this.recipeData;
		}
		set {
			recipeData = value;
		}
	}

	public ComponentModifierData ModifierData {
		get {
			return this.modifierData;
		}
		set {
			modifierData = value;
		}
	}

	public ConnectorInformationEvent OnItemFinished {
		get {
			return this.onItemFinished;
		}
		set {
			onItemFinished = value;
		}
	}

	public List<Modifier> GetComponentsModifiers() {
		if (item != null) {
			return item.GetComponentsModifiers ();
		}
		List<Modifier> modifiers = new List<Modifier> ();
		modifiers.AddRange (modifierData.Modifiers);
		return modifiers;
	}

	public void Duplicate() {
		GameObject copy = Instantiate (gameObject, new Vector3 (), Quaternion.identity);
		copy.transform.position = transform.position;
	}

	/// <summary>
	/// To be called (attached) to the CustomGrabbable grab begin event.
	/// Sends the grab begin signal to the item
	/// </summary>
	public void OnGrabBegin() {
		if (item != null) {
			item.OnComponentGrab (this);
		}
	}

   	/// <summary>
	/// To be called (attached) to the CustomGrabbable grab end event.
	/// Sends the grab end signal to the item
	/// </summary>
	public void OnGrabEnd() {
		if (item != null) {
			item.OnRelease ();
		}
	}

	/// <summary>
	/// Forces release if this component is grabbed, or its item if it exists.
	/// </summary>
	public void ForceRelease() {
		if (item != null) {
			item.ForceRelease ();
		} 
		else {
			if (customGrabbable.grabbedBy != null) {
				(customGrabbable.grabbedBy as CustomGrabber).ForceRelease (customGrabbable, false);
			}
		}
	}

	/// <summary>
	/// Disables the inputs and outputs of this component.
	/// </summary>
	public void Disable() {
		for (int i = 0; i < inputs.Count; i++) {
			inputs [i].gameObject.SetActive (false);
		}
		for (int i = 0; i < outputs.Count; i++) {
			outputs [i].gameObject.SetActive (false);
		}

		ui.gameObject.SetActive (false); 
	}

	/// <summary>
	/// Quick way of retrieving all components. If the component is part of an item, this method will retrieve all components of said item.
	/// </summary>
	/// <returns>All components.</returns>
	public List<ItemComponent> RetrieveItemComponents() {
		List<ItemComponent> components = new List<ItemComponent> ();
		if (item != null) {
			components.AddRange (item.Components);
		}
		else {
			components.Add (this);
		}
		return components;
	}

	/// <summary>
	/// Quick way of getting the currently grabbed object. If the component is part of an item, this method will get the currently held component of the item.
	/// </summary>
	/// <returns>The currently grabbed object.</returns>
	public CustomGrabbable GetCurrentlyGrabbedObject() {
		if (item != null) {
			return item.CurrentlyGrabbedComponent;
		}
		return customGrabbable;
	}

	/// <summary>
	/// Should be called whenever another components connects to this component.
	/// In this case, this component is the "input"
	/// </summary>
	/// <param name="other">The other component, which is the "output"</param>
	public void OnConnectorConnection(ConnectorOutput output, ConnectorInput input) {
		CustomGrabbable currentlyGrabbedItem = input.Component.GetCurrentlyGrabbedObject ();
		OVRGrabber hand = currentlyGrabbedItem.grabbedBy;
		Collider grabPoint = currentlyGrabbedItem.GrabbedCollider;

		if (this.item != null && this.item == output.Component.item) {
			return;
		}

		List<ItemComponent> components = new List<ItemComponent> ();
		components.AddRange (this.RetrieveItemComponents ( ));
		components.AddRange (output.Component.RetrieveItemComponents ());

		// Need to force release the held components and destroy the rigid bodies, since there will be a single rigid body on the output item.
		for (int i = 0; i < components.Count; i++) {
			ItemComponent component = components [i];
			component.ForceRelease ();

			if (component.AttachedRigidbody != null) {
				Destroy (component.AttachedRigidbody);
			}
		}

	 	// Destroy takes place on the next frame, so this needs to be delayed atleast one frame
		OnTheNextFrame (() => {
			// Create the new Item
			GameObject itemBase = Instantiate(PopupSpawner.Instance.ItemBase, new Vector3(), Quaternion.identity);
			Item newItem = itemBase.GetComponent<Item> ();
			newItem.Components = components;

			// Recycle old items
			if (item != null) {
				item.transform.DetachChildren();
				Destroy (item.gameObject);
			}
			if (output.Component.item != null) {
				output.Component.item.transform.DetachChildren();
				Destroy (output.Component.item.gameObject);
			}

			// Calculate new position
			Vector3 newPosition = new Vector3();
			ItemComponent componentToUseForRotation = components [0];
			for (int i = 0; i < components.Count; i++) {
				ItemComponent component = components [i];
				component.Item = newItem;
				component.customGrabbable.Item = newItem;
				newPosition += component.transform.position;

				ItemBase isItemBase = component.GetComponent<ItemBase>();

				if (isItemBase) {
					componentToUseForRotation = component;
					newItem.BaseComponent = component;
				}
			}
			newPosition /= components.Count;
			itemBase.transform.position = componentToUseForRotation.transform.position;
			itemBase.transform.rotation = componentToUseForRotation.transform.rotation;

			// Need to set parent after the new position is calculated
			for (int i = 0; i < components.Count; i++) {
				ItemComponent component = components [i];
				component.transform.SetParent (itemBase.transform);
			}

			newItem.ItemFinished(input, output);

			// If the previous component/item was beind held, make the hand hold the newly made item
			if(hand != null && hand is CustomGrabber) {
				CustomGrabber handGrabber = hand as CustomGrabber;

				handGrabber.M_target = currentlyGrabbedItem;
				handGrabber.M_targetCollider = grabPoint;

				handGrabber.ForceGrabBegin();
			}
		});

	}

	/// <summary>
	/// Quick way to trigger an action on the next frame.
	/// </summary>
	delegate void Action();
	void OnTheNextFrame(Action action) {
		StartCoroutine (NextFrameRoutine (action));
	}
	IEnumerator NextFrameRoutine(Action action) {
		yield return new WaitForEndOfFrame ();
		action ();
	}

	/// <summary>
	/// Should be called whenever this component becomes targetted by a hand
	/// </summary>
	public void OnBecomeTargeted() {
		if (customGrabbable.grabbedBy == null && item == null) {
			ui.gameObject.SetActive (true);
			ui.Allign ();
			titleText.text = char.ToUpper (recipeData.Name [0]) + recipeData.Name.Substring (1);
		}
	}

	/// <summary>
	/// Should be called whenever this component becomes untargetted by a hand
	/// </summary>
	public void OnBecomeUntargeted() {
		ui.gameObject.SetActive (false);
	}
}