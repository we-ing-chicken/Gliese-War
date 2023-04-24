using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadCam : MonoBehaviour
{
    public GameObject head;
    public float xmove = 0;
    public float ymove = 0;
    public float distance = 3;
    public float ymove_max = 60.0f;
    public float ymove_min = -5.0f;

    void Update()
    {
        xmove = Input.GetAxis("Mouse X");

        transform.rotation = Quaternion.Euler(0, xmove, 0);
        Vector3 reverseDistance = new Vector3(0.0f, 0.0f, distance);
        transform.position = head.transform.position - transform.rotation * reverseDistance;
    }
}
