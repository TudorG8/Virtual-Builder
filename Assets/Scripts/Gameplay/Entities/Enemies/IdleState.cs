using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// In the idle state, enemies check if they are in range of the player and start chasing if they aren't
/// </summary>
public class IdleState : FSMState {
	[Header("Read Only")]
	[Tooltip("Reference to the enemy script.")]
	[SerializeField] Enemy enemy;

	public IdleState (Enemy enemy){
		this.enemy = enemy;
	}

	public override void Reason() {
		if (enemy.Target == null || enemy.Player.Dead) {
			return;
		}

		if (Vector3.Distance (enemy.transform.position, enemy.Destination) > 1) {
			enemy.ClearTriggers ();
			enemy.NavAgent.destination = enemy.Destination;
			enemy.Animator.SetTrigger ("walk");

			enemy.StateSystem.PerformTransition ("StartChasing");
		}
	}

	public override void Act() {
	}
}
