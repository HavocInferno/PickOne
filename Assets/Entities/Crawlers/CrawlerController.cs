using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Crawler))]
public class CrawlerController : NetworkBehaviour
{
    public Vector2 movSpeed = new Vector2(4f, 4f);
	public Vector2 mov;
	public bool att;
    //called once per frame
    void Update()
    {
        if (!isLocalPlayer)
            return;

        //weapon firing. dumb and unoptimized.
        if (Input.GetButtonDown("Fire1"))
        {
            GetComponent<Crawler>().Attack();
        }

        if (Input.GetButtonDown("Ability1"))
        {
            GetComponent<Crawler>().ActivateAbility(0);
        }

        if (Input.GetButtonDown("Ability2"))
        {
            GetComponent<Crawler>().ActivateAbility(1);
        }

		if (Input.GetButtonDown("Ping"))
		{
			GetComponent<Crawler>().Ping();
		}

        //player movement..hor is forward/backward, ver is strafing
		Vector2 direction = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
		if (direction.magnitude > 1)
			direction /= direction.magnitude;
		mov = direction;
		att = Input.GetButtonDown("Fire1");
        direction = Time.deltaTime * Vector2.Scale(direction, movSpeed);
        transform.Translate(direction.x, 0, direction.y);
    }
}