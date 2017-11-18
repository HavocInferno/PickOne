using UnityEngine;

public class DungeonCam : MonoBehaviour
{
    public GameObject target;
    public float damping = 1;
    public Vector3 offset;

	public bool fixedRelativeToPlayer = false;

    void LateUpdate()
    {
        if (target != null)
        {
			if (fixedRelativeToPlayer) {
				Vector3 desiredPosition = target.transform.position + target.transform.forward*offset.x + target.transform.up*offset.y + target.transform.right*offset.z;
				Vector3 position = Vector3.Lerp (transform.position, desiredPosition, Time.deltaTime * damping);
				position.x = desiredPosition.x;
				/*if (desiredPosition.z - position.z < -1f)
					position.z = desiredPosition.z + 1f;*/
				transform.position = position;

				transform.LookAt (target.transform.position);
			} else {
				Vector3 desiredPosition = target.transform.position + offset;
				Vector3 position = Vector3.Lerp (transform.position, desiredPosition, Time.deltaTime * damping);
				position.x = desiredPosition.x;
				if (desiredPosition.z - position.z < -1f)
					position.z = desiredPosition.z + 1f;
				transform.position = position;

				transform.LookAt (target.transform.position);
			}
        } 
    }
}
