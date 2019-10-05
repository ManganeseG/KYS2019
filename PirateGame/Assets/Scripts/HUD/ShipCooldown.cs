using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipCooldown : MonoBehaviour
{
    private Slider slider;
    private float startTimer = 0f;
    public float GameTime = 60f;



    void Start()
    {
        slider = GetComponent<Slider>();
    }

    void Update()
    {
        startTimer += Time.deltaTime / GameTime;
        slider.value = startTimer;

        if (startTimer >= 1f)
        {
            startTimer = 0f;
            //GameEnd
        }
    }
}
