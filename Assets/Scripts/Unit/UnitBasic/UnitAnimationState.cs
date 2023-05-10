using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnitState
{
    Idle,
    Attack,
    Run,
    Stun,
    Death
}

public class UnitAnimationState : MonoBehaviour
{
    public Animator Animations;
    private UnitState currentState;

    public void SetMovementspeedMultiplier(float value)
    {
        Animations.SetFloat("MovementSpeed", value);
    }

    public void SetAttackSpeedMultiplier(float value)
    {
        //Debug.Log("Animations value " + value);
        Animations.SetFloat("AttackSpeed", value);
    }

    public void SetState(UnitState newState)
    {
        currentState = newState;
        Animations.SetBool("Run", false);
        Animations.SetBool("Death", false);
        Animations.SetBool("Idle", false);
        Animations.SetBool("Stun", false);
        switch (currentState)
        {
            case (UnitState.Attack):
                {
                    
                    Animations.SetTrigger("Attack");
                }
                break;
            case (UnitState.Idle):
                {
                    Animations.SetBool("Idle", true);
                }
                break;
            case (UnitState.Run):
                {
                    Animations.SetBool("Run", true);
                }
                break;
            case (UnitState.Stun):
                {
                    Animations.SetBool("Stun", true);
                }
                break;
            case (UnitState.Death):
                {
                    Animations.SetBool("Death", true);
                }
                break;
        }
    }
}
