using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    [SerializeField]
    private AudioSource source;

    private void OnEnable()
    {
        source.Play();
    }

    private void OnDisable()
    {
        source.Stop();
    }

}
