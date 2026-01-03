using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A bow item base is special in the sense that attaching objects to its input will duplicate the components on the other side.
/// In addition, adding line drawers will connect them to their new copies.
/// If arrows are armed in the docking zone, they will be shootable by dragging them backwards.
/// </summary>
public class BowItemBase : ItemBase {
	[System.Serializable]
	public struct Pair {
		public ConnectorInput Key;
		public ConnectorInput Value;   
	}

	[System.Serializable]
	public class ConnectorDictionary : SerializableDictionary<ConnectorInput, ConnectorInput> {}

	[System.Serializable]
	public class DrawerDictionary : SerializableDictionary<LineDrawer, LineDrawer> {}

	[Header("Object References")]
	[Tooltip("The position where arrows will be armed.")]
	[SerializeField] Transform arrowPosition;
	[Tooltip("A list of colliders present on this component. Arrows will ignore those upon being shot.")]
	[SerializeField] List<Collider> colliders;
	[Tooltip("An initial list of pairings on the bow")]
	[SerializeField] List<Pair> pairingList;
	[Tooltip("The source that will play sound effects.")]
	[SerializeField] AudioSource audioSource;

	[Header("Prefabs")]
	[Tooltip("The clip that should be played when arrow is launched.")]
	[SerializeField] VolumeClip shootSound;

	[Header("Settings")] 
	[Tooltip("A cooldown for how quick new arrows can be grabbed after one is fired.")]
	[SerializeField] float arrowCooldown = 0.3f;
	[Tooltip("If an arrow is shot below this distance it wont be fired.")]
	[SerializeField] float minimumShootDistance = 0.05f;

	[Header("Read Only")] 
	[Tooltip("The middle of all drawer pairings, generated at runtime.")]
	[SerializeField] Vector3 middle;
	[Tooltip("A list of current pairings, based on what new components are added to the item")]
	[SerializeField] ConnectorDictionary pairings;
	[Tooltip("A list of line drawer pairings, which will be connected to each other, or the current arrow.")]
	[SerializeField] DrawerDictionary drawerPairings;
	[Tooltip("Will be false when an arrow is armed")]
	[SerializeField] bool canGrabArrows = true;
	[Tooltip("Whenever any arrow is loaded, it will be here")] 
	[SerializeField] Arrow currentArrow;

	void Awake() {
		if (pairings.Count == 0) {
			for (int i = 0; i < pairingList.Count; i++) {
				pairings.Add (pairingList [i].Key, pairingList [i].Value);
			}
		}
	}

	void Start() {
		UpdateStats ();
	}
		
	/// <summary>
	/// Make sure to update the location of the arrow
	/// </summary>
	void LateUpdate() {
		if (currentArrow != null ) {
			Vector3 position = currentArrow.transform.localPosition;
			float z = Mathf.Clamp (position.z, -currentArrow.Size, middle.z);
			position = new Vector3 (0, 0, z);

			currentArrow.transform.localPosition = position;
			currentArrow.transform.forward = arrowPosition.forward;
		}
	}

	/// <summary>
	/// Create the duplicate on the other side of the bow and check for line drawers.
	/// It will go recursivily through each "layer" of components,
	/// duplicate them and add anything connected to their inputs for the next layer.
	/// </summary>
	public override void OnComponentConnection(ConnectorInformation connectors) {
		List<ConnectorInformation> currentList = new List<ConnectorInformation> ();
		currentList.Add (connectors);

		while (currentList.Count > 0) {
			List<ConnectorInformation> newList = new List<ConnectorInformation> ();

			for (int i = 0; i < currentList.Count; i++) {
				ConnectorInformation info = currentList [i];

				MakeCopyOfGameObjectAndConnectToInput (info);

				for (int j = 0; j < info.Output.Component.Inputs.Count; j++) {
					ConnectorInput input = info.Output.Component.Inputs [j];

					if (input.Connected) {
						ConnectorInformation newInfo = new ConnectorInformation (input, input.ConnectedTo);
						newList.Add (newInfo);
					}
				}
			}

			currentList = newList;
		}
	}

