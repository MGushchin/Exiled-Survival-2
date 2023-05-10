using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AilmentsStorage : MonoBehaviour
{
    public UnitActions Owner;
    private const float updateRateFrequency = 0.25f;
    private IEnumerator updateCoroutine;
    private List<string> statuses = new List<string>();

    public void SetActiveStatuses(bool value, bool clearAllStatus)
    {

    }

    public void AddStatus(string status)
    {

    }

    private IEnumerator update()
    {
        while (true)
        {


            yield return new WaitForSeconds(updateRateFrequency);
        }
    }
}
