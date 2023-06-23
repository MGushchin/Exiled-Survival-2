using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Homing : MonoBehaviour
{
    private bool ally = true;
    private float speed = 1;
    private Transform selfTransform;
    private Vector3 targetPosition = Vector3.zero;
    [SerializeField]
    private float homingUpdateTime = 1;
    private IEnumerator findingCoroutine;

    private void Awake()
    {
        selfTransform = transform;
    }

    private void OnEnable()
    {
        findingCoroutine = findingUpdate(homingUpdateTime);
        StartCoroutine(findingCoroutine);
    }

    private void OnDisable()
    {
        StopCoroutine(findingCoroutine);
    }

    public void SetAlly(bool ally)
    {
        this.ally = ally;
    }

    public void SetSpeed(float speed)
    {
        this.speed = speed;
    }

    private void FixedUpdate()
    {
        //Vector3 direction = Vector3.Normalize(targetPosition - selfTransform.position);
        Vector3 direction = Vector3.Normalize(selfTransform.position - targetPosition);
        selfTransform.Translate(direction * speed * Time.fixedDeltaTime);
    }

    private IEnumerator findingUpdate(float updateTime)
    {
        while(gameObject.activeSelf)
        {
            findingTarget();
            yield return new WaitForSeconds(updateTime);
        }
    }

    private void findingTarget()
    {
        targetPosition = UnitPool.instance.GetNearestFromPool(selfTransform.position, !ally).transform.position;
        if(targetPosition == null)
            targetPosition = Vector3.zero;
    }
}
