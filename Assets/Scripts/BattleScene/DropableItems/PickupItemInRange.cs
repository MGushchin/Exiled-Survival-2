using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pickup
{
    public class PickupItemInRange : MonoBehaviour
    {
        public PlayerPicker Picker;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.GetComponent<PickupableItem>() != null)
            {
                Picker.Pickup(collision.gameObject.GetComponent<PickupableItem>());
            }
        }
    }
}
