using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Trigger events whenever the in range variabled is toggled off and on.
/// </summary>
public class WhenInRange : MonoBehaviour {
	[Header("Events")]
	[Tooltip("Triggered when this becomes in range of a manager.")]
	[SerializeField] UnityEvent onBecomeInRange;
	[Tooltip("Triggered when goes out of range of a manager.")]
	[SerializeField] UnityEvent onBecomeOutOfRange;

	[Header("Read Only")]
	[Tooltip("Will be true when this is in range.")]
	[SerializeField][ReadOnly] bool inRange;

	public bool InRange {
		set {
			if (inRange != value) {
				if (value) { onBecomeInRange   .Invoke ();} 
				else       { onBecomeOutOfRange.Invoke ();}
			}
			inRange = value;
		}
	}
}
