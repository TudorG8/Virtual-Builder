using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// An UI element that represents a fill/progress bar
/// </summary>
public class ProgressBarUI : MonoBehaviour {
	[Header("Object References")]
	[Tooltip("Reference to the fill image")]
	[SerializeField] Image fill;

	public Image Fill {
		get {
			return this.fill;
		}
		set {
			fill = value;
		}
	}
}
