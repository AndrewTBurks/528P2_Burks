using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinklerSwitchBehavior : CAVE2Interactable
{
    public SprinklerBehavior targetScript;
    private bool isOn = false;
    public CAVE2.Button triggerButton;

    public Light onLight;
    public Light offLight;

    public GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        UpdateItems();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void OnWandButtonDown(CAVE2.WandEvent evt)
    {
        CAVE2PlayerIdentity playerID = evt.playerID;
        int wandID = evt.wandID;
        CAVE2.Button button = evt.button;
        CAVE2.InteractionType interactionType = evt.interactionType;


        Debug.Log("OnWandButtonDown: " + playerID.name + " " + wandID + " " + button);

        if (button == triggerButton && Vector3.Distance(player.transform.position, transform.position) < 10)
        {
            isOn = !isOn;

            UpdateItems();
        }


        //Debug.Log("OnWandButtonDown: ");
        //Debug.Log(evt);
    }

    private void UpdateItems()
    {
        targetScript.setSprinklerState(isOn);

        onLight.enabled = isOn;
        offLight.enabled = !isOn;

    }
}
