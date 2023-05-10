using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitDrop : MonoBehaviour
{
    private List<GameObject> storedItems = new List<GameObject>();

    public void AddItemsToDrop(List<GameObject> items)
    {
        foreach (GameObject item in items)
            storedItems.Add(item);
    }

    public void AddItemToDrop(GameObject item)
    {
            storedItems.Add(item);
    }

    public void DropItems()
    {
        DropableItemsFactory.Instance.CreateExperienceItem(1, transform.position); //Переписать
        float x = 0;
        float y = 0;
        foreach(GameObject item in storedItems)
        {
            item.transform.position = transform.position + new Vector3(x, y, 0);
            item.SetActive(true);
            x += 0.1f;
            y += 0.1f;
        }
        storedItems.Clear();
    }
}
