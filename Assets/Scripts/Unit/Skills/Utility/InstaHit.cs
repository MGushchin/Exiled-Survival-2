using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstaHit : Hit
{
    private IEnumerator activationDelay;

    private void OnEnable()
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
