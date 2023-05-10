using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaoticMoving : MonoBehaviour
{
    [SerializeField]
    private Transform selfTransform;
    public float OffsetMagnitude = 1;
    private Vector3 offset;

    private void Update()
    {
        offset = new Vector3(Random.Range(-OffsetMagnitude, OffsetMagnitude), Random.Range(-OffsetMagnitude, OffsetMagnitude), 0);
        selfTransform.Translate(new Vector3(offset.x * Time.fixedDeltaTime, offset.y * Time.fixedDeltaTime, 0));
    }
}
