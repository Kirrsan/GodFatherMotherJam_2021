using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class chronos : MonoBehaviour
{
    public Text tictacText;
    public float timeMax;
    private float timer;
    public float calculTimer;
    void Start()
    {
        timer = Time.time;
    }

    // Update is called once per frame
    void Update()
    {

        calculTimer = timeMax - (Time.time - timer);
        if (calculTimer > 0)
        {
            tictacText.text = calculTimer.ToString("F0");
        }
        if (calculTimer <= 0)
        {
           
        }

    }
}
