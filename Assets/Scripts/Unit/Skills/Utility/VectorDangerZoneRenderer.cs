using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VectorDangerZoneRenderer : MonoBehaviour
{
    [SerializeField]
    private Transform vectorZone;

    private float startSize = 0.1f;

    private float duration = 0;
    private float time = 0;
    private float size = 0;

    private float timer = 0;

    private float length = 0;
    private float sizeDelta = 0;

    public void SetActive(Vector2 size, float duration)
    {
        time = 0;
        this.duration = duration;
        this.length = size.y;
        vectorZone.localScale = new Vector3(size.x, 0.1f, 1);
        vectorZone.transform.position = new Vector3(vectorZone.transform.position.x, vectorZone.transform.position.y + (startSize / 2), vectorZone.transform.position.z);
        this.size = startSize;
        timer = 0;
        gameObject.SetActive(true);
    }

    public void SetRotation(Vector3 point)
    {
        //gameObject.transform.rotation = Quaternion.Euler(rotation);

        //transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, (Mathf.Atan2(point.y - transform.position.y, point.x - transform.position.x) * Mathf.Rad2Deg - 90)); //Переписать

        float angle = Mathf.Atan2(point.y, point.x) * Mathf.Rad2Deg - 90;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    private void Update()
    {
        if (time < duration)
        {
            sizeDelta = size;
            size = Mathf.Lerp(0.1f, length, time / duration);
            sizeDelta = size - sizeDelta;
            vectorZone.transform.localScale = new Vector3(vectorZone.transform.localScale.x, size, 1);
            //vectorZone.transform.position = new Vector3(vectorZone.transform.position.x, vectorZone.transform.position.y + sizeDelta, vectorZone.transform.position.z);
            vectorZone.transform.localPosition = new Vector3(vectorZone.transform.localPosition.x, vectorZone.transform.localScale.y / 2, vectorZone.transform.localPosition.z);
            time += Time.deltaTime;
        }
        else
        {
            //Debug.Log("position " + vectorZone.position + " scale " + vectorZone.localScale);
            vectorZone.localPosition = Vector3.zero;
            vectorZone.localScale = new Vector3(1, 1, 1);
            gameObject.SetActive(false);
        }
        timer += Time.deltaTime;
    }
}
