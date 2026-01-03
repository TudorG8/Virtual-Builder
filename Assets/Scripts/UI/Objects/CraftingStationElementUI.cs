using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

/// <summary>
/// An UI element used to represent a crafting recipe. Will be created at runtime by the <see cref="CraftignStation.cs"/>
/// </summary>
public class CraftingStationElementUI : MonoBehaviour {
	[System.Serializable]
	public class ClickEvent : UnityEvent<CraftingRecipeData> {}

	[Header("Object References")]
	[Tooltip("Referene to the title of this element")] 
	[SerializeField] TextMeshProUGUI titleText;
	[Tooltip("Referene to the image of this element")] 
	[SerializeField] Image art;

	[Header("Read Only")]
	[Tooltip("The crafting recipe used for this element")]
	[SerializeField] CraftingRecipeData recipe;
	[Tooltip("Will be called whenever this element is clicked")]
	[SerializeField] ClickEvent onClick;

	public TextMeshProUGUI TitleText {
		get {
			return this.titleText;
		}
		set {
			titleText = value;
		}
	}

	public Image Art {
		get {
			return this.art;
		}
		set {
			art = value;
		}
	}

	public CraftingRecipeData Recipe {
		get {
			return this.recipe;
		}
		set {
			recipe = value;

		}
	}

	public ClickEvent OnClick {
		get {
			return this.onClick;
		}
		set {
			onClick = value;
		}
	}

	public void OnClickEvent() {
		onClick.Invoke (recipe);
	}
}
