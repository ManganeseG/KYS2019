using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;
public class Crew : MonoBehaviour
{
    #region CustomStatPirate
    public string typePirate = "Default Pirate Template";

    [Header("Skill level added to dicerolls")]
    [Range(-3, 3)]
    public int modifierSkillOnSail = 0;
    [Range(-3, 3)]
    public int modifierSkillOnBucket = 0;
    [Range(-3, 3)]
    public int modifierSkillOnBorder = 0;
    [Range(-3, 3)]
    public int modifierSkillOnCanon = 0;
    [Range(-3, 3)]
    public int modifierSkillOnAnchor = 0;
    #endregion

    #region Public Members
    public UnityEvent Slapped;//colère
    public UnityEvent Helped;//aide
    public UnityEvent UnStuck;
    public UnityEvent Death;
    public UnityEvent Stuck;
    public UnityEvent Congratulated;//soutiens
    public UnityEvent Threaten;//menace
    public UnityEvent EnteredArea;

    public Transform UpperDeskPos;
    public Transform LowerDeskPos;
    public Transform BucketPos;
    public Transform SailPos;
    public Transform BorderPos;
    public Transform CanonPos;
    public Transform AnchorPos;

    public Area CurrentArea;
    public float SpeedWalk = 3f;
    public float SpeedRun = 5f;
    public float SkillLevel = 0f;
    public int Handicap=0;
    public float DurationOfSlapBoost = 25f;
    public bool BoostActive=false;
    public string LastLocation = "";
    public float TimeStuckBeforeDeath = 30f;
    public float timerDeath;
    public string Destination;
    public bool StateStuck=false;
    public bool StateDead = false;
    public bool Interactable = false;
    public float TimeRemainInteractableAfterUnstuck=2f;
    public float DurationSlapAnim=2f;
    public float DurationGratzAnim=2f;
    public float DurationHelpAnim=15f;
    public float DurationThreatAnim=2f;
    public bool FinishedIteraction = false;
    public enum e_Location
    {
        UPPERDESK,
        LOWERDESK,
        BUCKETAREA,
        SAILAREA,
        BORDERAREA,
        CANONAREA,
        ANCHORAREA
    }
    public e_Location Location = e_Location.UPPERDESK;

    public enum e_characterState
    {
        IDLE,
        WALKING,
        RUNNING,
        BUCKET,
        RELOAD,
        SAIL,
        BORDER,
        ANCHOR,
        STAIRSCLIMBUP,
        STAIRSCLIMBDOWN,
        INTERACTEDWITH,
    }
    public e_characterState characterState = e_characterState.IDLE;
    #endregion

    #region Private Members
    private bool BucketBusy = false;
    private bool CanonBusy = false;
    private bool SailBusy = false;
    private bool BorderBusy = false;
    private bool AnchorBusy = false;
    private bool timer01Underway = false;
    private bool GratzB = false;
    private bool ThreatB = false;
    private bool HelpB = false;
    private bool SlapB = false;
    private NavMeshAgent Agent;
    private float ActualSpeed = 10f;
    private bool IsMoving = false;
    #endregion

