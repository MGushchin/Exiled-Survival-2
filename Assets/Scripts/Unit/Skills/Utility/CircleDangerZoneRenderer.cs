using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.iOS;

public class CircleDangerZoneRenderer : MonoBehaviour
{
    [SerializeField]
    private Transform circleEmpty;
    [SerializeField]
    private Transform circleFilled;

    private float startSize = 0.1f;

    private float duration = 0;
    private float time = 0;
    private float size = 0;

    private float timer = 0;

    public void SetActive(float size, float duration)
    {
        time = 0;
        this.duration = duration;
        transform.localScale = new Vector3(size, size, 1);
        circleFilled.transform.localScale = new Vector3(startSize, startSize, 1);
        this.size = startSize;
        timer = 0;
        gameObject.SetActive(true);
    }

    private void Update()
    {
        //if (timer < duration)
        if (time < duration)
        {
            size = Mathf.Lerp(0.01f, 1, time / duration);
            circleFilled.transform.localScale = new Vector3(size, size, 1);
            time += Time.deltaTime;

            //size = timer / duration;
            //circleFilled.transform.localScale = new Vector3(size, size, 1);
        }
        else
        {
            Debug.Log("Timer " + timer + " " + size);
            gameObject.SetActive(false);
        }
        timer += Time.deltaTime;
    }

}
