using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// In the dead state, enemies stop doing any actions.
/// </summary>
public class DeadState : FSMState {
	[Header("Read Only")]
	[Tooltip("Reference to the enemy script.")]
	[SerializeField] Enemy enemy;

	public Enemy Enemy {
		get {
			return this.enemy;
		}
		set {
			enemy = value;
		}
	}

	public DeadState (Enemy enemy) {
		this.enemy = enemy;
	}


	public override void Reason() {
	}

	public override void Act() {
	}
}
