using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pickup
{
    public class TargetFollow : MonoBehaviour
    {
        public Transform MovingObject;
        [SerializeField]
        private float speed = 1;
        private Transform targetTransfrom;
        private IEnumerator movingCoroutine;

        public void SetFollow(Transform target)
        {
            targetTransfrom = target;
            movingCoroutine = follow();
            StartCoroutine(movingCoroutine);
        }

        private IEnumerator follow()
        {
            while (true)//Переписать
            {
                MovingObject.rotation = Quaternion.Euler(MovingObject.rotation.eulerAngles.x, MovingObject.rotation.eulerAngles.y, (Mathf.Atan2(targetTransfrom.position.y - MovingObject.position.y, targetTransfrom.position.x - MovingObject.position.x) * Mathf.Rad2Deg - 90)); //Переписать
                MovingObject.Translate(new Vector3(0, speed * Time.fixedDeltaTime, 0));
                yield return new WaitForFixedUpdate();
            }
        }
    }
}
