using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A modifier can be added to a stat to change its value.
/// </summary>
[System.Serializable]
public class Modifier {
	[SerializeField] ModifierName name;
	[SerializeField] ModifierType type;
	[SerializeField] float amount;

	public ModifierName Name {
		get {
			return this.name;
		}
		set {
			name = value;
		}
	}

	public ModifierType Type {
		get {
			return this.type;
		}
		set {
			type = value;
		}
	}

	public float Amount {
		get {
			return this.amount;
		}
		set {
			amount = value;
		}
	}
}
