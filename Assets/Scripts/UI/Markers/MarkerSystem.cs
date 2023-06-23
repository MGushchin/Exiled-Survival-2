using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace UIMarkers
{
    public enum MarkerType
    {
        Boss,
        Goodies
    }

    public class MarkerSystem : MonoBehaviour
    {
        public static MarkerSystem instance;

        private void Awake()
        {
            if (instance == null)
                instance = this;
        }

        public GameObject MarkerPrefab;
        public Queue<Marker> Markers = new Queue<Marker>();
        private Transform player;
        private Vector3 bounds;
        private Transform holder;

        public void Init(Transform player, Transform canvasTransform)
        {
            this.player = player;
            GameObject holderGO = new GameObject();
            holderGO.name = "Markers holder";
            //holderGO.transform.parent = canvasTransform;
            //holderGO.transform.localScale = new Vector3(1, 1, 1);
            //holderGO.transform.position = new Vector3(0, 0, 0); //???, по умолчанию скейл 108, 108, 108
            holder = holderGO.transform;

            float x = Camera.main.pixelWidth;
            float y = Camera.main.pixelHeight;
            Vector3 camera = new Vector3(x, y, Camera.main.depth);
            Vector3 boundsSize = Camera.main.ScreenToWorldPoint(camera);
            bounds = new Vector2(boundsSize.x - 0.5f, boundsSize.y - 0.5f);

            addMarkers(10);
        }

        public Marker Register(Transform target, MarkerType type)
        {
            if (Markers.Count == 0)
                addMarkers(1);
            Marker marker = Markers.Dequeue();
            marker.SetMarkerTarget(target, type);
            marker.gameObject.SetActive(true);
            return marker;
        }

        public void Unregister(Marker marker)
        {
            marker.gameObject.SetActive(false);
            Markers.Enqueue(marker);
        }

        private void addMarkers(int count)
        {
            for (int i = 0; i < count; i++)
            {
                Marker marker = Instantiate(MarkerPrefab, holder).GetComponent<Marker>();
                marker.Init(bounds, player);
                marker.gameObject.SetActive(false);
                Markers.Enqueue(marker);
            }
        }
    }
}

