using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crawler : MonoBehaviour
{
    public CharacterController characterController;
    public float maxRotationSpeed = 10.0f;

    public void Move(Vector3 direction)
    {
        float magnitude = direction.magnitude;
        if (magnitude < 1e-06f)
            return;
        direction.Normalize();
        Vector3 projectedDirection = Vector3.ProjectOnPlane(direction, transform.up);
        projectedDirection.Normalize();
        Debug.Log(projectedDirection);
        projectedDirection *= magnitude;
        Debug.Log(transform.right);
        float angle = Vector3.Angle(projectedDirection, transform.right);
        if (Vector3.Angle(projectedDirection, transform.forward) > 90.0)
        {
            angle = -angle;
        }
        Debug.Log(angle);
        angle = Mathf.Sign(angle) * Mathf.Min(Mathf.Abs(angle), maxRotationSpeed * Time.deltaTime);
        transform.Rotate(transform.up, -angle);
        characterController.Move(projectedDirection);
    }
}
