using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipCooldown : MonoBehaviour
{
    private Slider slider;
    private float startTimer = 0f;
    public float GameTime = 60f;
    //public string HappySentence;
    //public string GoodSentence;
    //public string NormalSentence;
    //public string BadSentence;
    //public string WorseSentence;
    public Text EndSentenceHolder;

    //public enum DifferentEnds
    //{
    //    ,
    //    HappyEnd,
    //    HelpingEnd,
    //    MenacingEnd;
    //}

    //public int HappinessFactor = 3;


    void Start()
    {
        slider = GetComponent<Slider>();
        EndSentenceHolder.text = null;
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

        StartCoroutine(HapinessCalculation());
    }

    IEnumerator HapinessCalculation()
    {
        //EndSentenceHolder.isEnabled = true;
        if (startTimer >= 1f)
        {
            if (CrewV2.HappinessGauge >= 8)
            {
                EndSentenceHolder.text = "On pouvait pas rêver d'un meilleur capitaine que toi !";
            }
            //happy
            else if (CrewV2.HappinessGauge >= 6)
            {
                EndSentenceHolder.text = "Félicitation tu es un bon capitaine mais tu peux mieux faire.";
            }
            //petite victoire
            else if (CrewV2.HappinessGauge == 5)
            {
                EndSentenceHolder.text = "Au moins tu as tes moussaillons.";
            }
            //pas de trésor mais tous ensemble
            else if (CrewV2.HappinessGauge == 4)
            {
                EndSentenceHolder.text = "Bon courage pour conduire ce navire seul.";
            }
            //seul avec le bateau
            else if (CrewV2.HappinessGauge <= 3)
            {
                EndSentenceHolder.text = "Bravo tu as tout raté.";
            }
            //supplice planche
        }

        yield return null;


    }
}
