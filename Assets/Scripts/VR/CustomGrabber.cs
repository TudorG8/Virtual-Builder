using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using OVRTouchSample;

[System.Serializable]
public class OnGrabEvent : UnityEvent<OVRGrabbable> {}

/// <summary>
/// This is an override of DistanceGrabber, with a few difference to better fit this game.
/// Rather than inherit/override, this changes some things that are not marked as virtual in the original.
/// In exchange, some of this code is the same as the original.
/// </summary>
public class CustomGrabber : OVRGrabber {
	[Header("Settings")]
	[Tooltip("Radius of sphere used in spherecast from hand along forward ray to find target object.")]
	[SerializeField] float m_spherecastRadius;
	[Tooltip("Distance below which no-snap objects won't be teleported, but will instead be left where they are in relation to the hand.")]
	[SerializeField] float m_noSnapThreshhold = 0.05f; 
	[Tooltip("Whether to use the spherecast.")]
	[SerializeField] bool m_useSpherecast;
	[Tooltip("If true will prevent grabbing if there is anything in the way.")]
	[SerializeField] bool m_preventGrabThroughWalls;
	[Tooltip("The velocity at which grabbed objects will be moved")]
	[SerializeField] float m_objectPullVelocity = 10.0f;
	[Tooltip("Max rotation rate in degrees per second")]
	[SerializeField] float m_objectPullMaxRotationRate = 360.0f;
	[Tooltip("Objects can be distance grabbed up to this distance from the hand.")]
	[SerializeField] float m_maxGrabDistance;
	[Tooltip("Only allow grabbing objects in this layer.")]
	[SerializeField] int m_grabObjectsInLayer;
	[Tooltip("Dont grab objects in this layer.")]
	[SerializeField] int m_obstructionLayer;
	[Tooltip("Whether to draw a line in the game to the targets.")]
	[SerializeField] bool drawGameLines = false;

	[Header("Scene References")]
	[Tooltip("Reference to the player controller, which will be used to set the location of the hands")]
	[SerializeField] GameObject m_player;
	[Tooltip("Reference to the line drawer used to point at items")]
	[SerializeField] LineDrawer lineDrawer;

	[Header("Events")]
	[Tooltip("Triggered whenever the grab ends.")]
	[SerializeField] OnGrabEvent onGrabEnd;
	[Tooltip("Triggered whenever a grab starts.")]
	[SerializeField] OnGrabEvent onGrabBegin;

	[Header("Read Only")]
	[Tooltip("Will be true when the object is moving to catch up with the hand")]
	[SerializeField] bool m_movingObjectToHand = false;
	[Tooltip("The current grabbale")]
	[SerializeField] CustomGrabbable m_target;
	[Tooltip("Tracked separately from m_target, because we support child colliders of a DistanceGrabbable.")]
	[SerializeField] Collider m_targetCollider; 
	[Tooltip("Will be populated at runtime")]
	[SerializeField] CustomGrabber m_otherHand;

	public CustomGrabbable GrabbedObject {
		get { 
			return m_grabbedObj as CustomGrabbable;
		}
	}

	public CustomGrabbable M_target {
		get {
			return this.m_target;
		}
		set {
			m_target = value;
		}
	}

	public Collider M_targetCollider {
		get {
			return this.m_targetCollider;
		}
		set {
			m_targetCollider = value;
		}
	}

	public OnGrabEvent OnGrabEnd {
		get {
			return this.onGrabEnd;
		}
		set {
			onGrabEnd = value;
		}
	}

	public OnGrabEvent OnGrabBegin {
		get {
			return this.onGrabBegin;
		}
		set {
			onGrabBegin = value;
		}
	}

	public bool UseSpherecast {
		get { return m_useSpherecast; }
		set {
			m_useSpherecast = value;
			GrabVolumeEnable(!m_useSpherecast);
		}
	}

	public OVRInput.Controller Controller {
		get { return m_controller; }
	}

	/// <summary>
	/// Same as the original
	/// </summary>
	protected override void Start() {
		base.Start();
		// Set up our max grab distance to be based on the player's max grab distance.
		// Adding a liberal margin of error here, because users can move away some from the 
		// OVRPlayerController, and also players have arms.
		// Note that there's no major downside to making this value too high, as objects
		// outside the player's grabbable trigger volume will not be eligible targets regardless.
		SphereCollider sc = m_player.GetComponentInChildren<SphereCollider>();
		m_maxGrabDistance = sc.radius + 3.0f;

		if(m_parentHeldObject == true) 	{
			Debug.LogError("m_parentHeldObject incompatible with DistanceGrabber. Setting to false.");
			m_parentHeldObject = false;
		}

		CustomGrabber[] grabbers = FindObjectsOfType<CustomGrabber>();
		for (int i = 0; i < grabbers.Length; ++i) {
			if (grabbers[i] != this) m_otherHand = grabbers[i];
		}
		Debug.Assert(m_otherHand != null);
		//m_camera = GameObject.Find("OVRCameraRig").GetComponent<OVRCameraRig>();
	}

