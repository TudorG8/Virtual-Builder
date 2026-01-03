using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Given a camera and a rawtexture, this create a rendertexture that is shared between the two.
/// </summary>
public class RuntimeRenderTextureLinker : MonoBehaviour {
	[Header("Object References")]
	[Tooltip("Reference to the camera.")]
	[SerializeField] Camera cameraToUse;
	[Tooltip("Reference to the raw image.")]
	[SerializeField] RawImage image;

	[Header("Settings")]
	[Tooltip("Size of the generated render texture.")]
	[SerializeField] int size = 512;

	[Header("Read Only")]
	[Tooltip("The generated render texture at runtime.")]
	[SerializeField] RenderTexture generatedTexture;

	void Awake() {
		generatedTexture = new RenderTexture (size, size, 16);
		cameraToUse.targetTexture = generatedTexture;
		image.texture = generatedTexture; 
	}
}
