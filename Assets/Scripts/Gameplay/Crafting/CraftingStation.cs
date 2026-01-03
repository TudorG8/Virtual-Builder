using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Used to combine materials into components and out put said components at a specific location.
/// Requires an input that will detect the entry and exit of potential materials.
/// </summary>
public class CraftingStation : MonoBehaviour {
	[Header("Scene Objects")]
	[Tooltip("The location where components will be output at")]
    [SerializeField] Transform outputLocation;
	[Tooltip("Reference to UI component")]
	[SerializeField] CraftingStationUI stationGUI;

	[Header("Scriptable Data")]
	[Tooltip("The recipes this station can output")]
	[SerializeField] List<CraftingRecipeData> recipes;

	[Header("Read Only")]
	[Tooltip("Currently selected recipe, which should be selectable through the UI")]
	[SerializeField][ReadOnly] CraftingRecipeData selectedCraftingRecipe;

	private Dictionary<string, HashSet<CraftingMaterial>> filledSpots;

	public List<CraftingRecipeData> Recipes {
		get {
			return this.recipes;
		}
		set {
			recipes = value;
		}
	}

	public CraftingRecipeData SelectedCraftingRecipe {
		get {
			return this.selectedCraftingRecipe;
		}
		set {
			selectedCraftingRecipe = value;
		}
	}

	public void SelectRecipe(CraftingRecipeData recipe) {
		selectedCraftingRecipe = recipe;
	}

	void Start() {
		Reset ();
	}

	void Reset() {
		filledSpots = new Dictionary<string, HashSet<CraftingMaterial>> ();
	}

	public void AddToDictionary(string key, CraftingMaterial material) {
		if (!filledSpots.ContainsKey (key)) {
			filledSpots [key] = new HashSet<CraftingMaterial>();
		} 

		filledSpots [key].Add(material);
	}

	public void RemoveFromDictionary(string key, CraftingMaterial material) {
		if (filledSpots.ContainsKey(key) && filledSpots [key].Count == 1) {
			filledSpots.Remove (key);
		} 
		else {
			filledSpots [key].Remove(material);
		}
	}

	/// <summary>
	/// Should be triggered whenever a new potential material is detected.
	/// </summary>
	public void MaterialAdded (Collider material) {
		CraftingMaterial materialScript = material.GetComponent<CraftingMaterial> ();

		if (materialScript) {
			AddToDictionary (materialScript.Type, materialScript);
		}
	}

	/// <summary>
	/// Should be triggered whenever an accepted material leaves the detection area
	/// </summary>
	public void MaterialRemoved (Collider material) {
		CraftingMaterial materialScript = material.GetComponent<CraftingMaterial> ();

		if (materialScript) {
			RemoveFromDictionary (materialScript.Type, materialScript);
		}
	}

	/// <summary>
	/// Used to assemble the currently held materials into a component.
	/// Will only use up exactly what it needs, without touching the  leftvoer components.
	/// </summary>
	void CraftComponent() {
		for (int i = 0; i < selectedCraftingRecipe.Materials.Count; i++) {
			CraftingRecipeData.Pair pair = selectedCraftingRecipe.Materials [i];

			HashSet<CraftingMaterial> leftoverMaterials = new HashSet<CraftingMaterial> ();
			int count = 0;
			foreach (CraftingMaterial material in filledSpots [pair.Type]) {
				if (count < pair.Amount) {
					Destroy (material.gameObject);
				} 
				else {
					leftoverMaterials.Add (material);
				}

				count++;
			}

			filledSpots [pair.Type] = leftoverMaterials;
		}

		Instantiate (selectedCraftingRecipe.ComponentResult, outputLocation.position, selectedCraftingRecipe.ComponentResult.transform.rotation);
	}

	void Update() {
		if (selectedCraftingRecipe == null) {
			return;
		}

		bool canCraft = true;

		stationGUI.UpdateCosts (filledSpots, selectedCraftingRecipe);

		for (int i = 0; i < selectedCraftingRecipe.Materials.Count; i++) {
			CraftingRecipeData.Pair pair = selectedCraftingRecipe.Materials [i];
			// Break if any recipe component is missing or its amount is not enough
			if (!filledSpots.ContainsKey(pair.Type) || filledSpots [pair.Type].Count < pair.Amount) {
				canCraft = false;
				break;
			}
		}

		if (canCraft) {
			CraftComponent ();
		}
	}
}