    // Start is called before the first frame update
    void Awake()
    {
        timerDeath = TimeStuckBeforeDeath;
        Agent = GetComponent<NavMeshAgent>();
        Agent.speed = ActualSpeed;
        Slapped.AddListener(Slap);
        Helped.AddListener(HelpTeach);
        Congratulated.AddListener(Gratz);
        Threaten.AddListener(Threat);
        EnteredArea.AddListener(Arrived);
        UnStuck.AddListener(Unstuck);
        Stuck.AddListener(IsStuck);
    }
    // Update is called once per frame
    void Update()
    {
        CharStateHandle();
    }
    private void FixedUpdate()
    {
        if(StateStuck)
        {
            timerDeath -= Time.deltaTime;
            if(timerDeath<=0 && StateDead == false)
            {
                StateDead = true;
                Debug.Log("Crew death !");
                Death.Invoke();
            }
        }
        
    }
    private void HelpTeach()
    {
        if (Interactable)
        {
            HelpB = true;
            Interactable = false;
            switch ((int)SkillLevel)
            {
                case 0:
                    SkillLevel += 1f;
                    break;
                case 1:
                    SkillLevel += 0.5f;
                    break;
                case 2:
                    SkillLevel += 0.25f;
                    break;
                case 3:
                    break;
            }
        }
    }
    private void Gratz()
    {
        if (Interactable)
        {
            GratzB = true;
            Interactable = false;
            //add reputation+? and something?
        }
    }
    private void Threat()
    {
        if (Interactable)
        {
            ThreatB = true;
            Interactable = false;
            //add reputation-? and something?
        }
    }
    private void Slap()
    {
        if(Interactable)
        {
            SlapB = true;
            Interactable = false;
            BoostActive = true;
            if (characterState == e_characterState.WALKING)
            {
                StartCoroutine("slapTimer");
                characterState = e_characterState.RUNNING;
            }
        }
    }

    
    private void Unstuck()
    {
        switch (Location)
        {
            case e_Location.SAILAREA:
                StartCoroutine("unstuckSailTimer");
                break;
            case e_Location.BORDERAREA:
                StartCoroutine("unstuckBorderTimer");
                break;
            case e_Location.BUCKETAREA:
                StartCoroutine("unstuckBucketTimer");
                break;
            case e_Location.CANONAREA:
                StartCoroutine("unstuckCanonTimer");
                break;
            case e_Location.ANCHORAREA:
                StartCoroutine("unstuckAnchorTimer");
                break;
        }
    }
    
    private void MoveTo(Transform pos)
    {
        IsMoving = true;
        if (BoostActive)
            characterState = e_characterState.RUNNING;
        else
            characterState = e_characterState.WALKING;
        Agent.destination = pos.position;
    }
    private void Arrived()
    {
        
        if (CurrentArea.Location.ToString()!= LastLocation)// stop multiple entry on same area
        {
            IsMoving = false;
            characterState = e_characterState.IDLE;
        }
    }
    private void IsStuck()
    {
        StateStuck = true;
        timerDeath = TimeStuckBeforeDeath;
    }
    private void PickActivity()
    {
        int rand;
        switch (Location)
        {
            case e_Location.UPPERDESK:
                rand =(int)Random.Range(1, 5);

                switch (rand)
                {
                    case 1:
                        MoveTo(LowerDeskPos);
                        Destination = "LOWERDESK";
                        break;
                    case 2:
                        MoveTo(BucketPos);
                        Destination = "BUCKETAREA";
                        break;
                    case 3:
                        MoveTo(SailPos);
                        Destination = "SAILAREA";
                        break;
                    case 4:
                        MoveTo(BorderPos);
                        Destination = "BORDERAREA";
                        break;
                    case 5:
                        MoveTo(AnchorPos);
                        Destination = "ANCHORAREA";
                        break;
                }
                break;
            case e_Location.LOWERDESK:
                rand = (int)Random.Range(0, 2);

                switch (rand)
                {
                    case 0:
                        characterState = e_characterState.IDLE;
                        break;
                    case 1:
                        MoveTo(UpperDeskPos);
                        Destination = "UPPERDESK";
                        break;
                    case 2:
                        MoveTo(CanonPos);
                        Destination = "CANONAREA";
                        break;
                }
                break;
            case e_Location.BUCKETAREA:
                if(CurrentArea.UsersInArea.Count<2)
                {
                    characterState = e_characterState.BUCKET;
                }
                else
                {
                    rand = (int)Random.Range(0, 4);

                    switch (rand)
                    {
                        case 0:
                            MoveTo(LowerDeskPos);
                            Destination = "LOWERDESK";
                            break;
                        case 1:
                            MoveTo(UpperDeskPos);
                            Destination = "UPPERDESK";
                            break;
                        case 2:
                            MoveTo(SailPos);
                            Destination = "SAILAREA";
                            break;
                        case 3:
                            MoveTo(BorderPos);
                            Destination = "BORDERAREA";
                            break;
                        case 4:
                            MoveTo(AnchorPos);
                            Destination = "ANCHORAREA";
                            break;
                    }
                }
                
                break;
            case e_Location.SAILAREA:
                if (CurrentArea.UsersInArea.Count <2)
                {
                    characterState = e_characterState.SAIL;
                }
                else
                {
                    rand = (int)Random.Range(0, 4);

                    switch (rand)
                    {
                        case 0:
                            MoveTo(LowerDeskPos);
                            Destination = "LOWERDESK";
                            break;
                        case 1:
                            MoveTo(UpperDeskPos);
                            Destination = "UPPERDESK";
                            break;
                        case 2:
                            MoveTo(BucketPos);
                            Destination = "BUCKETAREA";
                            break;
                        case 3:
                            MoveTo(BorderPos);
                            Destination = "BORDERAREA";
                            break;
                        case 4:
                            MoveTo(AnchorPos);
                            Destination = "ANCHORAREA";
                            break;
                    }
                }
                
                break;
            case e_Location.BORDERAREA:
                if (CurrentArea.UsersInArea.Count <2)
                {
                    characterState = e_characterState.BORDER;
                }
                else
                {
                    rand = (int)Random.Range(0, 4);

                    switch (rand)
                    {
                        case 0:
                            MoveTo(LowerDeskPos);
                            Destination = "LOWERDESK";
                            break;
                        case 1:
                            MoveTo(UpperDeskPos);
                            Destination = "UPPERDESK";
                            break;
                        case 2:
                            MoveTo(BucketPos);
                            Destination = "BUCKETAREA";
                            break;
                        case 3:
                            MoveTo(SailPos);
                            Destination = "SAILAREA";
                            break;
                        case 4:
                            MoveTo(AnchorPos);
                            Destination = "ANCHORAREA";
                            break;
                    }
                }
                
                break;
            case e_Location.CANONAREA:
                if (CurrentArea.UsersInArea.Count <2)
                {
                    characterState = e_characterState.RELOAD;
                }
                else
                {
                    MoveTo(LowerDeskPos);
                    Destination = "LOWERDESK";
                }
                
                break;
            case e_Location.ANCHORAREA:
                if (CurrentArea.UsersInArea.Count <2)
                {
                    characterState = e_characterState.ANCHOR;
                }
                else
                {
                    rand = (int)Random.Range(0, 4);

                    switch (rand)
                    {
                        case 0:
                            MoveTo(LowerDeskPos);
                            Destination = "LOWERDESK";
                            break;
                        case 1:
                            MoveTo(UpperDeskPos);
                            Destination = "UPPERDESK";
                            break;
                        case 2:
                            MoveTo(BucketPos);
                            Destination = "BUCKETAREA";
                            break;
                        case 3:
                            MoveTo(SailPos);
                            Destination = "SAILAREA";
                            break;
                        case 4:
                            MoveTo(BorderPos);
                            Destination = "BORDERAREA";
                            break;
                    }
                }
                
                break;
        }
    }
    private bool DiceIt(int min, int max, int successCap)
    {
        int rand = Random.Range(min + (int)SkillLevel, max - Handicap);
        Debug.Log("random result: " + rand + " cap: " + successCap );
        if (rand < successCap)
            return false;
        else
            return true;

    }

