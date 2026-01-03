using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(Enemy))]
public class EnemyEditor : Editor {
	public override void OnInspectorGUI() {
		DrawDefaultInspector();

		Enemy myScript = (Enemy)target;
		if(GUILayout.Button("Grab rigidbodies")) {
			myScript.RigidBodies.Clear ();
			myScript.RigidBodies = myScript.gameObject.GetComponentsInChildren<Rigidbody> ().ToList ();
		}
		if(GUILayout.Button("Set all to FREEZE")) {
			for (int i = 0; i < myScript.RigidBodies.Count; i++) {
				myScript.RigidBodies [i].constraints = RigidbodyConstraints.FreezeAll;
			}
		}
	}
}