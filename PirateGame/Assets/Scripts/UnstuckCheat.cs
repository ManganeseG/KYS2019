using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UnstuckCheat : MonoBehaviour
{
    public Crew c;
    public bool stuck = false;

    public bool playEventGratz = false;
    public bool playEventThreat = false;
    public bool playEventSlap = false;
    public bool playEventHelp = false;
    private bool canUnstuck = false;
    // Start is called before the first frame update
    void Awake()
    {
        c = this.GetComponent<Crew>();
        c.Stuck.AddListener(UpdateStuck);
        c.UnStuck.AddListener(UpdateUnStuck);
    }

    // Update is called once per frame
    void Update()
    {
        if(canUnstuck)
        {
            if (stuck == false)
            {
                c.UnStuck.Invoke();
                canUnstuck = false;
            }
        }
        if (c.Interactable == true)
        {
            if (playEventGratz == true)
            {
                c.Congratulated.Invoke();
                playEventGratz = false;
            }
            else if (playEventThreat == true)
            {
                c.Threaten.Invoke();
                playEventThreat = false;
            }
            else if (playEventHelp == true)
            {
                c.Helped.Invoke();
                playEventHelp = false;
            }
            else if (playEventSlap == true)
            {
                c.Slapped.Invoke();
                playEventSlap = false;
            }
                
        }
    }
    public void UpdateStuck()
    {
        stuck = true;
        canUnstuck = true;
    }
    public void UpdateUnStuck()
    {
        stuck = false;
    }
}
