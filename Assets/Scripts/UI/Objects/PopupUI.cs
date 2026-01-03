using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// UI element that has a single text element.
/// </summary>
[System.Serializable]
public class PopupUI : MonoBehaviour {
	[Header("Object References")]
	[Tooltip("Referene to the text of this element")] 
	[SerializeField] TextMeshProUGUI titleText;

	public TextMeshProUGUI TitleText {
		get {
			return this.titleText;
		}
		set {
			titleText = value;
		}
	}
}
