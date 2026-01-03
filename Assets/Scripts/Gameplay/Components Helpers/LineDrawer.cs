using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Draws a line between "start" and "target".
/// Only drawn when both references are not null.
/// </summary>
[ExecuteInEditMode]
public class LineDrawer : MonoBehaviour {
	[Header("Object References")]
	[Tooltip("Reference to the LineRenderer component")]
	[SerializeField] LineRenderer lineRenderer;
	[Tooltip("Reference to the starting point of the line.")]
	[SerializeField] Transform start;
	[Tooltip("Reference to the ending point of the line.")]
	[SerializeField] Transform target;

	public Transform Start {
		get {
			return this.start;
		}
		set {
			start = value;
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

	void LateUpdate() {
		if (target != null && start != null) {
			lineRenderer.positionCount = 2;
			lineRenderer.SetPosition (0, start.transform.position);
			lineRenderer.SetPosition (1, target.transform.position);
		}
		else {
			lineRenderer.positionCount = 0;
		}
	}
}