	/// <summary>
	/// Makes the copy of an input component and places it connected to its adjacenet pair.
	/// </summary>
	/// <param name="connectors">Connectors.</param>
	void MakeCopyOfGameObjectAndConnectToInput(ConnectorInformation connectors) {
		GameObject otherPart = Instantiate (connectors.Output.Component.gameObject, new Vector3 (), Quaternion.identity);
		otherPart.transform.SetParent (itemComponent.Item.transform);
		otherPart.transform.position = pairings [connectors.Input].transform.position;
		ItemComponent newComponent = otherPart.GetComponent<ItemComponent> ();

		Transform targetTransform    = pairings [connectors.Input].transform;
		Transform targetParent       = pairings [connectors.Input].Component.transform;
		Transform componentTransform = newComponent.Outputs [0].transform;
		Transform componentParent    = newComponent.transform;

		ConnectorOutput.OrientateNewPiece (componentTransform, componentParent, targetTransform);

		pairings.Remove (connectors.Input);

		newComponent.Disable ();

		itemComponent.Item.Components.Add (newComponent);

		for (int i = 0; i < connectors.Output.Component.Inputs.Count; i++) {
			ConnectorInput newInput        = connectors.Output.Component.Inputs[i];
			ConnectorInput newInputPairing = newComponent.Inputs [connectors.Output.Component.Inputs.Count - 1 - i];

			pairings.Add (newInput, newInputPairing);
		}


		LineDrawer lineDrawer = connectors.Output.Component.GetComponent<LineDrawer> ();
		LineDrawer otherLineDrawer = otherPart.GetComponent<LineDrawer> ();

		if (lineDrawer != null && otherLineDrawer != null) {
			lineDrawer.Target = otherLineDrawer.Start;
			otherLineDrawer.Target = lineDrawer.Start;

			drawerPairings.Add (lineDrawer, otherLineDrawer);

			middle = new Vector3 ();
			foreach (KeyValuePair<LineDrawer, LineDrawer> pair in drawerPairings) {
				middle += (pair.Key.Start.transform.position + pair.Value.Start.transform.position) / 2;
			}
			middle /= drawerPairings.Count;
			middle = arrowPosition.InverseTransformPoint (middle);
		}

		UpdateStats ();
	}

	/// <summary>
	/// Whenever a potential arrow is loaded.
	/// </summary>
	public void OnArrowCollision(Collider other) {
		if (currentArrow != null || !canGrabArrows || other.attachedRigidbody == null) {
			return;
		}

		Arrow arrow = other.attachedRigidbody.GetComponent<Arrow> ();

		if (arrow) {
			foreach (KeyValuePair<LineDrawer, LineDrawer> pair in drawerPairings) {
				pair.Key.Target = arrow.transform;
				pair.Value.Target = arrow.transform;
			}

			currentArrow = arrow;

			Rigidbody rigidBody = currentArrow.GetComponent<Rigidbody> ();
			rigidBody.isKinematic = true;
			rigidBody.useGravity = false;

			currentArrow.transform.SetParent (arrowPosition, true);
			currentArrow.transform.localPosition = middle;

			arrow.Bow = this;

			canGrabArrows = false;
		}
	}

	/// <summary>
	/// Makes the current colliders ignore collisions with the current arrow.
	/// </summary>
	void UpdateArrowIgnoreCollisions() {
		if (currentArrow == null) {
			return;
		}

		for (int i = 0; i < colliders.Count; i++) {
			Physics.IgnoreCollision (colliders[i], currentArrow.Collider);
		}
	}

	/// <summary>
	/// Makes it so that you can only grab arrows after a small cooldown.
	/// </summary>
	/// <returns>The routine.</returns>
	IEnumerator WaitRoutine() {
		yield return new WaitForSeconds (arrowCooldown);
		canGrabArrows = true;
	}

	/// <summary>
	/// Whenever the arrow is ungrabbed, it needs to be either shot or reset to its initial position.
	/// </summary>
	public void OnArrowUngrabbed() {
		if (currentArrow == null) {
			return;
		}

		float arrowDistance = Vector3.Distance (currentArrow.transform.position, arrowPosition.transform.position);
		float minDistance = middle.magnitude;
	
		if (arrowDistance < minDistance + minimumShootDistance) {
			currentArrow.transform.localPosition = middle;
			currentArrow.RigidBody.isKinematic = true;
			return;
		}

		foreach (KeyValuePair<LineDrawer, LineDrawer> pair in drawerPairings) {
			pair.Key.Target = pair.Value.Start;
			pair.Value.Target = pair.Key.Start;
		}

		Rigidbody rigidBody = currentArrow.GetComponent<Rigidbody> ();
		rigidBody.isKinematic = false;
		rigidBody.useGravity = true;
		rigidBody.constraints = RigidbodyConstraints.FreezeRotation; 

		OffensiveEntity offensiveEntity = currentArrow.GetComponent<OffensiveEntity> ();
		offensiveEntity.GetStat(ModifierName.Damage).CurrentMaximum = stats.Find(stat => stat.Name == ModifierName.Damage).Current;

		currentArrow.transform.parent = null;

		rigidBody.AddForce  (currentArrow.transform.forward * arrowDistance * stats.Find(stat => stat.Name == ModifierName.Velocity).Current);

		currentArrow.HasBeenShot = true;

		currentArrow.OnShoot ();

		currentArrow.Bow = null;

		currentArrow = null;

		shootSound.PlayFrom (audioSource);

		StartCoroutine (WaitRoutine ());
	}
}
