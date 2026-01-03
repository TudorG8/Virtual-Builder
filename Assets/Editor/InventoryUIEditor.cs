using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(InventoryUI))]
public class ObjectBuilderEditor : Editor {
	public override void OnInspectorGUI() {
		DrawDefaultInspector();

		InventoryUI myScript = (InventoryUI)target;
		if(GUILayout.Button("Update UI")) {
			myScript.UpdateUI ();
		}
	}
}