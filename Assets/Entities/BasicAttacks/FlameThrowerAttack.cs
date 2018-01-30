using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameThrowerAttack : BasicAttack
{
    //GenericCharacter _attacker;
    public ParticleSystem flameEffect;
    public float startTime = 0.1f;
    public float endTime = 0.9f;
    float _nextHitTime;

    protected override void Start()
    {
        base.Start();
        flameEffect.Stop();
    }

    public override void DoAttack(GenericCharacter attacker)
    {
        if (!_ready) return;

        //_ready = false;
        //_attacker = attacker;
        //PlayAnimation(attacker);

        base.DoAttack(attacker);

        StartCoroutine(EnableEffectRoutine());
        StartCoroutine(AttackRoutine());
    }

    IEnumerator EnableEffectRoutine()
    {
        yield return new WaitForSeconds(startTime);
        _nextHitTime = Time.time;
        flameEffect.Play();
        StartCoroutine(DisableEffectRoutine());
    }

    IEnumerator DisableEffectRoutine()
    {
        yield return new WaitForSeconds(endTime - startTime);
        flameEffect.Stop();
    }

    protected override IEnumerator AttackRoutine()
    {
        yield return base.AttackRoutine();
    }
}
