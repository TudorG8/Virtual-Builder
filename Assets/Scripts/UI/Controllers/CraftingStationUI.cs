using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// UI Controller for stations, which handles the creation of elements that hold the recipes.
/// </summary>
public class CraftingStationUI : MonoBehaviour {
	[Header("Scene References")]
	[Tooltip("Reference to the main station script.")]
	[SerializeField] CraftingStation station;
	[Tooltip("Will hold the text of the selected recipe.")]
	[SerializeField] TextMeshProUGUI titleText;
	[Tooltip("Will hold the cost of the selected recipe.")]
	[SerializeField] TextMeshProUGUI costs;
	[Tooltip("Will be the icon of the selected recipe.")]
	[SerializeField] Image defaultImage;
	[Tooltip("Where all the station elements will be parented under.")]
	[SerializeField] Transform parent;
	[Tooltip("The button used to open the options menu.")]
	[SerializeField] Button openButton;
	[Tooltip("The button used to close the options menu.")]
	[SerializeField] Button closeButton;

	[Header("Prefabs")]
	[Tooltip("Prefab of the element that will hold each recipe")]
	[SerializeField] GameObject componentUI;

	[Header("Events")]
	[Tooltip("Will be called whenever a recipe is selected.")]
	[SerializeField] UnityEvent onRecipeSelected;

	void Start() {
		if (station.SelectedCraftingRecipe == null) {
			station.SelectedCraftingRecipe = station.Recipes [0];
		} 

		openButton.onClick.Invoke ();
		UpdateElement (station.SelectedCraftingRecipe);

		for (int i = 0; i < station.Recipes.Count; i++) {
			CraftingRecipeData recipe = station.Recipes [i];

			GameObject componentUIObj = Instantiate (componentUI, parent);
			CraftingStationElementUI guiElement = componentUIObj.GetComponent<CraftingStationElementUI> ();

			guiElement.Art.sprite = recipe.ItemData.DisplayImage;
			guiElement.TitleText.text = recipe.Name;
			guiElement.Recipe = recipe;
			guiElement.OnClick.AddListener (station.SelectRecipe);
			guiElement.OnClick.AddListener ((CraftingRecipeData arg0) => { 
				onRecipeSelected.Invoke(); 
				UpdateElement(arg0);
				closeButton.onClick.Invoke();
			});
		}
	}

	/// <summary>
	/// Updates the normal view element to a given recipe
	/// </summary>
	/// <param name="recipe">Selected Recipe</param>
	void UpdateElement(CraftingRecipeData recipe) {
		defaultImage.sprite = recipe.ItemData.DisplayImage;

		titleText.text = recipe.Name;
	}

	/// <summary>
	/// Updates the costs of the current item based on what is present in the inventory.
	/// </summary>
	/// <param name="filledSpots">Filled spots of the inventory.</param>
	/// <param name="recipe">Selected Recipe.</param>
	public void UpdateCosts(Dictionary<string, HashSet<CraftingMaterial>> filledSpots, CraftingRecipeData recipe) {
		string additional = "";
		for (int i = 0; i < recipe.Materials.Count; i++) {
			CraftingRecipeData.Pair material = recipe.Materials [i];
			int current = filledSpots.ContainsKey (material.Type) ? filledSpots [material.Type].Count : 0;
			additional += material.Type + " : " + current + "/" + material.Amount + "\n";
		}
		costs.text = additional;
	}
}
