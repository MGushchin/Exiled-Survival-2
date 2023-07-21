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
    private IEnumerator animationsDelay;

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
        if(currentState != newState)
            changeAnimation(newState);
    }

    public void SetState(UnitState newState, float duration)
    {
        changeAnimation(newState);
        animationsDelay = delay(duration, UnitState.Idle);
        StartCoroutine(animationsDelay);
    }

    private void changeAnimation(UnitState newState)
    {
        currentState = newState;
        //Animations.SetBool("Run", false);
        //Animations.SetBool("Death", false);
        //Animations.SetBool("Idle", false);
        //Animations.SetBool("Stun", false);
            switch (currentState)
            {
                case (UnitState.Attack):
                    {
                        //Animations.SetTrigger("Attack");
                        Animations.Play("Attack");
                    }
                    break;
                case (UnitState.Idle):
                    {
                    //Animations.SetBool("Idle", true);
                    Animations.Play("Idle");
                }
                    break;
                case (UnitState.Run):
                    {
                        //Animations.SetBool("Run", true);
                        Animations.Play("Run");
                    }
                    break;
                case (UnitState.Stun):
                    {
                        //Animations.SetBool("Stun", true);
                        Animations.Play("Stun");
                    }
                    break;
                case (UnitState.Death):
                    {
                        //Animations.SetBool("Death", true);
                        Animations.Play("Death");
                    }
                    break;
            }
        currentState = newState;
    }

    private IEnumerator delay(float delayTime, UnitState nextState)
    {
        yield return new WaitForSeconds(delayTime);
        changeAnimation(nextState);
    }
}
