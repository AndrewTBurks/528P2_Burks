using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundTriggerBehavior : MonoBehaviour
{
    public GameObject player;
    public AudioSource audio;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter(Collider c)
    {
        if (c == player.GetComponent<Collider>() && !audio.isPlaying)
        {
            audio.Play(0);
        }
    }
}
