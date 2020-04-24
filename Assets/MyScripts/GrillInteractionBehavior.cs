using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrillInteractionBehavior : CAVE2Interactable
{
    public GameObject lid;
    public Transform openPosition;
    public Transform closedPosition;

    public Light light;
    public ParticleSystem particleSystem;

    public AudioSource lidSound;
    public AudioSource fireSound;

    public bool isOn = false;
    public CAVE2.Button triggerButton;
    public GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        // start off
        UpdateGrillState(isOn);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void UpdateGrillState(bool isOn)
    {
        if (isOn)
        {
            lid.transform.position = openPosition.position;
            lid.transform.rotation = openPosition.rotation;

            light.intensity = 5;
            particleSystem.Play();

            lidSound.Play(0);
            fireSound.Play(0);
        } else
        {
            lid.transform.position = closedPosition.position;
            lid.transform.rotation = closedPosition.rotation;

            light.intensity = 0;
            particleSystem.Stop();

            lidSound.Play(0);
            fireSound.Pause();
        }
    }

    public void OnWandButtonDown(CAVE2.WandEvent evt)
    {
        CAVE2PlayerIdentity playerID = evt.playerID;
        //int wandID = evt.wandID;
        CAVE2.Button button = evt.button;
        //CAVE2.InteractionType interactionType = evt.interactionType;

        Debug.Log(playerID);

        //Debug.Log("OnWandButtonDown: " + playerID.name + " " + wandID + " " + button);

        if (button == triggerButton && Vector3.Distance(player.transform.position, transform.position) < 10)
        {
            isOn = !isOn;

            UpdateGrillState(isOn);
        }


        //Debug.Log("OnWandButtonDown: ");
        //Debug.Log(evt);
    }
}
