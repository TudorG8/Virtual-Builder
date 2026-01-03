using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Quick tool to create an object at a given location.
/// </summary>
public class CreateObject : MonoBehaviour {
	[Header("Prefabs")]
	[Tooltip("What object to spawn in")]
	[SerializeField] GameObject o;

	public void SpawnIt(GameObject location) {
		Instantiate (o, location.transform.position, o.transform.rotation);
	}
}
