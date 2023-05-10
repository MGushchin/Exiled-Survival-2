using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropableItemsFactory : MonoBehaviour
{
    public static DropableItemsFactory Instance; //¬озможно перевод в нестатический класс
    public GameObject ExperiencePrefab;
    private List<PickupableItem> activeItems = new List<PickupableItem>();
    private Queue<PickupableItem> reservedItems = new Queue<PickupableItem>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void Start()
    {
        fillPool(10);
    }

    public void CreateExperienceItem(float experienceCount, Vector3 position)
    {
        if(reservedItems.Count == 0)
        {
            fillPool(5);
        }
        PickupableItem item = reservedItems.Dequeue();
        activeItems.Add(item);
        item.SetStoredExperience(experienceCount);
        item.SetPosition(position);
        item.gameObject.SetActive(true);
    }

    public GameObject CreateInactiveExperienceItem(float experienceCount)
    {
        if (reservedItems.Count == 0)
        {
            fillPool(5);
        }
        PickupableItem item = reservedItems.Dequeue();
        activeItems.Add(item);
        item.SetStoredExperience(experienceCount);
        return item.gameObject;
    }

    private void fillPool(int count)
    {
        for(int i=0; i < count; i++)
        {
            GameObject temporal = Instantiate(ExperiencePrefab);
            temporal.gameObject.SetActive(false);
            PickupableItem item = temporal.GetComponent<PickupableItem>();
            item.OnPickup.AddListener(ReturnToPool);
            reservedItems.Enqueue(item);

        }
    }

    public void ReturnToPool(PickupableItem item)
    {
        item.gameObject.SetActive(false);
        activeItems.Remove(item);
        reservedItems.Enqueue(item);
    }
}
