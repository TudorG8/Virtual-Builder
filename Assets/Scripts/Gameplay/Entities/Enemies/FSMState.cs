using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class represents the States in the Finite State System.
/// Each state has a Dictionary with pairs (transition-state) showing
/// which state the FSM should be if a transition is fired while this state
/// is the current state.
/// Method Reason is used to determine which transition should be fired .
/// Method Act has the code to perform the actions the NPC is supposed do if it's on this state.
/// 
/// Taken from http://wiki.unity3d.com/index.php/Finite_State_Machine
/// Slightly modified to fit my preferences.
/// </summary>
public abstract class FSMState {
	protected Dictionary<string, FSMState> transitions = new Dictionary<string, FSMState>();
	protected string stateName;

	public Dictionary<string, FSMState> Transitions {
		get {
			return this.transitions;
		}
		set {
			transitions = value;
		}
	}

	public string StateName {
		get {
			return this.stateName;
		}
		set {
			stateName = value;
		}
	}

	/// <summary>
	/// Adds a transition to a new state.
	/// Since this is a Deterministic FSM, it checks if the current transition was already inside the map
	/// </summary>
	public void AddTransition(string transition, FSMState state) {
		if (transitions.ContainsKey(transition)){
			return;
		}

		transitions.Add(transition, state);
	}

	/// <summary>
	/// This method is used to set up the State condition before entering it.
	/// It is called automatically by the FSMSystem class before assigning it
	/// to the current state.
	/// </summary>
	public virtual void OnStateEnter () {}

	/// <summary>
	/// This method is used to make anything necessary, as reseting variables
	/// before the FSMSystem changes to another one. It is called automatically
	/// by the FSMSystem before changing to a new state.
	/// </summary>
	public virtual void OnStateExit () {}

	/// <summary>
	/// This method decides if the state should transition to another on its list
	/// NPC is a reference to the object that is controlled by this class
	/// </summary>
	public abstract void Reason();

	/// <summary>
	/// This method controls the behavior of the NPC in the game World.
	/// Every action, movement or communication the NPC does should be placed here
	/// NPC is a reference to the object that is controlled by this class
	/// </summary>
	public abstract void Act();

}