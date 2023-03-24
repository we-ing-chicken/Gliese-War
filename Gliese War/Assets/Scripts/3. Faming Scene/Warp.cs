using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.TextCore.Text;

public class Warp : MonoBehaviour
{
    public GameObject warpTo;
    private bool isLockOn;

    private void Start()
    {
        isLockOn = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !other.GetComponent<Player>().isNear && !isLockOn)
        {
            Debug.Log("Enter");
            other.GetComponent<Player>().isNear = true;
            isLockOn = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && !other.GetComponent<Player>().isNear && !isLockOn)
        {
            other.GetComponent<Player>().isNear = true;
            isLockOn = true;
            return;
        }
        
        if ((other.CompareTag("Player") && isLockOn))
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                Debug.Log("Move");
                other.GetComponent<Player>().isNear = false;
                isLockOn = false;
                
                other.GetComponent<CharacterController>().enabled = false;
                other.transform.position = warpTo.transform.position;
                other.GetComponent<CharacterController>().enabled = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && other.GetComponent<Player>().isNear && isLockOn)
        {
            Debug.Log("Exit");
            other.GetComponent<Player>().isNear = false;
            isLockOn = false;
        }
    }
}