using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// An enemy class that uses a state system to dictate its actions and a nav mesh agent to nagivate the terrain.
/// During the day, it will slowly lose its hitpoints until death.
/// Includes behaviours for idling, chasing, attacking and death.
/// </summary>
public class Enemy : MonoBehaviour {
	[Header("Object References")]
	[Tooltip("Reference to the state system.")]
	[SerializeField] FSMSystem stateSystem;
	[Tooltip("Reference to animator component.")]
	[SerializeField] Animator animator;
	[Tooltip("Reference to the nav agent component.")]
	[SerializeField] NavMeshAgent navAgent;
	[Tooltip("Reference to all rigidbodies on this enemy.")]
	[SerializeField] List<Rigidbody> rigidBodies;
	[Tooltip("Reference to the health progress bar.")]
	[SerializeField] ProgressBarUI progressBar;
	[Tooltip("Reference to DamageableEntity component.")]
	[SerializeField] DamageableEntity damageableEntity;
	[Tooltip("Reference to the ui canvas.")]
	[SerializeField] GameObject ui;
	[Tooltip("Reference to the audio source.")]
	[SerializeField] AudioSource audioSource;
	[Tooltip("Reference to the on fire effect.")]
	[SerializeField] ParticleSystem onFireEffect;

	[Header("Prefabs")]
	[Tooltip("The clip that should be played when the enemy growls.")]
	[SerializeField] VolumeClip growlSound;
	[Tooltip("The clip that should be played when the enemy is hit.")]
	[SerializeField] VolumeClip hitSound;
	[Tooltip("The clip that should be played when the enemy dies.")]
	[SerializeField] VolumeClip deathSound;
	[Tooltip("The clip that should be played when the enemy attacks.")]
	[SerializeField] VolumeClip attackSound;

	[Header("Settings")]
	[Tooltip("How long it takes for the body to dissapear on death.")]
	[SerializeField] float timeToDie;
	[Tooltip("Whether this enemy is dead.")]
	[SerializeField] bool dead;
	[Tooltip("How much damage this enemy deals to the player.")]
	[SerializeField] FloatStat damage;

	[Header("Read Only")]
	[Tooltip("Reference to the Idle state of the state system.")]
	[SerializeField] IdleState idleState;
	[Tooltip("Reference to the Chasing state of the state system.")]
	[SerializeField] ChasingState chasingState;
	[Tooltip("Reference to the Attacking state of the state system.")]
	[SerializeField] AttackingState attackingState;
	[Tooltip("Reference to the Dead state of the state system.")]
	[SerializeField] DeadState deadState;
	[Tooltip("Should be set at runtime by the spawner")]
	[SerializeField] Player player;
	[Tooltip("True when the fire damage cooldown is off.")]
	[SerializeField] bool canTakeFireDamage = true;
	[Tooltip("Will be true between functions just as fire damage was taken.")]
	[SerializeField] bool tookDamageFromFire = false;
	[Tooltip("Will be equal to the player when they are detected.")]
	[SerializeField] Transform target;
	[Tooltip("Positions the nav agent will follow.")]
	[SerializeField] Vector3 destination;
	[Tooltip("True when the zombie can let out a growl.")]
	[SerializeField] bool canGrowl = true;

	public FSMSystem StateSystem {
		get {
			return this.stateSystem;
		}
		set {
			stateSystem = value;
		}
	}

	public Animator Animator {
		get {
			return this.animator;
		}
		set {
			animator = value;
		}
	}

	public NavMeshAgent NavAgent {
		get {
			return this.navAgent;
		}
		set {
			navAgent = value;
		}
	}

	public Vector3 Destination {
		get {
			return this.destination;
		}
		set {
			destination = value;
		}
	}

	public Transform Target {
		get {
			return this.target;
		}
		set {
			target = value;
		}
	}

	public List<Rigidbody> RigidBodies {
		get {
			return this.rigidBodies;
		}
		set {
			rigidBodies = value;
		}
	}

	public Player Player {
		get {
			return this.player;
		}
		set {
			player = value;
		}
	}

