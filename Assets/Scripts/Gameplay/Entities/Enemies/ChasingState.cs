using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// In the chasing state, enemies check if they are in close range of the player to start attacking them.
/// </summary>
public class ChasingState : FSMState {
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

	public ChasingState (Enemy enemy) {
		this.enemy = enemy;
	}
	

	public override void Reason() {
		if (Vector3.Distance (enemy.transform.position, enemy.Destination) <= 1) {
			enemy.ClearTriggers ();
			enemy.NavAgent.destination = enemy.transform.position;
			enemy.Animator.SetTrigger ("idle");
			enemy.StateSystem.PerformTransition ("StartAttacking");
		}
	}

	public override void Act() {
		enemy.NavAgent.destination = enemy.Destination;

		if (enemy.Player.Dead) {
			enemy.Reset ();
			enemy.Animator.SetTrigger ("idle");
		}
	}
}
