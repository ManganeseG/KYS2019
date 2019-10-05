using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipCooldown : MonoBehaviour
{
    private Slider slider;
    private float startTimer = 0f;
    public float GameTime = 60f;

    //public enum DifferentEnds
    //{
    //    ,
    //    HappyEnd,
    //    HelpingEnd,
    //    MenacingEnd;
    //}

    public int HappinessFactor = 3;


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

            //startTimer = 0f;
            //GameEnd
        }
    }

    IEnumerator HapinessCalculation()
    {
        if (startTimer >= 1f)
        {
            if (HappinessFactor >= 8)
            {

            }
                //happy
            else if(HappinessFactor >= 6)
            {

            }
                    //petite victoire
            else if(HappinessFactor == 5)
            {

            }
                        //pas de trésor mais tous ensemble
            else if(HappinessFactor == 4)
            {

            }
                            //seul avec le bateau
            else if(HappinessFactor<= 3)
            {

            }
                                //supplice planche
        }

            yield return null;


    }
}
