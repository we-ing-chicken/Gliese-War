using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CServercamTest : MonoBehaviour
{
    public CPlayer player;
    public float xmove = 0;
    public float ymove = 0;
    public float distance = 3;
    public float ymove_max = 60.0f;
    public float ymove_min = -5.0f;
    private Vector3 reverseDistance;

    private void Start()
    {

        // xmove = player.MouseX;
        //
        // ymove -= Input.GetAxis("Mouse Y");
        // ymove = Mathf.Clamp(ymove, ymove_min, ymove_max);
        //
        // transform.rotation = Quaternion.Euler(ymove, xmove, 0);
        // Vector3 reverseDistance = new Vector3(0.0f, -5.0f, distance);
        // transform.position = player.transform.position - transform.rotation * reverseDistance;
        //
        // camTerDis = distance;
    }

    void Update()
    {

        // xmove = player.MouseX;
        //
        // ymove -= Input.GetAxis("Mouse Y");
        // ymove = Mathf.Clamp(ymove, ymove_min, ymove_max);
        //
        // transform.rotation = Quaternion.Euler(ymove, xmove, 0);
        // Vector3 reverseDistance = new Vector3(0.0f, -5.0f, distance);
        // transform.position = player.transform.position - transform.rotation * reverseDistance;
        
        RaycastHit[] hit;

        //float dis = Vector3.Distance(Camera.main.transform.position, Player.instance.transform.position);
        Vector3 direction = (Camera.main.transform.position - CPlayer.instance.transform.position).normalized;
        hit = (Physics.RaycastAll(CPlayer.instance.transform.position, direction, distance));
        
        for (int i = 0; i < hit.Length; ++i)
        {
            if (hit[i].transform.GetComponent<TransparentObject>() != null)
            {
                hit[i].transform.GetComponent<TransparentObject>().StartTransparent();
            }
        }
    }
}