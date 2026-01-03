using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Hacky solution that will get the middle point of a skinned mesh renderer.
/// This is done every update and goes through the whole vertices array, which can be slow on complex objects.
/// </summary>
public class SkinnedMeshRendererMiddle : MonoBehaviour {
	[Header("Object References")]
	[Tooltip("Reference to the skinned mesh renderer")]
	[SerializeField] SkinnedMeshRenderer skin;

	[Header("Prefabs")]
	[Tooltip("Select this if you want the material of the mesh to be replaced with this material at start time")]
	[SerializeField] Material material;

	[Header("Read Only")]
	[Tooltip("The middle position of all vertices")]
	[SerializeField] Vector3 position;


	void Awake() {
		skin = GetComponent<SkinnedMeshRenderer> ();
	}

	void Start() {
		if (material != null) {
			skin.material = material;
		}
	}

	public Vector3 Position {
		get {
			return this.position;
		}
		set {
			position = value;
		}
	}

	public Material Material {
		get {
			return this.material;
		}
		set {
			material = value;
		}
	}

	void Update() {
		if (skin == null) {
			return;
		}

		Mesh baked = new Mesh();
		skin.BakeMesh(baked);

		Vector3[] vertices = baked.vertices;
		position = new Vector3 ();
		for (int i = 0; i < vertices.Length; i++) {
			position += transform.TransformPoint(vertices [i]);
		}
		position /= vertices.Length;
	}

	/// <summary>
	/// Draws a red sphere at the middle of the mesh, in editor mode.
	/// </summary>
	void OnDrawGizmos () {
		Gizmos.color = Color.red;
		Gizmos.DrawSphere (position, 0.01f);
		Gizmos.color = Color.white;
	}
}