	public FloatStat Damage {
		get {
			return this.damage;
		}
		set {
			damage = value;
		}
	}

	public VolumeClip AttackSound {
		get {
			return this.attackSound;
		}
		set {
			attackSound = value;
		}
	}

	public AudioSource AudioSource {
		get {
			return this.audioSource;
		}
		set {
			audioSource = value;
		}
	}

	void Awake() {
		SetUp ();
		progressBar.Fill.fillAmount = 1;
	}

    void Update() {
		if (target != null) {
			destination = target.position;
			destination.y = transform.position.y;
		}

		if (DayNightCycle.Instance.CurrentTime <= 0.5f && canTakeFireDamage && !dead) {
			if (!onFireEffect.isPlaying) {
				onFireEffect.Play ();
			}
			tookDamageFromFire = true;
			damageableEntity.Health.Current -= damageableEntity.Health.CurrentMaximum / 10f;
			StartCoroutine (BurnRoutine ());
		}

		if (canGrowl && !dead) {
			StartCoroutine(GrowlRoutine(Random.Range(5, 11)));
			growlSound.PlayFrom (audioSource);
		}
    }

	IEnumerator GrowlRoutine(float seconds) {
		canGrowl = false;
		yield return new WaitForSeconds (seconds);
		canGrowl = true;
	}

	/// <summary>
	/// Cooldown for taking damage from fire.
	/// </summary>
	IEnumerator BurnRoutine() {
		canTakeFireDamage = false;
		yield return new WaitForSeconds (1f);
		canTakeFireDamage = true;
	}

	/// <summary>
	/// Sets up the state system.
	/// </summary>
	public void SetUp() {
		idleState = new IdleState (this);
		chasingState = new ChasingState (this);
		attackingState = new AttackingState (this);
		deadState = new DeadState (this);

		idleState.AddTransition ("StartChasing", chasingState);
		chasingState.AddTransition ("StartAttacking", attackingState);
		attackingState.AddTransition ("StartChasing", chasingState);

		stateSystem.AddState (idleState);
		stateSystem.AddState (chasingState);
		stateSystem.AddState (attackingState);
		stateSystem.AddState (deadState);
	}

	/// <summary>
	/// Clears all triggers from the animator.
	/// </summary>
	public void ClearTriggers() {
		animator.ResetTrigger ("walk");
		animator.ResetTrigger ("idle");
		animator.ResetTrigger ("attack");
		animator.ResetTrigger ("dead");
	}

	/// <summary>
	/// Whenever the enemy takes damage, it needs to check for death and for aggroing upon the player.
	/// </summary>
	public void OnDamageTaken(float current, float maximum, float difference) {
		if (dead) {
			return;
		}

		progressBar.Fill.fillAmount = current / maximum;

		if (Mathf.Approximately (current, 0)) {
			dead = true;

			ui.gameObject.SetActive (false);

			Destroy (gameObject, timeToDie);

			ClearTriggers ();
			navAgent.destination = transform.position;
			animator.enabled = false;
			stateSystem.ForcePerformTransition (deadState);

			for (int i = 0; i < rigidBodies.Count; i++) {
				rigidBodies [i].constraints = RigidbodyConstraints.None;
			}

			deathSound.PlayFrom (audioSource);

			if (onFireEffect.isPlaying) {
				onFireEffect.Stop ();
			}
		} 
		else {
			hitSound.PlayFrom (audioSource);
		}

		if (stateSystem.CurrentState is IdleState && !tookDamageFromFire && player != null) {
			target = player.transform;
		}

		if (tookDamageFromFire) {
			tookDamageFromFire = false;
		}
	}

	/// <summary>
	/// Resets the triggers, nav mesh and forces the idle state.
	/// </summary>
	public void Reset() {
		ClearTriggers ();
		navAgent.destination = transform.position;
		stateSystem.ForcePerformTransition (idleState);
	}

	/// <summary>
	/// When the player is detected, the enemy should start chasing them.
	/// </summary>
	public void PlayerDetected(Collider collider) {
		if (stateSystem.CurrentState is IdleState && player != null) {
			target = player.transform;
		}
	}
}