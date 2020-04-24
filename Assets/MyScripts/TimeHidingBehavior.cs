using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeHidingBehavior : MonoBehaviour
{
    public GameObject[] objects;
    public float activeStartHour;
    public float activeEndHour;
    public Slider slider;

    // Start is called before the first frame update
    void Start()
    {
        UpdateHour();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateHour()
    {
        float hour = slider.value;

        bool isActive = (hour >= activeStartHour && hour < activeEndHour) || (activeStartHour > activeEndHour && (hour >= activeStartHour || hour < activeEndHour));
        Debug.Log(activeStartHour + "-" + activeEndHour + ": " + isActive);

        foreach (GameObject go in objects)
        {
            go.SetActive(isActive);
        }
    }
}
