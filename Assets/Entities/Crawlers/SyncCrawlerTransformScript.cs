
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;

public class SyncCrawlerTransformScript : NetworkBehaviour
{
    Vector3 prevPosition;
    Vector3 currPosition;
    Vector3 lastPosition;
    Quaternion prevRotation;
    Quaternion currRotation;
    Quaternion lastRotation;
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
        if (isLocalPlayer)
        {
            // update the server with position/rotation
            updateInterval += Time.deltaTime;
            if (updateInterval > 0.1f)
            {
                updateInterval = 0;
                CmdSync(transform.position, transform.rotation);
            }
        }
        else
        {
            transform.position = Vector3.Lerp(lastPosition, (1.5f * currPosition - 0.5f * prevPosition), (Time.time - lastRpc) / 0.11f);
            Quaternion targetRot = Quaternion.SlerpUnclamped(prevRotation, currRotation, 1.5f);
            transform.rotation = Quaternion.Slerp(lastRotation, targetRot, (Time.time - lastRpc) / 0.11f);
        }
    }

    [Command]
    void CmdSync(Vector3 position, Quaternion rotation)
    {
        RpcSync(position, rotation);
    }

    [ClientRpc]
    void RpcSync(Vector3 position, Quaternion rotation)
    {
        lastRpc = Time.time;
        lastPosition = transform.position;
        prevPosition = currPosition;
        currPosition = position;
        lastRotation = transform.rotation;
        prevRotation = currRotation;
        currRotation = rotation;
    }
}

