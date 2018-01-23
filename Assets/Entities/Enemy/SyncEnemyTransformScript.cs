﻿
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;

public class SyncEnemyTransformScript : NetworkBehaviour
{
	Vector2 prevPosition;
	Vector2 currPosition;
	Vector2 lastPosition;
	int prevRotationY;
	int currRotationY;
	int lastRotationY;
	float lastRpc = 0.0f;

	private float updateInterval;
	public int updateRate = 9;
	private float updateRateF;
	public float distThreshold = 0.005f;

	private void Start()
	{
		updateRateF = 1f / (float)updateRate;

		prevPosition = new Vector2(transform.position.x, transform.position.z);
		currPosition = new Vector2(transform.position.x, transform.position.z);;
		lastPosition = new Vector2(transform.position.x, transform.position.z);;
	}

    void Update()
    {
        if (isServer)
        {
			prevPosition = currPosition;
			currPosition = new Vector2(transform.position.x, transform.position.z);
			prevRotationY = currRotationY;
			currRotationY = (int)(transform.rotation.eulerAngles.y*1000f);
			float dist = Vector2.Distance(prevPosition, currPosition) + Mathf.Abs(currRotationY - prevRotationY);
			// update the server with position/rotation
			updateInterval += Time.deltaTime;
			if (updateInterval > updateRateF && dist > distThreshold)
			{
				updateInterval = 0f;
				RpcSync(currPosition, currRotationY);
            }
        }
        else
        {
			Vector2 interpPosV2 = Vector2.Lerp(lastPosition, (1.5f * currPosition - 0.5f * prevPosition), (Time.time - lastRpc) / updateRateF);
			Vector3 interpPosV3 = new Vector3 (interpPosV2.x, transform.position.y, interpPosV2.y);
			transform.position = interpPosV3;

			float targetRot = Mathf.LerpAngle(((float)(prevRotationY)/1000f), ((float)(currRotationY)/1000f), 1.5f);
			transform.transform.eulerAngles = new Vector3(
				transform.transform.eulerAngles.x,
				Mathf.Lerp(((float)(lastRotationY)/1000f), targetRot, (Time.time - lastRpc) / updateRateF),
				transform.transform.eulerAngles.z);
        }
    }

    [ClientRpc]
    void RpcSync(Vector3 position, float rotation)
    {
        if (!isServer)
        {
			lastRpc = Time.time;
			lastPosition = new Vector2(transform.position.x, transform.position.z);;
			prevPosition = currPosition;
			currPosition = position;
			lastRotationY = (int)(transform.rotation.eulerAngles.y*1000f);
			prevRotationY = currRotationY;
			currRotationY = rotation;
        }
    }
}
