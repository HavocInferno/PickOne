using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Collectible : NetworkBehaviour
{
    public ParticleSystem permanentEffect;
    public ParticleSystem activeEffect;

    void OnTriggerEnter(Collider other)
    {
        if (isServer)
        {
            GetComponent<Collider>().enabled = false;
            RpcCollect(other.gameObject);
            Destroy(gameObject, 10.0f);
        }
    }

    [ClientRpc]
    void RpcCollect(GameObject character)
    {
        Debug.LogFormat("Collectible {0} was collected", name);
        GetComponent<Renderer>().enabled = false;
        permanentEffect.Stop();
        activeEffect.Play();
        Collect(character);
    }

    public virtual void Collect(GameObject character)
    {
    }
}
