using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CServercamTest : MonoBehaviour
{
    public playerScript ps;
    public float xmove = 0;
    public float ymove = 0;
    public float distance = 3;
    public float ymove_max = 60.0f;
    public float ymove_min = -5.0f;

    void Update()
    {
        if (ps == null) return; 
        else
        {
            Debug.Log("!");
            Debug.Log(ps.transform.position);

            RaycastHit[] hit;

            //float dis = Vector3.Distance(Camera.main.transform.position, Player.instance.transform.position);
            Vector3 direction = (Camera.main.transform.position - ps.transform.position).normalized;
            hit = (Physics.RaycastAll(ps.transform.position, direction, distance));

            for (int i = 0; i < hit.Length; ++i)
            {
                if (hit[i].transform.GetComponent<TransparentObject>() != null)
                {
                    hit[i].transform.GetComponent<TransparentObject>().StartTransparent();
                }
            }
        }
        
        
    }
}