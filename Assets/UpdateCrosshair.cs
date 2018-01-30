using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateCrosshair : MonoBehaviour
{
    public Transform bulletSpawn;
    public LayerMask layerMask;

    protected Crawler _crawler;
    protected CUI_crosshair _crosshair;
    
    void Start ()
    {
        // TODO: Find alterantive
        _crosshair = Object.FindObjectOfType<CUI_crosshair>();
        _crawler = GetComponentInParent<Crawler>();
    }

    protected void Update()
    {
        if (_crawler.isLocalPlayer)
        {
            // Figure out where the gun is aiming
            var ray = new Ray(bulletSpawn.position, bulletSpawn.forward);

            RaycastHit rayHit;
            if (Physics.Raycast(ray, out rayHit, 100, layerMask))
            {
                // Rotate the gun to point to the... point
                var crosshairTransform = _crosshair.GetComponent<RectTransform>();
                crosshairTransform.position = Camera.main.WorldToScreenPoint(rayHit.point);
            }
        }
    }
}
