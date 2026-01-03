using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// FSMSystem class represents the Finite State Machine class.
///  It has a List with the States the NPC has and methods to add,
///  delete a state, and to change the current state the Machine is on.
/// 
/// Taken from http://wiki.unity3d.com/index.php/Finite_State_Machine
/// Slightly modified to fit my preferences.
/// </summary>
public class FSMSystem : MonoBehaviour {
	[Header("Read Only")]
	[Tooltip("A list of all states this system has.")]
	[SerializeField] List<FSMState> states;
	[Tooltip("The current active state of the system.")]
	[SerializeField] FSMState currentState;

	public FSMState CurrentState {
		get {
			return this.currentState;
		}
		set {
			currentState = value;
		}
	}

	public FSMSystem() {
		states = new List<FSMState>();
	}

	/// <summary>
	/// This method places new states inside the FSM,
	/// First state added is also the initial state.
	/// </summary>
	public void AddState(FSMState state) {
		if (state == null) {
			return;
		}

		for (int i = 0; i < states.Count; i++) {
			if (states [i] == state) {
				return;
			}
		}

		states.Add (state);

		if (states.Count == 1) {
			currentState = state;
		}
	}

	public void ForcePerformTransition(FSMState toState) {
		currentState.OnStateExit ();
		currentState = toState;
		currentState.OnStateEnter ();
	}

	/// <summary>
	/// This method tries to change the state the FSM is in based on
	/// the current state and the transition passed.
	/// </summary>
	public void PerformTransition(string transition) {
		if (currentState.Transitions.ContainsKey (transition)) {
			currentState.OnStateExit ();
			currentState = currentState.Transitions [transition];
			currentState.OnStateEnter ();
		}
	}

	void Update() {
		if (currentState != null) {
			currentState.Reason ();
			currentState.Act ();
		}
	}
}