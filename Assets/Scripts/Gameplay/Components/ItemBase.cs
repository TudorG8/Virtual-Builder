using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Item bases are specialized components that add special functionality.
/// All actual bases should inherit from this, as item components will detect the bases and call events when new components are added to the item.
/// </summary>
public class ItemBase : MonoBehaviour {
	[Header("Object References")]
	[Tooltip("Reference to the item component.")]
	[SerializeField] protected ItemComponent itemComponent;
	[Tooltip("A list of stats that this component uses.")]
	[SerializeField] protected List<FloatStat> stats;

	public ItemComponent ItemComponent {
		get {
			return this.itemComponent;
		}
		set {
			itemComponent = value;
		}
	}

	void Start() {
		UpdateStats ();
	}

	/// <summary>
	/// Call this to update stats whenever a new component is added.
	/// </summary>
	protected void UpdateStats() {
		DataModifierConverter.Instance.AddModifiersToStats (itemComponent.GetComponentsModifiers (), true, stats);
	}

	/// <summary>
	/// Called whenever an input is connected to an output. 
	/// This should be called when an item is finished.
	/// </summary>
	public virtual void OnComponentConnection(ConnectorInformation connectorInformation) {
		UpdateStats ();
	}
}
