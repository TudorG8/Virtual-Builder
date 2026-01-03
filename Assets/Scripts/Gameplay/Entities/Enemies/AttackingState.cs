using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// In the attacking state, enemies periodically deal damage to the player.
/// </summary>
public class AttackingState : FSMState {
	[Header("Read Only")]
	[Tooltip("Reference to the enemy script.")]
	[SerializeField] Enemy enemy;
	[Tooltip("Whether an attack can be made.")]
	[SerializeField] bool canAttack = true;

	[Header("Settings")]
	[Tooltip("How long to wait between each attack.")]
	[SerializeField] float attackCooldown = 3f;

	public Enemy Enemy {
		get {
			return this.enemy;
		}
		set {
			enemy = value;
		}
	}

	public AttackingState (Enemy enemy) {
		this.enemy = enemy;
	}


	public override void Reason() {
		if (Vector3.Distance (enemy.transform.position, enemy.Destination) > 1.5) {
			enemy.ClearTriggers ();
			enemy.NavAgent.destination = enemy.transform.position;
			enemy.Animator.SetTrigger ("walk");
			enemy.StateSystem.PerformTransition ("StartChasing");
		}
	}

	public override void Act() {
		if (canAttack) {
			if (enemy.Player.Dead) {
				enemy.Reset ();
				return;
			}

			enemy.StartCoroutine (AttackRoutine ());
			enemy.StartCoroutine (DamageRoutine ());
			enemy.Animator.SetTrigger ("attack");
			enemy.AttackSound.PlayFrom (enemy.AudioSource);
		}

		Quaternion targetRotation = Quaternion.LookRotation (enemy.Destination - enemy.transform.position);
		enemy.transform.rotation = Quaternion.Slerp (enemy.transform.rotation, targetRotation, 10f * Time.deltaTime);
	}

	IEnumerator DamageRoutine() {
		yield return new WaitForSeconds (1.5f);
		enemy.Player.Health.Current -= enemy.Damage.Current;
	}

	IEnumerator AttackRoutine() {
		canAttack = false;
		yield return new WaitForSeconds (attackCooldown);
		canAttack = true;
	}
}
