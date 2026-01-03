using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Allows the colour fade of a given object, assuming the colour fade happens on a list of renderers.
/// Will fade the colour of each renderer.
/// </summary>
public class ColourFade : MonoBehaviour {
	[Header("Object References")]
	[Tooltip("List of renderers on the object.")]
	[SerializeField] List<Renderer> renderers;

	[Header("Settings")]
	[Tooltip("Time to fade.")]
	[SerializeField] float current = 0f;

	public void StartFade(float duration) {
		StartCoroutine (Routine (duration));
	}

	IEnumerator Routine(float duration) {
		current = 0f;
		while (current < duration) {
			current += Time.deltaTime;
			yield return new WaitForEndOfFrame ();

			for (int i = 0; i < renderers.Count; i++) {
				for (int j = 0; j < renderers [i].materials.Length; j++) {
					Color color = renderers [i].materials [j].color;
					color.a = Mathf.Clamp01 ((duration - current) / duration);
					renderers [i].materials [j].color = color;
				}
			}
		}
	}
}
