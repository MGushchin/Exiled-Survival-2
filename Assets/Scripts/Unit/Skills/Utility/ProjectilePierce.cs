using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ProjectilePierce : MonoBehaviour
{
    public UnityEvent<Vector3> OnLastPierce = new UnityEvent<Vector3>();
    private Transform selfTransform;
    [SerializeField]
    private int pierceCount = 0;

    private void Start()
    {
        selfTransform = gameObject.transform;
    }

    public void SetPierce(int pierceCount)
    {
        this.pierceCount = pierceCount;
    }

    public void Hit(Vector3 position)
    {
        if (pierceCount <= 0)
        {
            OnLastPierce.Invoke(selfTransform.position);
        }
        else
            pierceCount--;
    }
}
