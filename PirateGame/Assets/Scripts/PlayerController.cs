using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerController))]
public class PlayerController : MonoBehaviour
{
    public float Speed = 1f;
    public float horizontal;
    public float vertical;
    public float turnSmoothing = 10f;
    public float speedDampTime = .1f;
    private float maxSpeedThreshold = 1f;

    private Rigidbody rbody;
    private Transform camTransform;

    private bool CanMove = true;

    public Image ActionsUI;

    void Start()
    {
        rbody = GetComponent<Rigidbody>();
        camTransform = Camera.main.transform;
        ActionsUI.enabled = false;

    }

    private void FixedUpdate()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        PlayerControl(horizontal, vertical);
    }

    private void Update()
    {
        //CaptainsAction();
    }

    void PlayerControl(float horizontal, float vertical)
    {
        if (CanMove)
        {
            if (horizontal != 0f || vertical != 0f)
            {
                Vector3 targetDirection = new Vector3(horizontal, 0f, vertical);

                Vector3 camRight = camTransform.right;
                Vector3 camForward = camTransform.up;
                //Vector3 camForward = Vector3.Lerp(camTransform.up, camTransform.forward,
                //                                    camTransform.forward.x + camTransform.forward.z);
                camForward.y = 0f;
                camRight.y = 0f;

                targetDirection = horizontal * camRight + vertical * camForward;

                Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
                Quaternion newRotation = Quaternion.Lerp(rbody.rotation, targetRotation, turnSmoothing * Time.deltaTime);

                rbody.position += targetDirection * Speed;


                rbody.rotation = newRotation;

                //if (StateType == PlayerState.Base)
                //{
                //    animator.SetBool("Walking", true);
                //}
                //else if (StateType == PlayerState.Fleeing)
                //    animator.SetBool("Running", true);

                //float magnitude = targetDirection.magnitude;
            }
            else
            {
                // animator.SetBool("Walking", false);
                // animator.SetBool("Running", false);
            }
        }
        else
        {
            horizontal = 0f;
            vertical = 0f;
        }
        

    }

    //void CaptainsAction()
    //{
    //    if (Input.GetButtonDown("Cross"))
    //        Debug.Log("Cross enter");
    //    if (Input.GetButtonDown("Circle"))
    //        Debug.Log("Circle enter");
    //    if (Input.GetButtonDown("Square"))
    //        Debug.Log("Square enter");
    //    if (Input.GetButtonDown("Triangle"))
    //        Debug.Log("Triangle enter");
    //}

    void OnTriggerStay(Collider col)
    {
        //if need help
        if (col.gameObject.layer == LayerMask.NameToLayer("Crew"))
        {
            if (col.GetComponent<Crew>().StateStuck == true && Input.GetButtonDown("Cross"))
            {
                CanMove = false;
                ActionsUI.enabled = true;
                col.GetComponent<Crew>().UnStuck.Invoke();
                
            }
            if (col.GetComponent<Crew>().Interactable == true)
            {
                Debug.Log("interactable");
                if (Input.GetButtonDown("Cross"))
                {
                    col.GetComponent<Crew>().Threaten.Invoke();
                }
                if (Input.GetButtonDown("Circle"))
                {
                    col.GetComponent<Crew>().Helped.Invoke();
                }
                if (Input.GetButtonDown("Square"))
                {
                    col.GetComponent<Crew>().Helped.Invoke();
                }
                if (Input.GetButtonDown("Triangle"))
                {
                    col.GetComponent<Crew>().Congratulated.Invoke();
                    
                }
            }
            if(col.GetComponent<Crew>().FinishedIteraction== true)
            {
                CanMove = true;
                Debug.Log(CanMove);
                ActionsUI.enabled = false;
                col.GetComponent<Crew>().FinishedIteraction = false;
            }
        }
    }
}


    
