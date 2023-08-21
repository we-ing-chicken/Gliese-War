using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magnetic : MonoBehaviour
{
    static public Magnetic instance;

    public GameObject mf;
    
    private float time;
    private float originY;
    private bool gameStart = false;
    
    // Start is called before the first frame update

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        time = 0f;
        originY = transform.localScale.y;
    }

    public void magneticStart()
    {
        gameStart = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameStart)
        {

            // Time.deltaTime 4초
            // 6분 == 360초
            // time * 0.01f == 410초
            // time * 0.0125f == 335초
            // time * 0.015f == 308초
            // time * 0.02f == 205초
            // time * 0.025f == 165초
            // time * 0.05f == 83초
            // time * 0.1f == 41초
            // time * 0.2f == 20초

            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, Time.deltaTime * 0.0225f);
            transform.localScale = new Vector3(transform.localScale.x, originY, transform.localScale.z);
            mf.transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);


            if (transform.localScale.x < 6f)
                Debug.Log(time);
            else
                time += Time.deltaTime;
        }
    }
}
