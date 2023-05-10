using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Pickup;

public class PickupableItem : MonoBehaviour
{
    [HideInInspector]
    public UnityEvent<PickupableItem> OnPickup = new UnityEvent<PickupableItem>();
    public Transform selfTransform;
    public TargetFollow PickerFollow;
    public TrailRenderer Trail;
    private float storedExperience = 0;

    public void SetStoredExperience(float value)
    {
        storedExperience = value;
    }

    public void SetPosition(Vector3 position)
    {
        Trail.gameObject.SetActive(false);
        selfTransform.position = position;
        Trail.gameObject.SetActive(true);
    }

    public void Pickup(UnitActions picker)
    {
        PickerFollow.SetFollow(picker.selfTransform);
    }

    public float TakeItem()
    {
        OnPickup.Invoke(this);
        return storedExperience;
    }
}