	/// <summary>
	/// Same as the original
	/// </summary>
	void Update() {
		CustomGrabbable target;
		Collider targetColl;
		FindTarget(out target, out targetColl);

		if (target != m_target) {
			if (m_target != null) {
				//m_target.Highlight = m_otherHand.m_target == m_target;
				m_target.Targeted = m_otherHand.m_target == m_target;
			}
			m_target = target;
			m_targetCollider = targetColl;
			if (m_target != null)
			{
				//m_target.Highlight = true;
				m_target.Targeted = true;
			}
		}

		if (target && drawGameLines) {
			lineDrawer.Start = transform;
			lineDrawer.Target = m_target.transform;
		} 
		else {
			lineDrawer.Start = null;
			lineDrawer.Target = null;
		}
	}



	/// <summary>
	/// Happens when a hand starts grabbing an object. The object has to be rotated and moved towards the hand, taking into account the snap offset.
	/// </summary>
	protected override void GrabBegin() {
		CustomGrabbable closestGrabbable = m_target;
		Collider closestGrabbableCollider = m_targetCollider;

		GrabVolumeEnable(false);

		if (closestGrabbable != null) {
			if (closestGrabbable.isGrabbed) {
				((CustomGrabber)closestGrabbable.grabbedBy).OffhandGrabbed(closestGrabbable);
			}

			m_grabbedObj = closestGrabbable;
			m_grabbedObj.GrabBegin(this, closestGrabbableCollider);

			m_movingObjectToHand = true;
			m_lastPos = transform.position;
			m_lastRot = transform.rotation;

			// If it's within a certain distance respect the no-snap.
			Vector3 closestPointOnBounds = closestGrabbableCollider.ClosestPointOnBounds(m_gripTransform.position);
			if(!m_grabbedObj.snapPosition && !m_grabbedObj.snapOrientation && m_noSnapThreshhold > 0.0f && (closestPointOnBounds - m_gripTransform.position).magnitude < m_noSnapThreshhold) {
				Vector3 relPos = m_grabbedObj.transform.position - transform.position;
				m_movingObjectToHand = false;
				relPos = Quaternion.Inverse(transform.rotation) * relPos;
				m_grabbedObjectPosOff = relPos;
				Quaternion relOri = Quaternion.Inverse(transform.rotation) * m_grabbedObj.transform.rotation;
				m_grabbedObjectRotOff = relOri;
			}
			else {
				// Set up offsets for grabbed object desired position relative to hand.
				m_grabbedObjectPosOff = new Vector3();
				if (closestGrabbable.HasSnapOffset()) {
					m_grabbedObjectPosOff = closestGrabbable.GetSnapOffsetPosition (m_gripTransform.localPosition, m_controller == OVRInput.Controller.LTouch);;
				}

				m_grabbedObjectRotOff = m_gripTransform.localRotation;
				if (closestGrabbable.HasSnapOffset()) {
					Transform rotationToUse = m_controller == OVRInput.Controller.LTouch ? closestGrabbable.snapOffsetOffhand : closestGrabbable.snapOffset;
					m_grabbedObjectRotOff = rotationToUse.rotation * m_grabbedObjectRotOff;
				}
			}

		}
	}

