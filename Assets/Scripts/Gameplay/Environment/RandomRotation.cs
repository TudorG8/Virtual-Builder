#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Allows attached game objects to have their rotation randomized on the press of a button.
/// </summary>
[ExecuteInEditMode]
public class RandomRotation : MonoBehaviour {
	/// <summary>
	/// Randomizes the y rotation between 0 and 359.
	/// </summary>
	public void Randomize() {
		transform.rotation = Quaternion.Euler (0, Random.Range(0, 360), 0);
	}
}
#endif