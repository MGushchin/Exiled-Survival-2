using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitTimeMultiplierSetter : MonoBehaviour
{
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private float time;

    public void SetHitAnimationTime(float duration) //Õ≈ –¿¡Œ“¿≈“
    {
        time = 1 / duration;
        animator.SetFloat("Speed", time);
        animator.SetFloat("Speed", time);
    }
}
