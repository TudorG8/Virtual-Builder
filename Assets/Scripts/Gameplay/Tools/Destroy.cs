using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Quick tool to destroy on object.
/// </summary>
public class Destroy : MonoBehaviour {
	[Header("Settings")]
	[Tooltip("If true, it will destroy the object on its Start step.")]
	[SerializeField] bool destroyOnStart;
	[Tooltip("Delay before destroy happens whenever destroy is called.")]
	[SerializeField] float delay;

	void Start() {
		if (destroyOnStart) {
			DoIt ();
		}
	}

    public void DoIt() {
		Destroy(this.gameObject, delay);
    }
}
