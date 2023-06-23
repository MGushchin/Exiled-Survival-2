using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class DamageIndicatorData
{
    public float Value;
    public Vector3 Position;
    public float Impotance;
}

public class DamageIndicatorSpawner : MonoBehaviour
{
    public GameObject IndicatorPrefab;
    public static DamageIndicatorSpawner instance;

    private Queue<DamageIndicator> reservePool = new Queue<DamageIndicator>();
    private List<DamageIndicator> activePool = new List<DamageIndicator>();

    private float randomTextSpreading = 0.5f;

    private Transform indicatorsHolder;

    private Dictionary<int, Color> colorByImpotance = new Dictionary<int, Color>() 
    {
        { 0, Color.grey },
        { 1, Color.white    },
        { 2,  Color.yellow  },
        { 3,  Color.red }
    };


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

            GameObject holder = new GameObject();
            holder.name = "Damage indicators holder";
            indicatorsHolder = holder.transform;

            addIndicators(50);
        }
    }

    public void IndicateDamage(float value, Vector3 position)
    {
        if(reservePool.Count == 0)
            addIndicators(1);
        DamageIndicator indicator = reservePool.Dequeue();
        Color color = colorByImpotance[1];
        indicator.transform.position = new Vector3(position.x + Random.Range(-randomTextSpreading, randomTextSpreading), position.y + Random.Range(-randomTextSpreading, randomTextSpreading), 1);
        indicator.SetIndicator(((int)value).ToString(), color, 1);
        indicator.OnDissapearing.AddListener(returnToPool);
        activePool.Add(indicator);
    }

    private void addIndicators(int count)
    {
        for(int i=0; i < count; i++)
        {
            DamageIndicator indicator = Instantiate(IndicatorPrefab, indicatorsHolder).GetComponent<DamageIndicator>();
            //indicator.OnDissapearing.AddListener(returnToPool);
            indicator.gameObject.SetActive(false);
            reservePool.Enqueue(indicator);
        }
    }

    public void returnToPool(DamageIndicator indicator)
    {
        activePool.Remove(indicator);
        indicator.OnDissapearing.RemoveListener(returnToPool);
        reservePool.Enqueue(indicator);
    }
}
