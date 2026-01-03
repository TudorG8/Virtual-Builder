using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Scritable Data that contains a list of modifiers for components.
/// </summary>
public class ComponentModifierData : ScriptableObject {
	#if UNITY_EDITOR
	[MenuItem("Assets/Level Design/Create/Modifier List")]
	public static void CreateMyAsset() {
		ComponentModifierData asset = ScriptableObject.CreateInstance<ComponentModifierData>();

		AssetDatabase.CreateAsset(asset, "Assets/Data/Components/component_modifiers.asset");
		AssetDatabase.SaveAssets();

		EditorUtility.FocusProjectWindow();

		Selection.activeObject = asset;
	}
	#endif
	[Tooltip("List of modifiers that this component will affect the final item.")]
	[SerializeField] List<Modifier> modifiers;

	public List<Modifier> Modifiers {
		get {
			return this.modifiers;
		}
		set {
			modifiers = value;
		}
	}
}
