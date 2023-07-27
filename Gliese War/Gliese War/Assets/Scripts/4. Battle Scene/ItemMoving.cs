using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ItemMoving : MonoBehaviour
{
    float initY;
    float dis;
    float turningPoint;
    float rotSpeed = 100f;
    bool isUD = true;

    private void Start()
    {
        initY = transform.position.y;
        turningPoint = initY - 1;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0, rotSpeed * Time.deltaTime, 0));
        
        // if (transform.position.y >= initY)
        // {
        //     isUD = false;
        // }
        // else if (transform.position.y <= turningPoint)
        // {
        //     isUD = true;
        // }
        //
        // if (isUD)
        // {
        //     
        //     transform.position = transform.position + new Vector3(0, 1, 0) * Time.deltaTime;
        // }
        // else
        // {
        //     
        //     transform.position = transform.position + new Vector3(0, -1, 0) * Time.deltaTime;
        // }

    }
}
