using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableAbility : MonoBehaviour {


    public float chargeMulti =1f;
    public float minVel=1, maxVel =10;
    public float minDistance= 1, maxDistance=10;
    public float explosionForce = 10;
    public AbstractEffect effect;
    public GameObject explosionPrefab;
    public float dieTime = 20;
	public AudioSource sound;  

    private Rigidbody rb;
    private PlayersManager playerManager;
	bool ded;

    // Use this for initialization
    void Start () {
        playerManager = FindObjectOfType<PlayersManager>();
        rb = GetComponent<Rigidbody>();
        StartCoroutine(die());
        foreach (Light lit in GetComponentsInChildren<Light>())
            lit.range = maxDistance*chargeMulti;
		if (sound!=null) {
			sound.volume = chargeMulti;
			sound.pitch = (float)(0.5+0.5*(1- chargeMulti));
		}
    }
	
	// Update is called once per frame
	void Update () {
		if (ded) {
			sound.volume = Mathf.Lerp(sound.volume,0,Time.deltaTime*3);
			sound.pitch = Mathf.Lerp(sound.pitch,0,Time.deltaTime*3);
		}
	}
    public void OnTriggerEnter(Collider other)
    {
        float velMulti, distanceMulti;
        float totalMulti;
        velMulti = getVelMulti();
        if (velMulti != 0)
        {
            for (int i = 0; i < playerManager.players.Count; i++)
            {
                if (playerManager.players[i] == null)
                    continue;
                Transform crawler = playerManager.players[i];
                distanceMulti = getDistMulti(crawler);

                if (distanceMulti == 0)
                    continue;

                totalMulti = getTotalMulti(velMulti, distanceMulti);
                Crawler cr = crawler.GetComponent<Crawler>();
                if (cr != null)
                     applyEffect(totalMulti, cr);
            }
            for (int i = 0; i < playerManager.enemies.Count; i++)
            {
                if (playerManager.enemies[i] == null)
                    continue;
                Transform enemy = playerManager.enemies[i];
                distanceMulti = getDistMulti(enemy);

                if (distanceMulti == 0)
                    continue;
                totalMulti = getTotalMulti(velMulti, distanceMulti);
                
                if (enemy.GetComponent<Enemy>())
                    applyEffect(totalMulti, enemy.GetComponent<Enemy>());
            }
        }
		ded = true;
        kill(velMulti);
    }

    public float getTotalMulti(float velMulti, float distanceMulti)
    {
        return chargeMulti * velMulti * distanceMulti;
    }

    public float getVelMulti()
    {
        return Mathf.Clamp((rb.velocity.magnitude - minVel) / (maxVel - minVel), 0, 1);
    }

    public float getDistMulti(Transform other)
    {
        return 1-Mathf.Clamp((((transform.position - other.position).magnitude) - minDistance) / (maxDistance - minDistance), 0, 1);
    }

    public virtual void applyEffect(float multi, GenericCharacter target)
    {
        //TODO: fill
        Debug.Log("Applying affect: "+ multi);
    }

    IEnumerator die()
    {
        yield return new WaitForSeconds(dieTime);
        kill(1);
    }
    void kill(float velMulti)
    {
        foreach (MeshRenderer rend in GetComponentsInChildren<MeshRenderer>())
            rend.enabled = false;
        foreach (Collider col in GetComponentsInChildren<Collider>())
            col.enabled = false;
        foreach (ParticleSystem part in GetComponentsInChildren<ParticleSystem>())
            part.enableEmission = false;
        foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
            rb.isKinematic = true;
        foreach (Light lit in GetComponentsInChildren<Light>())
            lit.intensity = 0;
        if (explosionPrefab)
        {
            GameObject expl = Instantiate(explosionPrefab, transform.position, transform.rotation);
            expl.transform.localScale = velMulti * transform.lossyScale;
        }

        Collider[] colliders = Physics.OverlapSphere(transform.position, maxDistance);
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
                rb.AddExplosionForce(explosionForce*velMulti*chargeMulti, transform.position, maxDistance, 3.0F, ForceMode.Impulse);
        }

        Destroy(gameObject,2);
    }

#if UNITY_EDITOR
        void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, minDistance);

        Gizmos.color = Color.Lerp(Color.red, Color.yellow, 0.5f); 
        Gizmos.DrawWireSphere(transform.position, maxDistance);
    }
#endif
}
