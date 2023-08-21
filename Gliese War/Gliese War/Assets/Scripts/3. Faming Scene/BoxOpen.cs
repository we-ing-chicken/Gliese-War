using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxOpen : MonoBehaviour
{
    private Camera mainCamera;
    private BoxCollider boxCollider;
    private bool isLockOn;
    private Drop drop;

    [SerializeField] private GameObject effectPosition;
    [SerializeField] private GameObject starEffect;

    private Animator anim;
    private AudioSource audio;
    
    // Start is called before the first frame update
    void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        
        isLockOn = false;
        //mainCamera = Player.instance.gameObject.transform.GetChild(0).GetComponent<Camera>();
        mainCamera = Camera.main;
        drop = gameObject.GetComponent<Drop>();

        anim = transform.GetChild(2).GetComponent<Animator>();
        audio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 cameraDir = mainCamera.transform.position - gameObject.transform.position;
        Quaternion rot = Quaternion.LookRotation(cameraDir);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !other.GetComponent<CPlayer>().isNear && !isLockOn)
        {
            other.GetComponent<CPlayer>().isNear = true;
            isLockOn = true;
            FarmingManager.Instance.ActiveG(gameObject);
        }
    }
    
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && !other.GetComponent<CPlayer>().isNear && !isLockOn)
        {
            other.GetComponent<CPlayer>().isNear = true;
            isLockOn = true;
            FarmingManager.Instance.ActiveG(gameObject);
            return;
        }
        
        if ((other.CompareTag("Player") && isLockOn))
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                boxCollider.enabled = false;
                other.GetComponent<CPlayer>().isNear = false;
                isLockOn = false;
                Instantiate(starEffect, effectPosition.transform.position, Quaternion.identity);
                anim.SetTrigger("Open");
                FarmingManager.Instance.UnActiveG();
                audio.Play();
                StartCoroutine(AfterOpen());
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && other.GetComponent<CPlayer>().isNear && isLockOn)
        {
            other.GetComponent<CPlayer>().isNear = false;
            isLockOn = false;
            FarmingManager.Instance.UnActiveG();
        }
    }

    IEnumerator AfterOpen()
    {
        yield return new WaitForSeconds(1.01f);
        if (drop.isFirst)
            drop.DropBasicItem();
        else
            drop.DropItem();
        gameObject.SetActive(false);
    }
}
