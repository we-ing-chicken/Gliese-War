using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxOpen : MonoBehaviour
{
    private GameObject keyButtonImage;
    private Camera mainCamera;
    private bool isLockOn;
    
    // Start is called before the first frame update
    void Start()
    {
        keyButtonImage = gameObject.transform.GetChild(0).gameObject;
        keyButtonImage.SetActive(false);
        isLockOn = false;
        //mainCamera = Player.instance.gameObject.transform.GetChild(0).GetComponent<Camera>();
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(mainCamera);
        Vector3 cameraDir = mainCamera.transform.position - gameObject.transform.position;
        Quaternion rot = Quaternion.LookRotation(cameraDir);
        keyButtonImage.transform.rotation = rot;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !other.GetComponent<Player>().isNear && !isLockOn)
        {
            other.GetComponent<Player>().isNear = true;
            isLockOn = true;
            keyButtonImage.SetActive(true);
        }
    }
    
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && !other.GetComponent<Player>().isNear && !isLockOn)
        {
            other.GetComponent<Player>().isNear = true;
            isLockOn = true;
            keyButtonImage.SetActive(true);
            return;
        }
        
        if ((other.CompareTag("Player") && isLockOn))
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                other.GetComponent<Player>().isNear = false;
                isLockOn = false;
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && other.GetComponent<Player>().isNear && isLockOn)
        {
            other.GetComponent<Player>().isNear = false;
            isLockOn = false;
            keyButtonImage.SetActive(false);
        }
    }
}
