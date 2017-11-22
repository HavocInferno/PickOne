using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateSword : MonoBehaviour
{
    public float SwingSpeed = 2.0f;

    private void Update()
    {
        // Rotate the sword
        gameObject.transform.rotation =
            Quaternion.Lerp(
                gameObject.transform.rotation,
                Quaternion.Euler(-179f, -179f, 90f),
                SwingSpeed * Time.deltaTime);
    }
}
