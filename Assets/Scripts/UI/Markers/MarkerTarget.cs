using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UIMarkers
{
    public class MarkerTarget : MonoBehaviour
    {
        private MarkerSystem mainSystem;
        private Marker marker;
        private Transform selfTransform;
        private MarkerType type;

        private void Awake()
        {
            //markRenderer = FindObjectOfType<MarkerRenderer>();
            mainSystem = MarkerSystem.instance;
        }

        public void Init(MarkerType type)
        {
            selfTransform = gameObject.transform;
            this.type = type;
        }

        private void OnEnable()
        {
            if (marker == null)
                marker = mainSystem.Register(selfTransform, type);
        }

        private void OnBecameVisible()
        {
            if (marker != null)
                mainSystem.Unregister(marker);
            marker = null;
        }

        private void OnBecameInvisible()
        {
            if (marker == null)
                marker = mainSystem.Register(selfTransform, type);
        }

        private void OnDisable()
        {
            if (marker != null)
                mainSystem.Unregister(marker);
            marker = null;
        }
    }
}
