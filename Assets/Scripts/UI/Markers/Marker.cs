using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UIMarkers
{
    public class Marker : MonoBehaviour
    {
        public GameObject MarkerObject;
        public SpriteRenderer MarkerImage;
        public List<Sprite> markerTypeSprites = new List<Sprite>();
        private Vector3 bounds;
        private Transform target;
        private Transform player;

        public void Init(Vector3 bounds, Transform player)
        {
            this.bounds = bounds;
            this.player = player;
        }

        private void Update()
        {
            Vector3 direction = Vector3.Normalize(target.position - player.position);
            float rotateZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Vector3 position = new Vector3(Mathf.Clamp(target.position.x, player.position.x - bounds.x, player.position.x + bounds.x), Mathf.Clamp(target.position.y, player.position.y - bounds.y, player.position.y + bounds.y), 0);
            MarkerObject.transform.position = position;
            MarkerObject.transform.rotation = Quaternion.Euler(0, 0, rotateZ - 90);
        }


        public void SetMarkerTarget(Transform target, MarkerType type)
        {
            this.target = target;
            MarkerImage.sprite = markerTypeSprites[(int)type];
        }
    }
}
