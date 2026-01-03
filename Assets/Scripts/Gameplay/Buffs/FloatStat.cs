using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Event called when the current value changes. 
/// It is called with the current, maximum values and the difference in value on this event.
/// </summary>
[System.Serializable]
public class FloatStatChangeEvent : UnityEvent<float, float, float> {}

/// <summary>
/// <para> A stat that has a current value and a maximum value. </para>
/// <para> Has a UnityEvent that is called whenever the current value is changed. </para>
/// </summary>
[System.Serializable]
public class FloatStat {
	[Header("Settings")]
	[Tooltip("Unique identifier of the stat.")]
	[SerializeField] ModifierName name;
	[Tooltip("Current value, which is the value that should actually be changed.")]
	[SerializeField] float current;
	[Tooltip("Maximum value, of which current cannot be more than.")]
	[SerializeField] float currentMaximum;
	[Tooltip("If there are modifiers, you can use this to add a base value to the final value.")]
	[SerializeField] float baseAmount;

	[Header("Events")]
	[Tooltip("Called when the value changes")]
	[SerializeField] FloatStatChangeEvent onChangeEvent;

	[Header("Read Only")]
	[Tooltip("A list of modifiers that are currently affecting this stat")]
	[SerializeField] List<Modifier> modifiers;

	[Header("Read Only")]
	[Tooltip("What method to use to calculate the final value of this stat.")]
	[SerializeField][ReadOnly] CalculationMethods.Method calculationMethod;

	public ModifierName Name {
		get {
			return this.name;
		}
		set {
			name = value;
		}
	}

	public float Current {
		get {
			return this.current;
		}
		set {
			float difference = current - value;

			current = Mathf.Clamp(value, 0, currentMaximum);
			onChangeEvent.Invoke (current, currentMaximum, difference);
		}
	}

	public float CurrentMaximum {
		get {
			return this.currentMaximum;
		}
		set {
			currentMaximum = value;
			current = value;
		}
	}

	public List<Modifier> Modifiers {
		get {
			return this.modifiers;
		}
		set {
			modifiers = value;
		}
	}

	public FloatStat() {
		modifiers = new List<Modifier> ();
		calculationMethod = CalculationMethods.SimpleMethod ();
		current = currentMaximum;
	}

	/// <summary>
	/// Should be called every time a stat changes.
	/// </summary>
	public void Recalculate() {
		currentMaximum = calculationMethod (baseAmount, modifiers);
		current = currentMaximum;
	}
}
