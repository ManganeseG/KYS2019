using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipRot : MonoBehaviour
{
    public float RotSpeed = 5f;
    private float tempRot;

    private RectTransform rt;
    void Start()
    {
        rt = GetComponent<RectTransform>();
    }

    void Update()
    {

        rt.eulerAngles = new Vector3(0f, 0f, (Mathf.PingPong(Time.time * RotSpeed, 15f)));
    }
}
