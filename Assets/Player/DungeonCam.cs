using UnityEngine;

public class DungeonCam : MonoBehaviour
{
	public GameObject target;
	public Vector3 offset;

	protected Transform tCamera;
	protected Transform tParent;

	protected Vector3 localRot;
	private Vector3 preUnlockLocalRot;
	protected float camDist = 10f;

	public float mouseSense = 4f;
	public float scrollSense = 2f;
	public float orbitDampening = 10f;
	public float scrollDampening = 6f;

	public Vector2 verticalRotationClamp = new Vector2(5f, 85f);
	public Vector2 scrollDistanceClamp = new Vector2(1.5f, 30f);

	public bool camDisabled = false;
	private bool lockToTarget = true;


	void Start() {
		tCamera = transform;
		tParent = transform.parent;

		tCamera.localPosition = offset;
	}

	void LateUpdate() {
		if (!target)
			return;

		//sticks the cam rig to the target. alternatively better: in Start(), parent target to the cam rig, set cam rig to pos 0,0,0
		tParent.position = target.transform.position;

		//while Alt is pressed, allow free look around player without affecting player rotation
		if (Input.GetKeyDown (KeyCode.LeftAlt)) {
			lockToTarget = false;
			preUnlockLocalRot = localRot;
		}
		if (Input.GetKeyUp (KeyCode.LeftAlt)) {
			lockToTarget = true;
			tParent.rotation = target.transform.rotation;
			localRot = preUnlockLocalRot;
		}

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

			//scrolling (zoom) based on scroll wheel
			if(Input.GetAxis("Mouse ScrollWheel") != 0f) {
				float scrollAmount = Input.GetAxis ("Mouse Scrollwheel") * scrollSense;

				//faster zoom at longer distance instead of linear speed
				scrollAmount *= (camDist * 0.3f);

				camDist += scrollAmount * -1f;

				camDist = Mathf.Clamp (camDist, scrollDistanceClamp.x, scrollDistanceClamp.y);
			}
		}

		//actual cam rig transformations
		Quaternion QT = Quaternion.Euler(localRot.y, localRot.x, 0f);
		tParent.rotation = Quaternion.Lerp (tParent.rotation, QT, Time.deltaTime * orbitDampening);

		if (Mathf.Abs (tCamera.localPosition.z - (camDist * -1f)) < 0.0001f) {
			tCamera.localPosition = new Vector3 (
											0f, 
											0f, 
											Mathf.Lerp (
													tCamera.localPosition.z, 
													camDist * -1f, 
													Time.deltaTime * scrollDampening));
		}
	}
}
