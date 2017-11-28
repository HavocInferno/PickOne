using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasicAttack : MonoBehaviour
{
    protected int damage;

    // TODO: Pick a better name for this function
    public abstract void DoAttack(int mDamage);
}