    #region Timers
    IEnumerator slapAnimTimer()
    {
        //anim 
        yield return new WaitForSeconds(DurationSlapAnim);
        FinishedIteraction = true;
    }
    IEnumerator gratzAnimTimer()
    {
        //anim 
        yield return new WaitForSeconds(DurationGratzAnim);
        FinishedIteraction = true;
    }
    IEnumerator helpAnimTimer()
    {
        //anim 
        yield return new WaitForSeconds(DurationHelpAnim);
        FinishedIteraction = true;
    }
    IEnumerator threatAnimTimer()
    {
        //anim 
        yield return new WaitForSeconds(DurationThreatAnim);
        FinishedIteraction = true;
    }
    IEnumerator unstuckCanonTimer()
    {
        //anim
        yield return new WaitForSeconds(5f);
        if(!StateDead)
        {
            Interactable = true;
            yield return new WaitForSeconds(TimeRemainInteractableAfterUnstuck);
            if (SlapB)
            {
                Interactable = false;
                StateStuck = false;
                SlapB = false;
                StartCoroutine("slapAnimTimer");
                yield return new WaitForSeconds(DurationSlapAnim);
            }
            else if (GratzB)
            {
                Interactable = false;
                StateStuck = false;
                GratzB = false;
                StartCoroutine("gratzAnimTimer");
                yield return new WaitForSeconds(DurationGratzAnim);
            }
            else if (HelpB)
            {
                Interactable = false;
                StateStuck = false;
                HelpB = false;
                StartCoroutine("helpAnimTimer");
                yield return new WaitForSeconds(DurationHelpAnim);
            }
            else if (ThreatB)
            {
                Interactable = false;
                StateStuck = false;
                ThreatB = false;
                StartCoroutine("threatAnimTimer");
                yield return new WaitForSeconds(DurationThreatAnim);
            }
            Interactable = false;
            StateStuck = false;
            MoveTo(LowerDeskPos);
            Destination = "LOWERDESK";
            CanonBusy = false;
        }
    }
    IEnumerator unstuckSailTimer()
    {
        //anim
        yield return new WaitForSeconds(5f);
        if (!StateDead)
        {
            Interactable = true;
            yield return new WaitForSeconds(TimeRemainInteractableAfterUnstuck);
            if (SlapB)
            {
                Interactable = false;
                StateStuck = false;
                SlapB = false;
                StartCoroutine("slapAnimTimer"); 
                 yield return new WaitForSeconds(DurationSlapAnim);
            }
            else if (GratzB)
            {
                Interactable = false;
                StateStuck = false;
                GratzB = false;
                StartCoroutine("gratzAnimTimer"); 
                yield return new WaitForSeconds(DurationGratzAnim);
            }
            else if (HelpB)
            {
                Interactable = false;
                StateStuck = false;
                HelpB = false;
                StartCoroutine("helpAnimTimer");
                yield return new WaitForSeconds(DurationHelpAnim);
            }
            else if (ThreatB)
            {
                Interactable = false;
                StateStuck = false;
                ThreatB = false;
                StartCoroutine("threatAnimTimer");
                yield return new WaitForSeconds(DurationThreatAnim);
            }
            Interactable = false;
            StateStuck = false;
            MoveTo(UpperDeskPos);
            Destination = "UPPERDESK";
            SailBusy = false;
        }
    }
    IEnumerator unstuckBucketTimer()
    {
        //anim
        yield return new WaitForSeconds(5f);
        if (!StateDead)
        {
            Interactable = true;
            yield return new WaitForSeconds(TimeRemainInteractableAfterUnstuck);
            if (SlapB)
            {
                Interactable = false;
                StateStuck = false;
                SlapB = false;
                StartCoroutine("slapAnimTimer");
                yield return new WaitForSeconds(DurationSlapAnim);
            }
            else if (GratzB)
            {
                Interactable = false;
                StateStuck = false;
                GratzB = false;
                StartCoroutine("gratzAnimTimer");
                yield return new WaitForSeconds(DurationGratzAnim);
            }
            else if (HelpB)
            {
                Interactable = false;
                StateStuck = false;
                HelpB = false;
                StartCoroutine("helpAnimTimer");
                yield return new WaitForSeconds(DurationHelpAnim);
            }
            else if (ThreatB)
            {
                Interactable = false;
                StateStuck = false;
                ThreatB = false;
                StartCoroutine("threatAnimTimer");
                yield return new WaitForSeconds(DurationThreatAnim);
            }
            Interactable = false;
            StateStuck = false;
            MoveTo(UpperDeskPos);
            Destination = "UPPERDESK";
            BucketBusy = false;
        }
    }
    IEnumerator unstuckAnchorTimer()
    {
        //anim
        yield return new WaitForSeconds(5f);
        if (!StateDead)
        {
            Interactable = true;
            yield return new WaitForSeconds(TimeRemainInteractableAfterUnstuck);
            if (SlapB)
            {
                Interactable = false;
                StateStuck = false;
                SlapB = false;
                StartCoroutine("slapAnimTimer");
                yield return new WaitForSeconds(DurationSlapAnim);
            }
            else if (GratzB)
            {
                Interactable = false;
                StateStuck = false;
                GratzB = false;
                StartCoroutine("gratzAnimTimer");
                yield return new WaitForSeconds(DurationGratzAnim);
            }
            else if (HelpB)
            {
                Interactable = false;
                StateStuck = false;
                HelpB = false;
                StartCoroutine("helpAnimTimer");
                yield return new WaitForSeconds(DurationHelpAnim);
            }
            else if (ThreatB)
            {
                Interactable = false;
                StateStuck = false;
                ThreatB = false;
                StartCoroutine("threatAnimTimer");
                yield return new WaitForSeconds(DurationThreatAnim);
            }
            Interactable = false;
            StateStuck = false;
            MoveTo(UpperDeskPos);
            Destination = "UPPERDESK";
            AnchorBusy = false;
        }
    }
    IEnumerator unstuckBorderTimer()
    {
        //anim
        yield return new WaitForSeconds(5f);
        if (!StateDead)
        {
            Interactable = true;
            yield return new WaitForSeconds(TimeRemainInteractableAfterUnstuck);
            if (SlapB)
            {
                Interactable = false;
                StateStuck = false;
                SlapB = false;
                StartCoroutine("slapAnimTimer");
                yield return new WaitForSeconds(DurationSlapAnim);
            }
            else if (GratzB)
            {
                Interactable = false;
                StateStuck = false;
                GratzB = false;
                StartCoroutine("gratzAnimTimer");
                yield return new WaitForSeconds(DurationGratzAnim);
            }
            else if (HelpB)
            {
                Interactable = false;
                StateStuck = false;
                HelpB = false;
                StartCoroutine("helpAnimTimer");
                yield return new WaitForSeconds(DurationHelpAnim);
            }
            else if (ThreatB)
            {
                Interactable = false;
                StateStuck = false;
                ThreatB = false;
                StartCoroutine("threatAnimTimer");
                yield return new WaitForSeconds(DurationThreatAnim);
            }
            Interactable = false;
            
            StateStuck = false;
            MoveTo(UpperDeskPos);
            Destination = "UPPERDESK";
            BorderBusy = false;
        }
    }   
    IEnumerator crewTimerIdle(float time)
    {
        timer01Underway = true;
        yield return new WaitForSeconds(time);
        PickActivity();
        timer01Underway = false;
    }
    IEnumerator slapTimer()
    {
        yield return new WaitForSeconds(DurationOfSlapBoost);
        if (characterState == e_characterState.RUNNING)
        {
            BoostActive = false;
            characterState = e_characterState.WALKING;
        }

    }
    IEnumerator bucketTimer(float time)
    {
        BucketBusy = true;
        yield return new WaitForSeconds(time);
        int rand = (int)Random.Range(0, 4);

        switch (rand)
        {
            case 0:
                MoveTo(LowerDeskPos);
                Destination = "LOWERDESK";
                break;
            case 1:
                MoveTo(UpperDeskPos);
                Destination = "UPPERDESK";
                break;
            case 2:
                MoveTo(SailPos);
                Destination = "SAILAREA";
                break;
            case 3:
                MoveTo(BorderPos);
                Destination = "BORDERAREA";
                break;
            case 4:
                MoveTo(AnchorPos);
                Destination = "ANCHORAREA";
                break;
        }
        BucketBusy = false;
    }
    IEnumerator sailTimer(float time)
    {
        SailBusy = true;
        yield return new WaitForSeconds(time);
        int rand = (int)Random.Range(0, 4);

        switch (rand)
        {
            case 0:
                MoveTo(LowerDeskPos);
                Destination = "LOWERDESK";
                break;
            case 1:
                MoveTo(UpperDeskPos);
                Destination = "UPPERDESK";
                break;
            case 2:
                MoveTo(BucketPos);
                Destination = "BUCKETAREA";
                break;
            case 3:
                MoveTo(BorderPos);
                Destination = "BORDERAREA";
                break;
            case 4:
                MoveTo(AnchorPos);
                Destination = "ANCHORAREA";
                break;
        }
        SailBusy = false;
    }
    IEnumerator canonTimer(float time)
    {
        CanonBusy = true;
        yield return new WaitForSeconds(time);
        MoveTo(LowerDeskPos);
        Destination = "LOWERDESK";
        CanonBusy = false;
    }
    IEnumerator anchorTimer(float time)
    {
        AnchorBusy = true;
        yield return new WaitForSeconds(time);
        int rand = (int)Random.Range(0, 4);
        switch (rand)
        {
            case 0:
                MoveTo(LowerDeskPos);
                Destination = "LOWERDESK";
                break;
            case 1:
                MoveTo(UpperDeskPos);
                Destination = "UPPERDESK";
                break;
            case 2:
                MoveTo(SailPos);
                Destination = "SAILAREA";
                break;
            case 3:
                MoveTo(BorderPos);
                Destination = "BORDERAREA";
                break;
            case 4:
                MoveTo(BucketPos);
                Destination = "BUCKETAREA";
                break;
        }
        AnchorBusy = false;
    }
    IEnumerator borderTimer(float time)
    {
        BorderBusy = true;
        yield return new WaitForSeconds(time);
        int rand = (int)Random.Range(0, 4);

        switch (rand)
        {
            case 0:
                MoveTo(LowerDeskPos);
                Destination = "LOWERDESK";
                break;
            case 1:
                MoveTo(UpperDeskPos);
                Destination = "UPPERDESK";
                break;
            case 2:
                MoveTo(SailPos);
                Destination = "SAILAREA";
                break;
            case 3:
                MoveTo(BucketPos);
                Destination = "BUCKETAREA";
                break;
            case 4:
                MoveTo(AnchorPos);
                Destination = "ANCHORAREA";
                break;
        }
        BorderBusy = false;
    }
    
