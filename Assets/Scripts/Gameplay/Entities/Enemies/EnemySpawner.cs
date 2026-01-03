using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A game controller that spawns enemies over time at a given location.
/// </summary>
public class EnemySpawner : Singleton<EnemySpawner> {
	[Header("Scene References")]
	[Tooltip("A list of spawning locations where enemies can be places.")]
	[SerializeField] List<Transform> spawnerLocations;
	[Tooltip("Reference to the player.")]
	[SerializeField] Player player;
	[Tooltip("Where to parent enemies to.")]
	[SerializeField] Transform enemyFolder;

	[Header("Prefabs")]
	[Tooltip("Prefab of the enemy to spawn.")]
	[SerializeField] GameObject enemy;

	[Header("Settings")]
	[Tooltip("Maximum distance the player needs to be away from a spawner for an enemy to spawn.")]
	[SerializeField] float maxDistance = 8;
	[Tooltip("Maximum enemies that can be alive at a point.")]
	[SerializeField] int maxEnemies = 20;
	[Tooltip("Cooldown to spawn an enemy.")]
	[SerializeField] float cooldown = 5;
	[Tooltip("Layer that enemies are on.")]
	[SerializeField] LayerMask enemyMask;

	[Header("Read Only")]
	[Tooltip("This is true whenever it is night.")]
	[SerializeField] bool spawningPeriodActive = false;
	[Tooltip("This is true if the spawning cooldown is over")]
	[SerializeField] bool canSpawn = true;

	void Awake() {
		InitiateSingleton ();
	}

	void Update() {
		if (spawningPeriodActive && canSpawn && enemyFolder.childCount < maxEnemies) {
			StartCoroutine (SpawnRoutine ());
		}

		spawningPeriodActive = DayNightCycle.Instance.CurrentTime >= 0.5f;
	}

	/// <summary>
	/// Destroys all enemies.
	/// </summary>
	public void Reset() {
		foreach (Transform child in enemyFolder.transform) {
			Destroy (child.gameObject);
		}
		StopAllCoroutines ();
		canSpawn = true;
	}

	/// <summary>
	/// Spawns an enemy after a given delay.
	/// Will go through all spawners and check if they are valid.
	/// They are valid if they have no enemies on top of them and are far away from the player.
	/// </summary>
	/// <returns>The routine.</returns>
	IEnumerator SpawnRoutine() {
		canSpawn = false;

		yield return new WaitForSeconds (cooldown);

		canSpawn = true;

		List<Transform> validSpawnerLocations = new List<Transform>();
		for (int i = 0; i < spawnerLocations.Count; i++) {
			Transform spawnerLocation = spawnerLocations [i];

			if (Vector3.Distance (spawnerLocation.position, player.transform.position) > maxDistance) {
				Collider[] colliders = Physics.OverlapSphere (spawnerLocation.position, 1f, enemyMask);
				bool noEnemies = true;

				for (int j = 0; j < colliders.Length; j++) {
					if (colliders [j].tag == "Enemy") {
						noEnemies = false;
					}
				}

				if (noEnemies) {
					validSpawnerLocations.Add (spawnerLocation);
				}
			}
		}

		if (validSpawnerLocations.Count == 0) {
			yield break;
		}

		int spawnerIndex = Random.Range (0, validSpawnerLocations.Count);

		GameObject enemyObj = Instantiate (enemy, validSpawnerLocations[spawnerIndex].position, Quaternion.identity);
		enemyObj.transform.SetParent (enemyFolder);
		enemyObj.transform.rotation = Quaternion.Euler (0, Random.Range (0, 360), 0);

		Enemy enemyScript = enemyObj.GetComponent<Enemy> ();
		enemyScript.Player = player;
	}
}
