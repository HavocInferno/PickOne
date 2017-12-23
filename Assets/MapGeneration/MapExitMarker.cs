using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapExitMarker : MonoBehaviour
{
    [Tooltip("This will be enabled/disabled if the exit is not used")]
    public GameObject wallPrefab;

    private void OnValidate()
    {
        if (wallPrefab == null)
        {
            Debug.LogErrorFormat(this, "{0} | Wall prefab is missing.", this.name);
        }
    }
}
