using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitCanvasRegister : MonoBehaviour
{
    public int OrderInLayer = 3;

    private void Awake()
    {
        Canvas canvas = gameObject.GetComponent<Canvas>();
        canvas.worldCamera = Camera.main;
        canvas.sortingOrder = OrderInLayer;
    }
}
