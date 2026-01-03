using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Item base for tools and weapons that have offensive parts attached to some of their components.
/// </summary>
public class ToolItemBase : ItemBase {
	[Header("Read Only")]
	[Tooltip("A list of all added offensive parts.")]
	[SerializeField] List<OffensiveEntity> offensiveParts;

	void Start() {
		UpdateStats ();
	}

	/// <summary>
	/// Whenever a new component is added, check if it is a gun shooting point
	/// </summary>
	public override void OnComponentConnection(ConnectorInformation connectorInformation) {
		base.OnComponentConnection (connectorInformation);

		ItemComponent newComponent = connectorInformation.Output.Component;

		OffensiveEntity offensiveEntity = newComponent.GetComponent<OffensiveEntity> ();
		if (offensiveEntity) {
			offensiveParts.Add (offensiveEntity);
		} 

		for (int i = 0; i < offensiveParts.Count; i++) {
			OffensiveEntity offensivePart = offensiveParts [i];
			offensivePart.GetStat(ModifierName.Damage).CurrentMaximum = stats.Find(stat => stat.Name == ModifierName.Damage).Current;
		}
	}
}
