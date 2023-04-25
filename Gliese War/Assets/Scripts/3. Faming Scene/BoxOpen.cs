using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxOpen : MonoBehaviour
{
    private GameObject keyButtonImage;
    private Camera mainCamera;
    private BoxCollider boxCollider;
    private bool isLockOn;
    private Drop drop;

    [SerializeField] private GameObject effectPosition;
    [SerializeField] private GameObject starEffect;

    private Animator anim;
    
    // Start is called before the first frame update
    void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        
        keyButtonImage = gameObject.transform.GetChild(0).gameObject;
        keyButtonImage.SetActive(false);
        isLockOn = false;
        //mainCamera = Player.instance.gameObject.transform.GetChild(0).GetComponent<Camera>();
        mainCamera = Camera.main;
        drop = gameObject.GetComponent<Drop>();

        anim = transform.GetChild(1).GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
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
                boxCollider.enabled = false;
                other.GetComponent<Player>().isNear = false;
                isLockOn = false;
                Instantiate(starEffect, effectPosition.transform.position, Quaternion.identity);
                anim.SetTrigger("Open");
                
                StartCoroutine(AfterOpen());
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

    IEnumerator AfterOpen()
    {
        yield return new WaitForSeconds(1.01f);
        drop.DropItem();
        gameObject.SetActive(false);
    }
}
