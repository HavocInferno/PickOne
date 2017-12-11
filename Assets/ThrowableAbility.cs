using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableAbility : MonoBehaviour {


    public float chargeMulti =1f;
    public float minVel=1, maxVel =10;
    public float minDistance= 1, maxDistance=10;
    public AbstractEffect effect;
    public GameObject explosionPrefab;

    private Rigidbody rb;
    private PlayersManager playerManager;

    // Use this for initialization
    void Start () {
        transform.localScale *= chargeMulti;
        playerManager = FindObjectOfType<PlayersManager>();
        rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
		
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

                if (crawler.GetComponent<Crawler>() != null)
                    continue;
                applyEffect(totalMulti, crawler.GetComponent<Crawler>());
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

                if (enemy.GetComponent<Enemy>() != null)
                    continue;
                applyEffect(totalMulti, enemy.GetComponent<Enemy>());
            }
        }
        if(explosionPrefab)
            Instantiate(explosionPrefab, transform.position, transform.rotation);
        Destroy(gameObject);
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
        return Mathf.Clamp((((transform.position - other.position).magnitude) - minDistance) / (maxDistance - minDistance), 0, 1);
    }

    public virtual void applyEffect(float multi, GenericCharacter target)
    {
        //TODO: fill
        Debug.Log("Applying affect: "+ multi);
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
