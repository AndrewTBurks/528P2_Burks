using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeriodicNoise : MonoBehaviour
{
    public AudioSource noise;
    public float chance = 0.005f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (chance > Random.value && !noise.isPlaying)
        {
            noise.Play(0);
        }
    }
}
