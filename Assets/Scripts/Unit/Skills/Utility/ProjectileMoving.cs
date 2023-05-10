using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMoving : MonoBehaviour
{
    [SerializeField]
    private Transform selfTransform;
    [SerializeField]
    private float speed = 2;
    public float Speed => speed;

    public void SetSpeed(float value)
    {
        speed = value;
    }

    private void FixedUpdate()
    {
        selfTransform.Translate(new Vector3(0, speed * Time.fixedDeltaTime, 0));
    }
}
