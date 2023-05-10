using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float Smooth = 5;
    private Transform target;
    private Transform cameraFollower;
    private Vector3 offset;
    private bool following = false;
    private IEnumerator moving;

    private void Start()
    {
        cameraFollower = gameObject.GetComponent<Transform>(); //����������� �������� ���������
    }

    public void SetTarget(Transform target)
    {
        offset = target.transform.position - cameraFollower.position; //���������� ������� ��������� ������ � ������� ��������
        this.target = target;
        following = true;
        moving = followTarget();
        StartCoroutine(moving);
    }

    private IEnumerator followTarget()
    {
        while(following)
        {
            //float desiredAngle = target.transform.eulerAngles.y;
            //Quaternion rotation = Quaternion.Euler(0, desiredAngle, 0); //������ �������� ���� �������� ������ �� ��� y
            Vector3 targetPosition = new Vector3(target.transform.position.x, target.transform.position.y, target.transform.position.z);
            cameraFollower.position = Vector3.Lerp(cameraFollower.position, targetPosition - (offset), Time.fixedDeltaTime * Smooth);
            yield return new WaitForFixedUpdate();
        }
    }

}
