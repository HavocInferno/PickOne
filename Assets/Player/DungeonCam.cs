using UnityEngine;

public class DungeonCam : MonoBehaviour
{
	public enum CamType {
		MouseAim,
		FixedRotation,
		FixedLocalOffset
	}
	public CamType cameraType;
    public GameObject target;
    public float damping = 1;
	public Vector3 offset;
	public float rotateSpeed = 5;

	private Vector3 desiredPosition;
	private Vector3 position;

	/*void Start() {
		offset = target.transform.position - transform.position;
	}*/

    void LateUpdate()
    {
        if (!target)
            return;
        switch (cameraType) {
		case CamType.MouseAim:
			float horizontal = Input.GetAxis ("Mouse X") * rotateSpeed;
            target.transform.Rotate(0, horizontal, 0);
            
			float desiredAngle = target.transform.eulerAngles.y;
			Quaternion rotation = Quaternion.Euler(0, desiredAngle, 0);
			transform.position = target.transform.position - (rotation * offset);

			transform.LookAt(target.transform);
			break;
		case CamType.FixedRotation:
			desiredPosition = target.transform.position + offset;
			position = Vector3.Lerp (transform.position, desiredPosition, Time.deltaTime * damping);
			position.x = desiredPosition.x;
			if (desiredPosition.z - position.z < -1f)
				position.z = desiredPosition.z + 1f;
			transform.position = position;

			transform.LookAt (target.transform.position);
			break;
		case CamType.FixedLocalOffset:
			desiredPosition = target.transform.position + target.transform.forward*offset.x + target.transform.up*offset.y + target.transform.right*offset.z;
			position = Vector3.Lerp (transform.position, desiredPosition, Time.deltaTime * damping);
			position.x = desiredPosition.x;
			/*if (desiredPosition.z - position.z < -1f)
					position.z = desiredPosition.z + 1f;*/
			transform.position = position;

			transform.LookAt (target.transform.position);
			break;
		default:
			return;
		} 
    }
}
