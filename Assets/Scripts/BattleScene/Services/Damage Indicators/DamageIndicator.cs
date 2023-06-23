using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class DamageIndicator : MonoBehaviour
{
    public TMP_Text TextValue;
    public Canvas Canvas;
    public UnityEvent<DamageIndicator> OnDissapearing = new UnityEvent<DamageIndicator>();

    [SerializeField]
    private float speed = 0.75f;
    [SerializeField]
    private float delayTime = 0.75f;

    private IEnumerator dissapearingCorotine;

    private void Awake()
    {
        Canvas.worldCamera = Camera.main;
    }

    public void SetIndicator(string value, Color textColor, float size)
    {
        TextValue.color = textColor;
        TextValue.text = value;
        //transform.localScale = new Vector3(size, size, 1);

        gameObject.SetActive(true);

        dissapearingCorotine = dissapearingDelay(delayTime);
        StartCoroutine(dissapearingCorotine);
    }

    private void Update()
    {
        transform.Translate(new Vector3(0, speed * Time.deltaTime, 0));
        TextValue.color = new Color(TextValue.color.r, TextValue.color.g, TextValue.color.b, TextValue.color.a - (1 * Time.deltaTime));
    }

    private IEnumerator dissapearingDelay(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        OnDissapearing.Invoke(this);
        gameObject.SetActive(false);
    }
}