	/// <summary>
	/// Rotates a point around a pivot by angles.
	/// </summary>
	/// <returns>The rotated vector.</returns>
	public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles) {
		return Quaternion.Euler(angles) * (point - pivot) + pivot;
	}

	/// <summary>
	/// This is called to move the grabbed object every frame.
	/// </summary>
	protected override void MoveGrabbedObject(Vector3 pos, Quaternion rot, bool forceTeleport = false) {
		if (m_grabbedObj == null) {
			return;
		}

		// Parent due to items that can have the rigidbody instead of the grabbables
		Rigidbody  grabbedRigidbody  = m_grabbedObj.GetComponentInParent<Rigidbody> ();
		Vector3    grabbablePosition = pos + rot * m_grabbedObjectPosOff;
		Quaternion grabbableRotation = rot * m_grabbedObjectRotOff;

		if (m_movingObjectToHand){
			float travel = m_objectPullVelocity * Time.deltaTime;
			Vector3 dir = grabbablePosition - grabbedRigidbody.transform.position;
			if(travel * travel * 1.1f > dir.sqrMagnitude){
				m_movingObjectToHand = false;
			}
			else{
				dir.Normalize();
				grabbablePosition = grabbedRigidbody.transform.position + dir * travel;
				grabbableRotation = Quaternion.RotateTowards(grabbedRigidbody.transform.rotation, grabbableRotation, m_objectPullMaxRotationRate * Time.deltaTime);
			}
		}
		grabbedRigidbody.MovePosition(grabbablePosition);
		grabbedRigidbody.MoveRotation(grabbableRotation);
	}

	/// <summary>
	/// Same as original
	/// </summary>
	static private CustomGrabbable HitInfoToGrabbable(RaycastHit hitInfo){
		if (hitInfo.collider != null){
			GameObject go = hitInfo.collider.gameObject;
			return go.GetComponent<CustomGrabbable>() ?? go.GetComponentInParent<CustomGrabbable>();
		}
		return null;
	}

	/// <summary>
	/// Same as original
	/// </summary>
	protected bool FindTarget(out CustomGrabbable dgOut, out Collider collOut){
		dgOut = null;
		collOut = null;
		float closestMagSq = float.MaxValue;

		// First test for objects within the grab volume, if we're using those.
		// (Some usage of DistanceGrabber will not use grab volumes, and will only 
		// use spherecasts, and that's supported.)
		foreach (OVRGrabbable cg in m_grabCandidates.Keys){
			if (!(cg is CustomGrabbable))
				continue;

			CustomGrabbable grabbable = cg as CustomGrabbable;
			bool canGrab = grabbable != null && grabbable.InRange && !(grabbable.isGrabbed && !grabbable.allowOffhandGrab) && grabbable.CanBeGrabbed;
			if (!canGrab){
				continue;
			}

			for (int j = 0; j < grabbable.grabPoints.Length; ++j){
				Collider grabbableCollider = grabbable.grabPoints[j];
				// Store the closest grabbable
				Vector3 closestPointOnBounds = grabbableCollider.ClosestPointOnBounds(m_gripTransform.position);
				float grabbableMagSq = (m_gripTransform.position - closestPointOnBounds).sqrMagnitude;
				if (grabbableMagSq < closestMagSq){
					bool accept = true;
					if(m_preventGrabThroughWalls){
						// NOTE: if this raycast fails, ideally we'd try other rays near the edges of the object, especially for large objects.
						// NOTE 2: todo optimization: sort the objects before performing any raycasts.
						Ray ray = new Ray();
						ray.direction = grabbable.transform.position - m_gripTransform.position;
						ray.origin = m_gripTransform.position;
						RaycastHit obstructionHitInfo;
						Debug.DrawRay(ray.origin, ray.direction, Color.red, 0.1f);

						if (Physics.Raycast(ray, out obstructionHitInfo, m_maxGrabDistance, 1 << m_obstructionLayer))
						{
							float distToObject = (grabbableCollider.ClosestPointOnBounds(m_gripTransform.position) - m_gripTransform.position).magnitude;
							if(distToObject > obstructionHitInfo.distance * 1.1)
							{
								accept = false;
							}
						}
					}
					if(accept){
						closestMagSq = grabbableMagSq;
						dgOut = grabbable;
						collOut = grabbableCollider;
					}
				}
			}
		}

		if (dgOut == null && m_useSpherecast){
			return FindTargetWithSpherecast(out dgOut, out collOut);
		}
		return dgOut != null;
	}

	/// <summary>
	/// Same as original
	/// </summary>
	protected bool FindTargetWithSpherecast(out CustomGrabbable dgOut, out Collider collOut){
		dgOut = null;
		collOut = null;
		Ray ray = new Ray(m_gripTransform.position, m_gripTransform.forward);
		RaycastHit hitInfo;

		// If no objects in grab volume, raycast.
		// Potential optimization: 
		// In DistanceGrabbable.RefreshCrosshairs, we could move the object between collision layers.
		// If it's in range, it would move into the layer DistanceGrabber.m_grabObjectsInLayer,
		// and if out of range, into another layer so it's ignored by DistanceGrabber's SphereCast.
		// However, we're limiting the SphereCast by m_maxGrabDistance, so the optimization doesn't seem
		// essential.
		if (Physics.SphereCast(ray, m_spherecastRadius, out hitInfo, m_maxGrabDistance, 1 << m_grabObjectsInLayer)){
			CustomGrabbable grabbable = null;
			Collider hitCollider = null;
			if (hitInfo.collider != null){
				grabbable = hitInfo.collider.gameObject.GetComponentInParent<CustomGrabbable>();
				hitCollider = grabbable == null ? null : hitInfo.collider;
			}

			if (grabbable != null && m_preventGrabThroughWalls){
				// Found a valid hit. Now test to see if it's blocked by collision.
				RaycastHit obstructionHitInfo;
				ray.direction = hitInfo.point - m_gripTransform.position;

				dgOut = grabbable;
				collOut = hitCollider;
				if (Physics.Raycast(ray, out obstructionHitInfo, 1 << m_obstructionLayer)){
					CustomGrabbable obstruction = null;
					if(hitInfo.collider != null){
						obstruction = obstructionHitInfo.collider.gameObject.GetComponentInParent<CustomGrabbable>();
					}
					if (obstruction != grabbable && obstructionHitInfo.distance < hitInfo.distance){
						dgOut = null;
						collOut = null;
					}
				}
			}
		}
		return dgOut != null;
	}

	/// <summary>
	/// Same as original
	/// </summary>
	protected override void GrabVolumeEnable(bool enabled){
		if(m_useSpherecast) enabled = false;
		base.GrabVolumeEnable(enabled);
	}

	/// <summary>
	/// Same as original
	/// </summary>
	protected override void OffhandGrabbed(OVRGrabbable grabbable){
		base.OffhandGrabbed(grabbable);
	}

	/// <summary>
	/// Same as original
	/// </summary>
	protected override void GrabEnd() {
		OVRGrabbable releasedObj = m_grabbedObj;
		if (m_grabbedObj != null) {
			OVRPose localPose = new OVRPose { position = OVRInput.GetLocalControllerPosition(m_controller), orientation = OVRInput.GetLocalControllerRotation(m_controller) };
			OVRPose offsetPose = new OVRPose { position = m_anchorOffsetPosition, orientation = m_anchorOffsetRotation };
			localPose = localPose * offsetPose;

			OVRPose trackingSpace = transform.ToOVRPose() * localPose.Inverse();
			Vector3 linearVelocity = trackingSpace.orientation * OVRInput.GetLocalControllerVelocity(m_controller);
			Vector3 angularVelocity = trackingSpace.orientation * OVRInput.GetLocalControllerAngularVelocity(m_controller);

			GrabbableRelease(linearVelocity, angularVelocity);
		}

		// Re-enable grab volumes to allow overlap events
		GrabVolumeEnable(true);

		onGrabEnd.Invoke (releasedObj);
	}


	/// <summary>
	/// Specialized grab end, where it will only apply forces if the parameter is set.
	/// </summary>
	protected void GrabEnd(bool applyForces) {
		OVRGrabbable releasedObj = m_grabbedObj;
		if (m_grabbedObj != null) {
			OVRPose localPose = new OVRPose { position = OVRInput.GetLocalControllerPosition(m_controller), orientation = OVRInput.GetLocalControllerRotation(m_controller) };
			OVRPose offsetPose = new OVRPose { position = m_anchorOffsetPosition, orientation = m_anchorOffsetRotation };
			localPose = localPose * offsetPose;

			OVRPose trackingSpace = transform.ToOVRPose() * localPose.Inverse();
			Vector3 linearVelocity = trackingSpace.orientation * OVRInput.GetLocalControllerVelocity(m_controller);
			Vector3 angularVelocity = trackingSpace.orientation * OVRInput.GetLocalControllerAngularVelocity(m_controller);

			GrabbableRelease(linearVelocity, angularVelocity, applyForces);
		}

		// Re-enable grab volumes to allow overlap events
		GrabVolumeEnable(true);

		onGrabEnd.Invoke (releasedObj);
	}

	/// <summary>
	/// Specialized grabbable release, to take into account forces
	/// </summary>
	protected void GrabbableRelease(Vector3 linearVelocity, Vector3 angularVelocity, bool applyForces) {
		((CustomGrabbable)(m_grabbedObj)).GrabEnd(linearVelocity, angularVelocity, applyForces);
		if(m_parentHeldObject) m_grabbedObj.transform.parent = null;
		m_grabbedObj = null;
	}

	/// <summary>
	/// Forces the hand to release the grabbable.
	/// </summary>
	/// <param name="grabbable">Grabbable.</param>
	/// <param name="applyForces">If set to <c>true</c>, it will apply forces to the object.</param>
	public void ForceRelease(CustomGrabbable grabbable, bool applyForces) {
		bool canRelease = (
			(m_grabbedObj != null) &&
			(m_grabbedObj == grabbable)
		);
		if (canRelease) {
			GrabEnd (applyForces);
		}
	}

	/// <summary>
	/// Force the grab begin on the current target, if it was set manually
	/// </summary>
	public void ForceGrabBegin() {
		GrabBegin ();
	}
}