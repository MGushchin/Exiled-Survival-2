using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class UnitMove : MonoBehaviour
{
    public NavMeshAgent Agent;
    [HideInInspector]
    public UnityEvent OnReachingPosition = new UnityEvent();
    public float UnitMovementSpeed => Agent.speed;
    public Vector3 CurrentMoveDirection => currentMoveDirection;
    [SerializeField]
    private SpriteRenderer unitImage; // Переписать
    private Transform selfTransform;
    [Header("Settings")]
    [SerializeField]
    private float reachingDistance = 0.5f;
    [SerializeField]
    private float checkingInterval = 0.1f;
    private Vector3 currentMoveDirection = new Vector3(1, 0, 0);
    private Vector3 currentTargetPosition;
    private IEnumerator movingCoroutine;
    public bool IsMoving => move;
    private bool move = false; // Возможно исправить
    private bool canMove = true;

    private void Awake()
    {
        selfTransform = gameObject.transform;
        movingCoroutine = moving();
        Agent.updateRotation = false;
        Agent.updateUpAxis = false;
    }

    public void SetUnitMovespeed(float movespeed)
    {
        Agent.speed = movespeed;
    }

    public void MoveTo(Vector3 position)
    {
        if (canMove)//Возможно переписать контроль
        {
            LookAt(position);
            currentMoveDirection = Vector3.Normalize(position - selfTransform.position);
            currentTargetPosition = position;
            movingCoroutine = moving();
            StartCoroutine(movingCoroutine);
            Agent.SetDestination(position);
        }
    }

    public void Move(Vector3 position)
    {
        if (canMove)//Возможно переписать контроль
        {
            currentMoveDirection = position;
            position = selfTransform.position + position;
            LookAt(position);
            currentTargetPosition = position;
            //var agentDrift = 0.0001f; // minimal
            //currentTargetPosition.x += agentDrift; //debug
            //currentTargetPosition.y += agentDrift;
            //position.x += agentDrift; //debug
            //position.y += agentDrift;
            Agent.SetDestination(position);
            if (!move)
            {
                movingCoroutine = moving();
                StartCoroutine(movingCoroutine);
            }
        }
    }

    public void Stop()
    {
        StopCoroutine(movingCoroutine);
        move = false;
        Agent.ResetPath();
    }

    public void SetMove(bool value)
    {
        canMove = value;
        if(!canMove)
        {
            Stop();
        }

    }

    public void LookAt(Vector3 lookDirection)
    {
        if (selfTransform.position.x <= lookDirection.x)
        {
            //imageTransform.rotation = Quaternion.Euler(0, 0, 0);
            unitImage.flipX = false;
        }
        else
        {
            //imageTransform.rotation = Quaternion.Euler(0, 180, 0);
            unitImage.flipX = true;
        }
    }

    private IEnumerator moving()
    {
        move = true;
        while (Vector3.Distance(selfTransform.position, currentTargetPosition) >= reachingDistance)
        {
            yield return new WaitForSeconds(checkingInterval);
            //if (selfTransform.position.x <= currentTargetPosition.x)
            //    imageTransform.rotation = Quaternion.Euler(0, 0, 0);
            //else
            //    imageTransform.rotation = Quaternion.Euler(0, 180, 0);
        }
        OnReachingPosition.Invoke();
        Stop();
    }
}
