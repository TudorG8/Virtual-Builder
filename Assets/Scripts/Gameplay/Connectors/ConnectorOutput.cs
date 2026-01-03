using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Outputs create a ray in their forward direction and try to detect an <see cref="ConnectorInput">Input</see>.
/// Upon detecting a free input, the two will connect together and their parents (the components), will be merged together.
/// If the parents are part of items, such items will be merged instead.
/// </summary>
public class ConnectorOutput : MonoBehaviour { 
    [Header("Object References")]
    [Tooltip("Reference to the main component this output belongs to.")]
	[SerializeField] ItemComponent component;

    [Header("Settings")]
    [Tooltip("The layer that the ray will check for inputs on.")]
	[SerializeField] LayerMask layerMask;
	[Tooltip("How far the ray will be cast")]
	[SerializeField] float distance;
	[Tooltip("Colour of the ray, present in the editor only")]
	[SerializeField] Color color;
	
	[Header("Read only")]
	[Tooltip("Whether this component has been connected to an input yet.")]
	[SerializeField][ReadOnly] bool connected;
	[Tooltip("The output currently connected to this element.")]
	[SerializeField] ConnectorInput connectedTo;

	public ItemComponent Component {
		get { return component; }
	}

	public ConnectorInput ConnectedTo {
		get {
			return this.connectedTo;
		}
		set {
			connectedTo = value;
		}
	}

	void Update () {
		RaycastHit hit;

		if (connected) {
			return;
		}

		Ray ray = new Ray (transform.position, transform.TransformDirection (Vector3.forward));
		if (Physics.Raycast (ray, out hit, distance, layerMask)) {
			
			ConnectorInput receiver = hit.transform.GetComponent<ConnectorInput> ();
			
			// Avoid connecting to inputs that already have an output connected.
			if (receiver != null && !receiver.Connected) {
				if (!IsValidConnection (this.component, receiver.Component)) {
					return;
				}

				if (component.AttachedRigidbody != null) {
					component.AttachedRigidbody.isKinematic = true;
				}

				if (receiver.Component.AttachedRigidbody != null) {
					receiver.Component.AttachedRigidbody.isKinematic = true;
				}

				Transform targetTransform    = hit.collider.transform;
				Transform targetParent       = hit.collider.transform.parent;
				Transform componentTransform = this.transform;
				Transform componentParent    = component.Item != null ? component.Item.transform : component.transform;

				OrientateNewPiece (componentTransform, componentParent, targetTransform);

				receiver.Component.OnConnectorConnection (this, receiver);

				// Avoid further connections from other components
				connected = true;
				receiver.Connected = true;

				// Link each piece to eachother
				connectedTo = receiver;
				receiver.ConnectedTo = this;
			} 
		} 

		Debug.DrawRay(ray.origin, ray.direction * distance, color);
	}

	/// <summary>
	/// Checks if the connection between two components would be valid.
	/// This checks so that the resulting item would not have more than one item base and more than one unique component.
	/// </summary>
	bool IsValidConnection(ItemComponent first, ItemComponent second) {
		List<ItemComponent> allComponents = new List<ItemComponent> ();
		allComponents.AddRange (first.RetrieveItemComponents ());
		allComponents.AddRange (second.RetrieveItemComponents ());

		Dictionary<string, int> validDictionary = new Dictionary<string, int> ();
		for (int i = 0; i < allComponents.Count; i++) {
			ItemComponent component = allComponents [i];

			if (component.Unique) {
				int count;
				validDictionary.TryGetValue(component.Type, out count); 
				validDictionary[component.Type] = count + 1;
			}
		}

		foreach (KeyValuePair<string, int> type in validDictionary) {
			if (type.Value > 1) {
				Debug.Log (type.Key + " " + type.Value);
				return false;
			}
		}
			
		return true;
	}

	/// <summary>
	/// <para> A bit of convoluted logic here, but the main goal is to rotate the parent in such a way that the child (component) faces in the 
	/// opposite direction of the target.
	/// First, the child is rotated on the forward and up axis and the values are stored as quaternions. 
	/// These will be applied to the parent to make it match the direction as well.
	/// The child's direction must be reset and it must go through the rotations as well.
	/// Finally, the parent is moved based on the distance between it and the child.</para>
	/// 
	/// <para> This makes sure that the the input will always match the output location, regardless of how the parent looks like.
	/// There might be a better way to do this, but I am not that great at quaternions yet.</para>
	/// </summary>
	public static void OrientateNewPiece(Transform componentTransform, Transform componentParent, Transform targetTransform ) {
		// Orientate the parent so that the connector faces the opposite of the input
		Quaternion original = componentTransform.rotation;

		// Orientate the forward axis
		Quaternion forwardRotation = Quaternion.FromToRotation (componentTransform.forward, targetTransform.forward * -1);

		// Keeping it here to easily access the up vector
		componentTransform.rotation = forwardRotation * componentTransform.rotation;

		// Orientate the up axis
		Quaternion upRotation = Quaternion.FromToRotation (componentTransform.up, targetTransform.up);

		// Rotate the parent with the found rotations
		componentParent.rotation = forwardRotation * componentParent.rotation;
		componentParent.rotation = upRotation * componentParent.rotation;

		// At this point, the child has gone through two rotations (one from the simulation and one from the parent).
		componentTransform.rotation = upRotation * (forwardRotation * original);

		// Move the parent
		Vector3 movement = targetTransform.position - componentTransform.position;
		componentParent.transform.position = componentParent.transform.position + movement;
	}
}