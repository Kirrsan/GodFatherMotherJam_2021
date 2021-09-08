using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderChanger : MonoBehaviour
{
    public Slider slider;
    public chronos timer;
    private void Start()
    {
        slider.maxValue = timer.timeMax;
    }
    void Update()
    {
        slider.value = timer.calculTimer;
        
    }
}
