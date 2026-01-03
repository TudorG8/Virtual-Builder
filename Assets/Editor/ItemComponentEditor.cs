using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ItemComponent))]
public class ItemComponentEditor : Editor {
	public override void OnInspectorGUI() {
		DrawDefaultInspector();

		ItemComponent myScript = (ItemComponent)target;
		if(GUILayout.Button("Duplicate")) {
			myScript.Duplicate ();
		}
	}
}
