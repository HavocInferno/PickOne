using UnityEngine;
using System.Collections.Generic;

public class DungeonCamera : MonoBehaviour
{
	[Header("base cam options")]
	public GameObject target;
	public Vector3 offset;
	public Vector3 pivotOffsetIntended;
	private Vector3 pivotOffset;
	private float pivotHitDist;

	protected Transform tCamera;
	protected Transform tParent;

	protected Vector3 localRot;
	private Vector3 preUnlockLocalRot;
	public float distanceIntended = 10f;
	private float distance = 10f;

	public float mouseSense = 4f;
	public float scrollSense = 2f;
	public float orbitDampening = 10f;
	public float scrollDampeningIntended = 6f;
	public float scrollDampeningObstructed = 40f;
	private float scrollDampening = 6f;

	public Vector2 verticalRotationClamp = new Vector2(5f, 85f);
	public Vector2 scrollDistanceClamp = new Vector2(1.5f, 30f);

	public bool camDisabled = false;
	private bool lockToTarget = true;

	public LayerMask mask;

	[Space]
	[Header("Screenshake options")]
	//screenshake stuff
	public List<Screenshaker> shakeybakeys = new List<Screenshaker>();

	public float cumulativeShakeyStrength;
	public float maxCumStrength;
	private Vector3 originalPos;
	private bool shaking = false;
	public Transform shakeDistanceTarget;

	void Start()
    {
		pivotOffset = pivotOffsetIntended;
		pivotHitDist = pivotOffsetIntended.x;

		tCamera = transform;
		tParent = transform.parent;
		tParent.SetParent (target.transform);
		tParent.localPosition = Vector3.zero + pivotOffset;

		tCamera.localPosition = offset;

		scrollDampening = scrollDampeningIntended;
        distance = distanceIntended;

		originalPos = transform.localPosition;
		if (!shakeDistanceTarget)
			shakeDistanceTarget = transform;
	}

	void LateUpdate()
    {
		if (!target)
			return;

		//sticks the cam rig to the target. alternatively better: in Start(), parent target to the cam rig, set cam rig to pos 0,0,0
		//tParent.position = target.transform.position;

		//while Alt is pressed, allow free look around player without affecting player rotation
		/*if (Input.GetKeyDown (KeyCode.LeftAlt)) {
			lockToTarget = false;
			preUnlockLocalRot = localRot;
		}
		if (Input.GetKeyUp (KeyCode.LeftAlt)) {
			lockToTarget = true;
			tParent.rotation = target.transform.rotation;
			localRot = preUnlockLocalRot;
		}*/

		if (!camDisabled) {
			//Rot of cam based on mouse coords
			if (Input.GetAxis ("Mouse X") != 0 || Input.GetAxis ("Mouse Y") != 0) {
				localRot.x += Input.GetAxis ("Mouse X") * mouseSense;
				localRot.y -= Input.GetAxis ("Mouse Y") * mouseSense;

				//Clamp y rot to horizon and high noon
				localRot.y = Mathf.Clamp(localRot.y, verticalRotationClamp.x, verticalRotationClamp.y);

				if (lockToTarget) {
					//player rotates the same as cam on y axis
					float horizontal = Input.GetAxis ("Mouse X") * mouseSense;
					target.transform.Rotate (0, horizontal, 0);
				}
			}

			Vector3 raycastDir = transform.position - target.transform.position;
			RaycastHit hit;
			if (Physics.Raycast (
				    target.transform.position,
				    raycastDir,
				    out hit,
				    distanceIntended,
				    mask)) {
				distance = hit.distance;
				scrollDampening = scrollDampeningObstructed;
			} else {
				distance = distanceIntended;
				scrollDampening = scrollDampeningIntended;
			}

			raycastDir = tParent.position - target.transform.position;
			if (Physics.Raycast (
				target.transform.position,
				raycastDir,
				out hit,
				pivotHitDist,
				mask)) {
				pivotOffset = Vector3.zero;
			} else {
				pivotOffset = pivotOffsetIntended;
			}

			tParent.localPosition = Vector3.Lerp(tParent.localPosition, pivotOffset, scrollDampeningObstructed);

			//scrolling (zoom) based on scroll wheel
			/*if(Input.GetAxis("Mouse ScrollWheel") != 0f) {
				float scrollAmount = Input.GetAxis ("Mouse ScrollWheel") * scrollSense;

				//faster zoom at longer distance instead of linear speed
				scrollAmount *= (distanceIntended * 0.3f);

				distanceIntended += scrollAmount * -1f;

				distanceIntended = Mathf.Clamp (distanceIntended, scrollDistanceClamp.x, scrollDistanceClamp.y);
			}*/
		}

		//actual cam rig transformations
		Quaternion QT = Quaternion.Euler(localRot.y, localRot.x, 0f);
		tParent.rotation = Quaternion.Lerp (tParent.rotation, QT, Time.deltaTime * orbitDampening);

		//Debug.Log ("Cam lpos is off by " + Mathf.Abs (tCamera.localPosition.z - (camDist * -1f)));
		if (Mathf.Abs (tCamera.localPosition.z - (distance * -1f)) > 0.0001f) {
			tCamera.localPosition = new Vector3 (
											0f, 
											0f, 
											Mathf.Lerp (
													tCamera.localPosition.z, 
													distance * -1f, 
													Time.deltaTime * scrollDampening));
		}

		shakeScreen ();
	}

	void shakeScreen() {
		//calculate total shake strength
		cumulativeShakeyStrength = 0f;
		foreach (Screenshaker ss in shakeybakeys) {
			float dist = Vector3.Magnitude (shakeDistanceTarget.position - ss.origin.position);
			if (dist < ss.maxShakeDistance) {
				cumulativeShakeyStrength += ss.shakeyStrength * (1 - dist / ss.maxShakeDistance);
			}
		}
		cumulativeShakeyStrength = Mathf.Clamp (cumulativeShakeyStrength, 0f, maxCumStrength);

		//actual camera shake part
		if (cumulativeShakeyStrength > 0) {
			transform.localPosition += Random.insideUnitSphere * cumulativeShakeyStrength * Time.deltaTime;
		}
		transform.localPosition = Vector3.Lerp (transform.localPosition, originalPos, Time.deltaTime);
	}
}
