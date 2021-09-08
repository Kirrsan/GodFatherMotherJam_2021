using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderChanger : MonoBehaviour
{
    public float timeRemaining;
    private const float timerMax = 5f;
    public Slider slider;

    
    void Start()
    {
        
    }

   
    void Update()
    {
        slider.value = CalulateSliderValue();

        if(Input.GetKeyDown(KeyCode.Space))
        {
            timeRemaining = timerMax;
        }
        if (timeRemaining <= 0)
        {
            timeRemaining = 0;
        }
        else if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
        }
     
    }

    float CalulateSliderValue()
    {
       return (timeRemaining / timerMax);
    }
}
