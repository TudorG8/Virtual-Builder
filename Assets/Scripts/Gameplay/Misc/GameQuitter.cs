using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Commands for respawning, reloading and quiting the game.
/// </summary>
public class GameQuitter : Singleton<GameQuitter> {
	[Header("Object References")]
	[Tooltip("Reference to the player.")]
	[SerializeField] Player player;

	void Awake() {
		InitiateSingleton ();
	}

	public void Respawn() {
		player.Reset ();
		EnemySpawner.Instance.Reset ();
	}

	public void ReloadScene() {
		Scene scene = SceneManager.GetActiveScene ();
		SceneManager.LoadScene (scene.name);
	}

	public void QuitGame() {
		#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
		#else
		Application.Quit();
		#endif
	}
}
