using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Light_LookAt : MonoBehaviour
{
    public GameObject player;
    void Update()
    {
        transform.LookAt(player.transform);
    }
}
