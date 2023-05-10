using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pickup
{
    public class PlayerPicker : MonoBehaviour
    {
        public UnitActions Owner;
        public Collider2D PickupCollider;
        public Collider2D TakingCollider;

        //private void OnTriggerEnter2D(Collider2D collision) //Ограничить матрицу коллизий
        //{
        //    if(collision.gameObject.GetComponent<PickupableItem>() != null)
        //    {

        //    }
        //}

        public void Pickup(PickupableItem item)
        {
            item.Pickup(Owner);
        }

        public void Take(PickupableItem item)
        {
            Owner.TakeExperience(item.TakeItem());
        }
    }
}
