using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SprinklerBehavior : MonoBehaviour
{
    public bool initialIsOn = false;
    // Start is called before the first frame update
    void Start()
    {
        setSprinklerState(initialIsOn);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setSprinklerState(bool isOn)
    {
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            // particle system
            var ps = gameObject.transform.GetChild(i).Find("Particle System").GetComponent<ParticleSystem>();

            if (isOn)
            {
                ps.Play();
            } else
            {
                ps.Stop();
            }
            // particle system
            gameObject.transform.GetChild(i).Find("Audio Source").GetComponent<AudioSource>().mute = !isOn;
        }
    }
}
