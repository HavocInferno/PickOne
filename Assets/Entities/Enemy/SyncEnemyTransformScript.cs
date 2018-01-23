
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;

public class SyncEnemyTransformScript : NetworkBehaviour
{
    Vector3 prevPosition;
    Vector3 currPosition;
    Vector3 lastPosition;
    float prevRotation;
    float currRotation;
    float lastRotation;
    float lastRpc = 0.0f;

    private float updateInterval;

    private void Start()
    {
        prevPosition = transform.position;
        currPosition = transform.position;
        lastPosition = transform.position;
    }

    void Update()
    {
        if (isServer)
        {
            prevPosition = currPosition;
            currPosition = transform.position;
            prevRotation = currRotation;
            currRotation = transform.rotation.eulerAngles.y;
            float dist = Vector3.Distance(prevPosition, currPosition) + Mathf.Abs(currRotation - prevRotation);
            // update the server with position/rotation
            updateInterval += Time.deltaTime;
            if (updateInterval > 0.11f && dist > 0.001f)
            {
                updateInterval = 0;
                RpcSync(transform.position, transform.rotation.eulerAngles.y);
            }
        }
        else
        {
            transform.position = Vector3.Lerp(lastPosition, (1.5f * currPosition - 0.5f * prevPosition), (Time.time - lastRpc) / 0.11f);
            float targetRot = Mathf.LerpAngle(prevRotation, currRotation, 1.5f);
            transform.transform.eulerAngles = new Vector3(
                transform.transform.eulerAngles.x,
                Mathf.Lerp(lastRotation, targetRot, (Time.time - lastRpc) / 0.11f),
                transform.transform.eulerAngles.z);
        }
    }

    [ClientRpc]
    void RpcSync(Vector3 position, float rotation)
    {
        if (!isServer)
        {
            lastRpc = Time.time;
            lastPosition = transform.position;
            prevPosition = currPosition;
            currPosition = position;
            lastRotation = transform.rotation.eulerAngles.y;
            prevRotation = currRotation;
            currRotation = rotation;
        }
    }
}
