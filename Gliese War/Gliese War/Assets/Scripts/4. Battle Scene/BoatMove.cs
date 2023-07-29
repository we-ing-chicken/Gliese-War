using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatMove : MonoBehaviour
{
    float initY;
    float dis;
    float turningPoint;
    bool isUD = true;
    
    // Start is called before the first frame update
    void Start()
    {
        initY = transform.position.y;
        turningPoint = initY - 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y >= initY)
        {
            isUD = false;
        }
        else if (transform.position.y <= turningPoint)
        {
            isUD = true;
        }
        
        if (isUD)
        {
            
            transform.position = transform.position + new Vector3(0, 1, 0) * Time.deltaTime;
        }
        else
        {
            
            transform.position = transform.position + new Vector3(0, -1, 0) * Time.deltaTime;
        }
    }
}
