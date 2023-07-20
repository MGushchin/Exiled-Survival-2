using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayHit : Hit
{
    private IEnumerator activationDelay;

    private void Start()
    {
        HitCollider.enabled = false;
    }

    public void MakeHit()
    {
        HitCollider.enabled = true;
        activationDelay = hitTime();
        StartCoroutine(activationDelay);
    }

    private IEnumerator hitTime()
    {
        yield return new WaitForFixedUpdate();
        HitCollider.enabled = false;
    }
}