    #endregion
    #region CrewTryTools
    private void TryBucket() 
    {
        if(DiceIt(0+ modifierSkillOnBucket, 10, 3))
        {
            //success anim etc
            StartCoroutine("bucketTimer", 10f);
        }
        else
        {
            BucketBusy = true;
            //fail anim etc
            Debug.Log("StuckBucket");
            Stuck.Invoke();
            //followed by stuck,need captain;
        }
    }
    private void TrySail()
    {
        if (DiceIt(0+ modifierSkillOnSail, 10, 3))
        {
            //success anim etc
            StartCoroutine("sailTimer", 10f);
        }
        else
        {
            SailBusy = true;
            //fail anim etc
            Stuck.Invoke();
            Debug.Log("StuckSail");
            //followed by stuck,need captain;
        }
    }
    private void TryBorder()
    {
        if (DiceIt(0 + modifierSkillOnBorder, 10, 3))
        {
            //success anim etc
            
            StartCoroutine("borderTimer", 10f);
        }
        else
        {
            BorderBusy = true;
            //fail anim etc
            Stuck.Invoke();
            Debug.Log("StuckBorder");
            //followed by stuck,need captain;
        }
    }
    private void TryCanon()
    {
        if (DiceIt(0+ modifierSkillOnCanon, 10, 3))
        {
            //success anim etc

            StartCoroutine("canonTimer", 10f);
        }
        else
        {
            CanonBusy = true;
            //fail anim etc
            Stuck.Invoke();
            Debug.Log("StuckCanon");
            //followed by stuck,need captain;
        }
    }
    private void TryAnchor()
    {
        if (DiceIt(0 + modifierSkillOnAnchor, 10, 3))
        {
            //success anim etc
            StartCoroutine("anchorTimer", 10f);
        }
        else
        {
            AnchorBusy = true;
            //fail anim etc
            Stuck.Invoke();
            Debug.Log("StuckAnchor");
            //followed by stuck,need captain;
        }
    }
    #endregion



    

    private void CharStateHandle()
    {
        switch (characterState)
        {
            case e_characterState.IDLE:
                if (timer01Underway == false)
                    StartCoroutine("crewTimerIdle", 2f);
                break;
            case e_characterState.WALKING:
                ActualSpeed = SpeedWalk;
                Agent.speed = ActualSpeed;
                break;
            case e_characterState.RUNNING:
                ActualSpeed = SpeedRun;
                Agent.speed = ActualSpeed;
                break;
            case e_characterState.BUCKET:
                if(!BucketBusy)
                {
                    Debug.Log("tryBucket"); 
                    TryBucket();
                }
                break;
            case e_characterState.RELOAD:
                if (!CanonBusy)
                {
                    Debug.Log("tryCanon"); 
                    TryCanon();                
                }
                break;
            case e_characterState.SAIL:
                if (!SailBusy)
                {
                    Debug.Log("trySail");
                    TrySail();
                }
                break;
            case e_characterState.BORDER:
                if (!BorderBusy)
                {
                    Debug.Log("tryBorder");
                    TryBorder();
                }
                break;
            case e_characterState.ANCHOR:
                if (!AnchorBusy)
                {
                    Debug.Log("tryAnchor");
                    TryAnchor();
                }
                break;
        }
    }
}

