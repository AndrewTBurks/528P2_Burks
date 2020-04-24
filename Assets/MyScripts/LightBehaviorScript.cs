using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LightBehaviorScript : MonoBehaviour
{
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

        float lightLevel = hour < 7 || hour > 17 ? 0.8f : 0;

        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            gameObject.transform.GetChild(i).GetChild(1).GetComponent<Light>().intensity = lightLevel;
        }

        //gameObject.transform.GetChild(1);
    }
}
