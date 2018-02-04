using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseCameraTransparency : MonoBehaviour
{
    public List<Renderer> renderers = new List<Renderer>();

    public float minThreshold = 2.5f;
    public float maxThreshold = 3.5f;

    // TODO: it shouldn't be here.
    [HideInInspector()]
    public bool invisible = false;

	float alpha = 0.0f;

    private void Start()
    {
        // Who split prefabs.... I hate you
        Renderer r = gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
        if (r)
        {
            renderers.Add(r);
            r.material.SetInt("_ZWrite", 1);
        }
    }

    // Update is called once per frame
    void Update ()
    {
        float dist = Vector3.Distance(Camera.main.transform.position, transform.position);

		if (dist > maxThreshold && alpha > 0.999f)
			return;
		
	    alpha = (invisible ? 0.1f : 1.0f) * Mathf.Clamp((dist - minThreshold) / (maxThreshold - minThreshold), 0.0f, 1.0f);
        foreach (var r in renderers)
        {
            Color color_diffuse = r.material.GetColor("_Color");
            color_diffuse.a = alpha;
            r.material.SetColor("_Color", color_diffuse);
            
            r.material.SetFloat("_Metallic", alpha * 0.5f);
        }
	}
}
